using _01.Carlos.Fontes.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace _01.Carlos.Fontes.Controllers
{
    public class Bilheteira : Controller
    {
        private readonly ApplicationDbContext db;

        // Construtor que procede à injeção da instância do contexto da base de dados
        public Bilheteira(ApplicationDbContext context)
        {
            db = context;
        }

        // Método de ação que processa a visualização principal e a lógica de redirecionamento para aquisição de bilhética
        public IActionResult Index(String clicado)
        {
            // Recupera o valor pecuniário base do próximo evento desportivo via LINQ para exibição informativa na interface de utilizador
            ViewBag.precoBilhete = db.Jogo.Where(m => m.DataJogo > DateTime.Now).OrderBy(m => m.DataJogo).FirstOrDefault().PrecoBase;

            // Extrai a designação do adversário do próximo encontro agendado para apresentação na camada de visualização
            ViewBag.adversario = db.Jogo.Where(m => m.DataJogo > DateTime.Now).OrderBy(m => m.DataJogo).FirstOrDefault().Adversario;

            // Avalia se o gatilho de submissão foi acionado e valida o estado de autenticação do utilizador no contexto de segurança
            if (!clicado.IsNullOrEmpty() && User.Identity.IsAuthenticated)
            {
                // Em caso de utilizador autenticado, executa o redirecionamento para o fluxo de finalização de transação
                return Redirect("~/EfetuarCompraBilhete/Index");
            }
            // Avalia a ativação do evento de interação em instâncias onde o utilizador não se encontra autenticado
            else if (!clicado.IsNullOrEmpty() && !User.Identity.IsAuthenticated)
            {
                // Redireciona o fluxo de execução para a interface de autenticação de identidade do sistema
                return Redirect("~/Identity/Account/Login");
            }
            else
            {
                // Devolve o resultado da ação através da renderização da View associada
                return View();
            }
        }
    }
}