using _01.Carlos.Fontes.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace _01.Carlos.Fontes.Controllers
{
    // Restringe o acesso ao controller exclusivamente a utilizadores autenticados no sistema
    [Authorize]
    public class GerirProduto : Controller
    {
        private readonly ApplicationDbContext db;

        // Construtor que procede à injeção da instância do contexto da base de dados
        public GerirProduto(ApplicationDbContext context)
        {
            db = context;
        }

        // Método de ação que gere o inventário, permitindo a filtragem de produtos por nome e tamanho
        public IActionResult Index(string clicado, string nome, string tamanho)
        {
            // Recupera o identificador único do utilizador através das Claims do contexto de segurança
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Procura na tabela de utilizadores o registo correspondente ao ID obtido
            var user = db.Users.FirstOrDefault(m => m.Id == userId);

            // Valida a existência do utilizador e verifica se o mesmo detém privilégios de administrador
            if (user == null || user.IsAdmin == false)
            {
                // Bloqueia o acesso a não-administradores, redirecionando-os para a página inicial
                return Redirect("~/Home/Index");
            }

            // Verifica se a intenção de filtragem foi acionada pelo utilizador
            if (!clicado.IsNullOrEmpty())
            {
                // Cenário 1: Pesquisa sem critérios específicos - retorna todos os produtos com stock disponível
                if (nome.IsNullOrEmpty() && tamanho.IsNullOrEmpty())
                {
                    ViewBag.Produto = db.Produtos.Where(p => p.Stock != 0).ToList();
                    return View();
                }
                // Cenário 2: Filtragem exclusiva por variante de dimensão (tamanho)
                else if (nome.IsNullOrEmpty())
                {
                    ViewBag.Produto = db.Produtos.Where(p => p.Stock != 0 && p.Tamanho == tamanho).ToList();
                    return View();
                }
                // Cenário 3: Filtragem exclusiva pela denominação do artigo (nome)
                else if (tamanho.IsNullOrEmpty())
                {
                    ViewBag.Produto = db.Produtos.Where(p => p.Stock != 0 && p.Nome == nome).ToList();
                    return View();
                }
                // Cenário 4: Filtragem combinada por nome e tamanho específicos
                else
                {
                    ViewBag.Produto = db.Produtos.Where(p => p.Stock != 0 && p.Nome == nome && p.Tamanho == tamanho).ToList();
                    return View();
                }
            }
            else
            {
                // Fluxo padrão: Apresentação inicial de todo o catálogo de produtos ativos com stock positivo
                ViewBag.Produto = db.Produtos.Where(p => p.Stock != 0).ToList();
                return View();
            }
        }
    }
}