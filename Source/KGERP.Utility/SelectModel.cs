using System;
using System.Web.Mvc;

namespace KGERP.Utility
{
    public class SelectModel
    {
        public object Text { get; set; }
        public object Value { get; set; }
    }

    public class SelectModelPr
    {
        public string Text { get; set; }
        public int Value { get; set; }
    }
    public class SelectModelEmp
    {
        public object Text { get; set; }
        public long Value { get; set; }
    }
    public class SelectModelForGrade
    {
        public object Text { get; set; }
        public int Value { get; set; }
        public object Code { get; set; }

    }
    public class SelectModelType
    {
        public string Text { get; set; }
        public int Value { get; set; }
    }
    public class SelectModelSaleTitle
    {
        public string Text { get; set; }
        public long Value { get; set; }
    }



  
    public class SelectDDLModel
    {
        public string Text { get; set; }
        public long Value { get; set; }
    }

    public class SelectWorkLebel
    {
        public string Text { get; set; }
        public string ColorName { get; set; }
         public long Value { get; set; }
    }
    public class SelectWorkState
    {
        public string Text { get; set; }
        public long Value { get; set; }
    }
 
    public class SelectDDLModelVM
    {
        public string GroupName { get; set; }
        public string FileNo { get; set; }
        public string PrimaryContactNo { get; set; }
        public long Value { get; set; }
        public string ProductName { get;set; }
    }
    public class SelectMarketingModel
    {
        public string Text { get; set; }
        public int Value { get; set; }
    }
    public class SelectModelInstallmentType
    {
        public string Text { get; set; }
        public int Value { get; set; }
        public bool IsOneTime { get; set; }
        public double IntervalMonths { get; set; }
        public decimal Amount { get; set; }
    }

    public class SelectModelAdvanceType
    {
        public string Text { get; set; }
        public int Value { get; set; }
    }
    public class SceduleInstallment
    {
        public string Text { get; set; }
        public string Title { get; set; }
        public long Value { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string StringDate { get; set; }

    }

    public class SelectedListItemCustom : SelectListItem
    {
        public string CustomProOne { get; set; }
        public int CustomProTwo { get; set; }
        public string CustomProThree { get; set; }
    }




}
