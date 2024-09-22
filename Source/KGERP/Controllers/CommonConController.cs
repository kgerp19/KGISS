using System;
using System.Configuration;
using System.Web.Mvc;
namespace KGERP.Controllers
{
    public class CommonConController : Controller
    {
        // GET: CommonCon
        public ActionResult CommonConIndex()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;

            // Parse the connection string to extract the database name
            string databaseName = GetDatabaseNameFromConnectionString(connectionString);

            // Set database name in ViewBag or ViewData
            ViewBag.DatabaseName = databaseName;

            return View();
        }

        private string GetDatabaseNameFromConnectionString(string connectionString)
        {
            try
            {
                var builder = new System.Data.SqlClient.SqlConnectionStringBuilder(connectionString);
                return builder.InitialCatalog;
            }
            catch (ArgumentException ex)
            {
                // Handle the exception appropriately, e.g., log it
                throw new ArgumentException("Invalid connection string format", ex);
            }
        }
    }
}