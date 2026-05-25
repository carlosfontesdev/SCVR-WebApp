namespace _01.Carlos.Fontes.Areas.Identity.Pages.Account.Manage;

using _01.Carlos.Fontes.Data;
using _01.Carlos.Fontes.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;


public class SouSocioModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext db;

    public SouSocioModel(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        db = context;
    }
    public bool isSocio { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = db.Users.Where(m=>m.Id==userId);
       
        foreach(var item in user)
        {
            isSocio = item.IsSocio;
        }
        return Page();
    }

   
    public async Task<IActionResult> OnPostAsync(String email, String numeroSocio, String clicado)
    {
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = db.Users.Where(m => m.Id == userId);
        if (userId == null)
        {
            return NotFound($"Não foi possível carregar o utilizador com ID '{_userManager.GetUserId(User)}'.");
        }
        if(!clicado.IsNullOrEmpty())
        {
            if (db.Socio.Any(m => m.Email == email && m.NumeroSocio == Convert.ToInt32(numeroSocio)))
            {
                foreach (var item in user)
                {
                    item.NumeroSocio = Convert.ToInt16(numeroSocio);
                    item.IsSocio = true;
                }
                db.SaveChanges();
                TempData["Sucesso"] = "Parabéns é Sócio do Sport Clube de Vila Real";
                return Page();
            }
            TempData["Erro"] = $"Ainda não é Sócio por favor dirija-se á sede do Clube!";
            return Page();
        }
       

        return Page();
    }


}


