using _01.Carlos.Fontes.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace _01.Carlos.Fontes.Controllers
{
    // Restringe o acesso ao controller exclusivamente a utilizadores autenticados no sistema
    [Authorize]
    public class GerirUtilizadores : Controller
    {
        private readonly ApplicationDbContext db;

        // Construtor que procede à injeção da instância do contexto da base de dados
        public GerirUtilizadores(ApplicationDbContext context)
        {
            db = context;
        }

        // Método de ação que lista todos os utilizadores registados no sistema
        public IActionResult Index()
        {
            // Recupera o identificador único do utilizador atual através das Claims de segurança
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Procura o registo do utilizador para validar o seu nível de privilégio
            var user = db.Users.FirstOrDefault(m => m.Id == userId);

            // Valida se o utilizador tem permissões de administrador
            if (user == null || user.IsAdmin == false)
            {
                // Redireciona para a página inicial caso não possua privilégios administrativos
                return Redirect("~/Home/Index");
            }

            // Disponibiliza a lista completa de utilizadores à View através da ViewBag
            ViewBag.Utilizadores = db.Users.ToList();
            return View();
        }

        // Método de ação que gere a remoção de contas de utilizador do sistema
        public IActionResult EliminarUtilizador(String Id, String clicado)
        {
            // Verifica se a intenção de eliminação foi confirmada (via clique no botão)
            if (!clicado.IsNullOrEmpty())
            {
                // Localiza o utilizador alvo através do identificador fornecido
                var user = db.Users.FirstOrDefault(m => m.Id == Id);

                if (user != null)
                {
                    // Remove o registo do utilizador do contexto da base de dados
                    db.Users.Remove(user);
                    // Persiste a eliminação de forma definitiva na base de dados
                    db.SaveChanges();

                    // Redireciona para a listagem principal após a remoção bem-sucedida
                    return Redirect("~/GerirUtilizadores/Index");
                }
            }
            else
            {
                // Caso seja apenas a visualização de confirmação, carrega os dados do utilizador específico
                ViewBag.Utilizador = db.Users.Where(m => m.Id == Id);
            }

            // Renderiza a View de confirmação ou erro
            return View();
        }
    }
}