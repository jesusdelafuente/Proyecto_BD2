using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Proyecto_BD.DatabaseHelper;
using Proyecto_BD2.Models;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;


namespace Proyecto_BD2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("userSession")))
            {
                ViewBag.User = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("userSession"));

                return View();
            }
            else
            {
                ViewBag.Error = new ErrorHandler()
                {
                    Title = "You must need to login to access this page",
                    ErrorMessage = "Please login",
                    Path = "/Login"
                };

                return View("ErrorHandler");
            }
            

        }

		public ActionResult UpdatePhoto(IFormFile photo)
		{
			if (!string.IsNullOrEmpty(HttpContext.Session.GetString("userSession")))
			{
				ViewBag.User = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("userSession"));

				string foto_Usuario = Path.Combine("img\\", ViewBag.User.ID_Usuario + new FileInfo(photo.FileName).Extension);

				using (var stream = new FileStream(Directory.GetCurrentDirectory() + "\\wwwroot\\" + foto_Usuario, FileMode.Create))
				{
					photo.CopyTo(stream);
				}

				DatabaseHelper.ExecStoreProcedure("spUpdateFotoUsuario", new List<SqlParameter>()
				{
					new SqlParameter("@foto_Usuario", foto_Usuario),
					new SqlParameter("@id_Usuario", ViewBag.User.ID_Usuario)
				});

				ViewBag.User.Foto_Usuario = foto_Usuario;

				return View("Index");
			}
			else
			{
				ViewBag.Error = new ErrorHandler()
				{
					Title = "You must need to login to access this page",
					ErrorMessage = "Please login",
					Path = "/Login"
				};

				return View("ErrorHandler");
			}
		}

		public ActionResult DeletePhoto()
		{
			if (!string.IsNullOrEmpty(HttpContext.Session.GetString("userSession")))
			{
				ViewBag.User = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("userSession"));

				string foto_Usuario = "img\\0.jpg";

				DatabaseHelper.ExecStoreProcedure("spUpdateFotoUsuario", new List<SqlParameter>()
				{
					new SqlParameter("@foto_Usuario", foto_Usuario),
					new SqlParameter("@id_Usuario", ViewBag.User.ID_Usuario)
				});

				ViewBag.User.Foto_Usuario = foto_Usuario;

				return View("Index");
			}
			else
			{
				ViewBag.Error = new ErrorHandler()
				{
					Title = "You must need to login to access this page",
					ErrorMessage = "Please login",
					Path = "/Login"
				};

				return View("ErrorHandler");
			}
		}

		public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}