namespace _01.Carlos.Fontes.Areas.Identity.Pages.Account.Manage;
using _01.Carlos.Fontes.Data;
using _01.Carlos.Fontes.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;


    public class MeusBilhetesModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext db;

    public MeusBilhetesModel(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            db = context;
        }

        public List<dynamic> ListaDeBilhetes;
        public async Task<IActionResult> OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return NotFound($"Não foi possível carregar o utilizador com ID '{_userManager.GetUserId(User)}'.");
            }

            ListaDeBilhetes = (from b in db.Bilhetes
                                 join j in db.Jogo on b.JogoId equals j.Id
                               where b.AspNetUserId == userId
                                 select (dynamic)new
                                 {
                                     Adversario = j.Adversario,
                                     DataCompra = b.DataCompra,
                                     NumeroBilhete = b.NumeroBilhete,
                                     UserId = b.AspNetUserId
                                 }).ToList();
        

            
            return Page();
        }

   
}


