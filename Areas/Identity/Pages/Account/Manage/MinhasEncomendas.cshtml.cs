namespace _01.Carlos.Fontes.Areas.Identity.Pages.Account.Manage;
using _01.Carlos.Fontes.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using _01.Carlos.Fontes.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

public class MinhasEncomendasModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext db;

    public MinhasEncomendasModel(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            db = context;
        }

    public List<dynamic> ListaDeEncomendas { get; set; }
    public async Task<IActionResult> OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return NotFound($"Não foi possível carregar o utilizador com ID '{_userManager.GetUserId(User)}'.");
            }
       
        ListaDeEncomendas = (from en in db.Encomendas
                             join encPro in db.Encomenda_Produto on en.Id equals encPro.EncomendaId
                             join prod in db.Produtos on encPro.ProdutoId equals prod.Id
                             where en.AspNetUserId == userId
                             select (dynamic)new
                             {
                                 DataEncomenda = en.DataEncomenda,
                                 MoradaEnvio = en.MoradaEnvio,
                                 PrecoVenda = encPro.PrecoVenda, 
                                 Tipo = prod.Tipo,
                                 Tamanho = prod.Tamanho
                             }).ToList();
        //ListaDeEncomendas = (from en in db.Encomendas
        //           join produtos in db.Produtos on en.Id equals produtos.EncomendaId
        //            where en.ClienteId == userId
        //            select (dynamic)new
        //           {
        //                en.DataEncomenda,
        ///                en.MoradaEnvio,
        ////                produtos.Tamanho,
        ////                produtos.Preco,
        ////                produtos.Nome,
        ////            }).ToList();

        return Page();
        }

   
    }


