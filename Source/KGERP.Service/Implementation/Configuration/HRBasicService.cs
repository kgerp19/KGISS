using KGERP.Data.CustomModel;
using KGERP.Data.Models;
using KGERP.Service.Configuration;
using KGERP.Service.Implementation.Accounting;
using KGERP.Service.Implementation.CurrencyCon;
using KGERP.Service.Implementation.PortCountry;
using KGERP.Service.ServiceModel;
using KGERP.Service.ServiceModel.RealState;
using KGERP.Utility;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.Remoting.Contexts;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace KGERP.Service.Implementation

{
    public class HRBasicService
    {
        private readonly ERPEntities _db;

        public HRBasicService(ERPEntities db)
        {
            _db = db;
        }

        #region Grade
        public async Task<VMGrade> GradeGet()
        {
            VMGrade vmGrade = new VMGrade();
            vmGrade.DataList = await Task.Run(() => GradeDataLoad());
            return vmGrade;
        }

        public IEnumerable<VMGrade> GradeDataLoad()
        {
            var v = (from t1 in _db.Grades
                     where t1.IsActive == true
                     select new VMGrade
                     {
                         GradeId = t1.GradeId,
                         Name = t1.Name,
                         GradeCode = t1.GradeCode
                     }).OrderByDescending(x => x.GradeId).AsEnumerable();
            return v;
        }


        public async Task<int> GradeAdd(VMGrade vmGrade)
        {
            var result = -1;


            Grade grade = new Grade
            {
                Name = vmGrade.Name,
                GradeCode = vmGrade.GradeCode,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreatedDate = DateTime.Now,
                IsActive = true
            };
            _db.Grades.Add(grade);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = grade.GradeId;
            }
            return result;
        }
        public async Task<int> GradeEdit(VMGrade vmGrade)
        {
            var result = -1;
            using (DbContextTransaction dbTran = _db.Database.BeginTransaction())
            {
                try
                {
                    Grade grad = _db.Grades.Find(vmGrade.GradeId);


                    grad.Name = vmGrade.Name;
                    grad.GradeCode = vmGrade.GradeCode;

                    await _db.SaveChangesAsync();
                    result = grad.GradeId;
                    dbTran.Commit();
                }
                catch (DbEntityValidationException ex)
                {
                    dbTran.Rollback();
                    //throw;
                }
            }
            return result;
        }
        public async Task<int> GradeDelete(int id)
        {
            int result = -1;

            if (id != 0)
            {
                using (DbContextTransaction dbTran = _db.Database.BeginTransaction())
                {
                    try
                    {
                        Grade grad = _db.Grades.Find(id);


                        grad.IsActive = false;

                        await _db.SaveChangesAsync();
                        result = grad.GradeId;
                        dbTran.Commit();
                    }
                    catch (DbEntityValidationException ex)
                    {
                        dbTran.Rollback();
                        //throw;
                    }
                }
            }
            return result;
        }
        #endregion

        #region Departments
        public async Task<VMDepartment> DepartmentGet()
        {
            VMDepartment vmDepartment = new VMDepartment();
            vmDepartment.DataList = (from t1 in _db.Departments
                                     where t1.IsActive == true
                                     select new VMDepartment
                                     {
                                         DepartmentId = t1.DepartmentId,
                                         Name = t1.Name,
                                         IsActive = t1.IsActive,
                                         CreatedBy = t1.CreatedBy
                                     }).OrderByDescending(x => x.DepartmentId).AsEnumerable();
            return vmDepartment;
        }
        public async Task<int> DepartmentAdd(VMDepartment vmDepartment)
        {
            var result = -1;


            Department department = new Department
            {
                Name = vmDepartment.Name,                
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreatedDate = DateTime.Now,
                IsActive = true
            };
            _db.Departments.Add(department);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = department.DepartmentId;
            }
            return result;
        }
        public async Task<int> DepartmentEdit(VMDepartment vmDepartment)
        {
            var result = -1;
            using (DbContextTransaction dbTran = _db.Database.BeginTransaction())
            {
                try
                {
                    Department department = _db.Departments.Find(vmDepartment.DepartmentId);
                    department.Name = vmDepartment.Name;                    
                   await _db.SaveChangesAsync();
                    result = department.DepartmentId;
                    dbTran.Commit();
                }
                catch (DbEntityValidationException ex)
                {
                    dbTran.Rollback();
                    //throw;
                }
            }
            return result;
        }
        public async Task<int> DepartmentDelete(int id)
        {
            int result = -1;

            if (id != 0)
            {
                using (DbContextTransaction dbTran = _db.Database.BeginTransaction())
                {
                    try
                    {
                        Department department = _db.Departments.Find(id);


                        department.IsActive = false;

                        await _db.SaveChangesAsync();
                        result = department.DepartmentId;
                        dbTran.Commit();
                    }
                    catch (DbEntityValidationException ex)
                    {
                        dbTran.Rollback();
                        //throw;
                    }
                }
            }
            return result;
        }
        #endregion

        #region DropDownItem 
        public async Task<DropDownItemModel> DropDownItemsGet(int? dropDownTypeId)
        {
            DropDownItemModel dropDownItemModel = new DropDownItemModel();
            if (dropDownTypeId > 0 && dropDownTypeId != null)
            {
                dropDownItemModel = (from t1 in _db.DropDownTypes
                                     where t1.DropDownTypeId == dropDownTypeId
                                     select new DropDownItemModel
                                     {
                                         DropDownTypeId = t1.DropDownTypeId,
                                         TypeName = t1.Name,
                                     }).FirstOrDefault();
            }

            dropDownItemModel.DataList = (from t1 in _db.DropDownItems
                                          join t2 in _db.DropDownTypes on t1.DropDownTypeId equals t2.DropDownTypeId
                                     where t1.IsActive == true && t1.DropDownTypeId == dropDownTypeId
                                     select new DropDownItemModel
                                     {
                                         DropDownItemId = t1.DropDownItemId,
                                         Name = t1.Name,
                                         IsActive = t1.IsActive,
                                         CreatedBy = t1.CreatedBy,
                                         DropDownTypeId = t1.DropDownTypeId,
                                         TypeName = t2.Name,
                                     }).OrderByDescending(x => x.DropDownItemId).AsEnumerable();
            return dropDownItemModel;
        }
        public async Task<int> DropDownItemsAdd(DropDownItemModel dropDownItemModel)
        {
            var result = -1;


            DropDownItem dropDownItem = new DropDownItem
            {
                Name = dropDownItemModel.Name,
                DropDownTypeId = dropDownItemModel.DropDownTypeId,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreatedDate = DateTime.Now,
                IsActive = true,
                CompanyId= dropDownItemModel.CompanyId
            };
            _db.DropDownItems.Add(dropDownItem);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = (int)dropDownItem.DropDownTypeId;
            }
            return result;
        }
        public async Task<int> DropDownItemEdit(DropDownItemModel dropDownItemModel)
        {
            var result = -1;
            using (DbContextTransaction dbTran = _db.Database.BeginTransaction())
            {
                try
                {
                    DropDownItem dropDownItem = _db.DropDownItems.Find(dropDownItemModel.DropDownItemId);
                    dropDownItem.Name = dropDownItemModel.Name;
                    dropDownItem.DropDownTypeId = dropDownItemModel.DropDownTypeId;

                    await _db.SaveChangesAsync();
                    result = (int)dropDownItem.DropDownTypeId;
                    dbTran.Commit();
                }
                catch (DbEntityValidationException ex)
                {
                    dbTran.Rollback();
                    //throw;
                }
            }
            return result;
        }
        public async Task<int> DropDownItemDelete(int id)
        {
            int result = -1;

            if (id != 0)
            {
                using (DbContextTransaction dbTran = _db.Database.BeginTransaction())
                {
                    try
                    {
                        DropDownItem dropDownItem = _db.DropDownItems.Find(id);
                        dropDownItem.IsActive = false;
                        await _db.SaveChangesAsync();
                        result = (int)dropDownItem.DropDownTypeId;
                        dbTran.Commit();
                    }
                    catch (DbEntityValidationException ex)
                    {
                        dbTran.Rollback();
                        //throw;
                    }
                }
            }
            return result;
        }
        #endregion

    }



}

