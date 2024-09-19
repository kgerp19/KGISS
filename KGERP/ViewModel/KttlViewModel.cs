using KGERP.Data.Models;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web;

namespace KGERP.ViewModel
{
    public class KttlViewModel
    {
        public KttlCustomerModel kttlModel { get; set; }
        public KttlServiceModel kttlServiceModel { get; set; }
        public KGREPaymentModel Payment { get; set; }
        public List<SelectModel> VoucherTypes { get; set; }
        public List<SelectModel> CostCenters { get; set; }
        public List<SelectModel> Customers { get; set; }
        public List<SelectModel> PaymentModes { get; set; }
        public List<SelectModel> PaymentFors { get; set; }
        public List<SelectModel> Banks { get; set; }
        public KGREPlotBookingModel kGREPlotBookingModel { get; set; }
        public KttlViewModel()
        {
            KttlCustomerModel _KttlCustomerModel = new KttlCustomerModel();
        }
        public IEnumerable<KGRECustomer> KGRECustomers { get; set; }
        public List<SelectModel> SourceOfMedias { get; set; }
        public List<SelectModel> PromotionalOffers { get; set; }
        public List<SelectModel> ResponsiblePersons { get; set; }
        public List<SelectModel> PlotFlats { get; set; }
        public List<SelectModel> Impressions { get; set; }
        public List<SelectModel> StatusLevels { get; set; }
        public List<SelectModel> Genders { get; set; }
        public List<SelectModel> Religions { get; set; }
        public List<SelectModel> KGREProjects { get; set; }
        public List<SelectModel> Plots { get; set; }
        public List<SelectModel> KGREChoiceAreas { get; set; }

        public List<SelectModel> SalesTypes { get; set; }
        [DisplayName("Upload File")]
        public string FilePath { get; set; }
        public HttpPostedFileBase ExcelFile { get; set; }
    }
}