using _01.Carlos.Fontes.Data;
using _01.Carlos.Fontes.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace _01.Carlos.Fontes.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext db;
       

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            db = context;

        }
        
        public IActionResult Index()
        {
            ViewBag.SCVR = db.Classificacaos.Where(m=>m.Id == 1).ToList();
            ViewBag.adversario = db.Jogo.Where(m => m.DataJogo > DateTime.Now).OrderBy(m => m.DataJogo).FirstOrDefault().Adversario;

            return View();
        }

        public IActionResult Privacy()
        {
            
            ViewBag.classificacao = db.Classificacaos.ToList();
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        
    }
}
