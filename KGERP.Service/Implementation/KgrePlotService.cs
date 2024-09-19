using KGERP.Data.Models;
using KGERP.Service.Interface;
using System.Linq;

namespace KGERP.Service.Implementation
{
    public class KgrePlotService : IKgrePlotInfoService
    {
        public object LoadProjectName(int companyId)
        {
            ERPEntities db = new ERPEntities();
            object BasicInfo = null;
            BasicInfo = (from basic in db.KGREProjects
                         where basic.CompanyId == companyId
                         select new
                         {
                             basic.ProjectId,
                             basic.ProjectName
                         }).ToList();

            return BasicInfo;
        }
        public object LoadPlotStatus()
        {
            ERPEntities db = new ERPEntities();
            object BasicInfo = null;
            BasicInfo = (from basic in db.DropDownItems
                         where basic.DropDownTypeId == 62
                         select new
                         {
                             basic.DropDownItemId,
                             basic.Name
                         }).ToList();

            return BasicInfo;
        }

        public object LoadPlotInfo()
        {
            //ERPEntities db = new ERPEntities();
            //object BasicInfo = null;
            //BasicInfo = (from basic in db.PloatInfoSetups
            //             join proj in db.KGREProjects on basic.projectId equals proj.ProjectId
            //             select new
            //             {
            //                 basic.pol_id,
            //                 proj.ProjectName,
            //                 basic.projectId,
            //                 basic.PloatNo,
            //                 basic.PlotFace,
            //                 basic.PlotSize,
            //                 basic.BlockNo
            //             });

            //return BasicInfo;

            ERPEntities db = new ERPEntities();
            object BasicInfo = null;
            BasicInfo = (from basic in db.KGREPlots
                         join proj in db.KGREProjects on basic.ProjectId equals proj.ProjectId
                         join pStatus in db.DropDownItems on basic.PlotStatus equals pStatus.DropDownItemId
                         select new
                         {
                             basic.PlotId,
                             proj.ProjectName,
                             basic.ProjectId,
                             basic.PlotNo,
                             basic.PlotFace,
                             basic.PlotSize,
                             basic.BlockNo,
                             pStatus.Name
                         });

            return BasicInfo;
        }

        public object LoadData(int id)
        {
            //ERPEntities db = new ERPEntities();
            //object BasicInfo = null;
            //BasicInfo = (from basic in db.PloatInfoSetups
            //             join proj in db.KGREProjects on basic.projectId equals proj.ProjectId
            //             where basic.pol_id == id
            //             select new
            //             {
            //                 basic.pol_id,
            //                 proj.ProjectName,
            //                 basic.projectId,
            //                 basic.PloatNo,
            //                 basic.PlotFace,
            //                 basic.PlotSize,
            //                 basic.BlockNo
            //             });

            //return BasicInfo;
            ERPEntities db = new ERPEntities();
            object BasicInfo = null;
            BasicInfo = (from basic in db.KGREPlots
                         join proj in db.KGREProjects on basic.ProjectId equals proj.ProjectId
                         join pStatus in db.DropDownItems on basic.PlotStatus equals pStatus.DropDownItemId
                         where basic.PlotId == id
                         select new
                         {
                             basic.PlotId,
                             proj.ProjectName,
                             basic.ProjectId,
                             basic.PlotNo,
                             basic.PlotFace,
                             basic.PlotSize,
                             basic.BlockNo,
                             pStatus.Name
                         });

            return BasicInfo;

        }

        //public int SaveProjectInfo(PloatInfoSetup BasicInfo)
        public int SaveProjectInfo(KGREPlot kGREPlot)
        {
            //PloatInfoSetup projectNameSetup = new PloatInfoSetup();
            //ERPEntities db = new ERPEntities();

            //var ploatNo = (from p in db.PloatInfoSetups
            //               where p.PloatNo == BasicInfo.PloatNo & p.BlockNo == BasicInfo.BlockNo & p.projectId == BasicInfo.projectId
            //               select p.PloatNo).FirstOrDefault();
            //if (ploatNo == null)
            //{
            //    if (BasicInfo.pol_id.Equals(0))
            //    {

            //        projectNameSetup.projectId = BasicInfo.projectId;
            //        projectNameSetup.PloatNo = BasicInfo.PloatNo;
            //        projectNameSetup.PlotFace = BasicInfo.PlotFace;
            //        projectNameSetup.PlotSize = BasicInfo.PlotSize;
            //        projectNameSetup.BlockNo = BasicInfo.BlockNo;
            //        db.PloatInfoSetups.Add(projectNameSetup);
            //        db.SaveChanges();
            //        return 0;
            //    }
            //    else
            //    {

            //        PloatInfoSetup extUser = new PloatInfoSetup();
            //        extUser = (from i in db.PloatInfoSetups
            //                   where i.pol_id == BasicInfo.pol_id
            //                   select i).FirstOrDefault();


            //        extUser.projectId = BasicInfo.projectId;
            //        extUser.PloatNo = BasicInfo.PloatNo;
            //        extUser.PlotFace = BasicInfo.PlotFace;
            //        extUser.PlotSize = BasicInfo.PlotSize;
            //        extUser.BlockNo = BasicInfo.BlockNo;
            //        db.SaveChanges();
            //        return 0;
            //    }
            //}
            //else
            //{
            //    return 1;
            //}

            KGREPlot plot = new KGREPlot();
            ERPEntities db = new ERPEntities();

            var plotNo = (from p in db.KGREPlots
                          where p.PlotNo == kGREPlot.PlotNo & p.BlockNo == kGREPlot.BlockNo & p.ProjectId == kGREPlot.ProjectId
                          select p.PlotNo).FirstOrDefault();
            //if (plotNo == null)
            //{
            if (kGREPlot.PlotId.Equals(0))
            {

                plot.ProjectId = kGREPlot.ProjectId;
                plot.PlotNo = kGREPlot.PlotNo;
                plot.PlotFace = kGREPlot.PlotFace;
                plot.PlotSize = kGREPlot.PlotSize;
                plot.PlotStatus = kGREPlot.PlotStatus;
                plot.BlockNo = kGREPlot.BlockNo;
                db.KGREPlots.Add(plot);
                db.SaveChanges();
                return 0;
            }
            else
            {

                KGREPlot _kGREPlot = new KGREPlot();
                _kGREPlot = (from i in db.KGREPlots
                             where i.PlotId == kGREPlot.PlotId
                             select i).FirstOrDefault();


                _kGREPlot.ProjectId = kGREPlot.ProjectId;
                _kGREPlot.PlotNo = kGREPlot.PlotNo;
                _kGREPlot.PlotFace = kGREPlot.PlotFace;
                _kGREPlot.PlotSize = kGREPlot.PlotSize;
                _kGREPlot.BlockNo = kGREPlot.BlockNo;
                _kGREPlot.PlotStatus = kGREPlot.PlotStatus;
                db.SaveChanges();
                return 0;
            }
            //}
            //else
            //{
            //    return 1;
            //}
        }
    }
}
