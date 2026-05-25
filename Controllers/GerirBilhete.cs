using _01.Carlos.Fontes.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;

namespace _01.Carlos.Fontes.Controllers
{
    // Restringe o acesso ao controller exclusivamente a utilizadores autenticados no sistema
    [Authorize]
    public class GerirBilhete : Controller
    {
        private readonly ApplicationDbContext db;

        // Construtor que procede à injeção da instância do contexto da base de dados
        public GerirBilhete(ApplicationDbContext context)
        {
            db = context;
        }

        // Método de ação que gere a listagem de bilhetes, permitindo cálculos de bilhética e faturação
        public IActionResult Index()
        {
            // Recupera o identificador único do utilizador através das Claims do contexto de segurança
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Procura na tabela de utilizadores o registo correspondente ao ID obtido
            var user = db.Users.FirstOrDefault(m => m.Id == userId);

            // Valida a existência do utilizador e verifica se o mesmo detém privilégios de administrador
            if (user == null || user.IsAdmin == false)
            {
                // Em caso de ausência de privilégios, redireciona para a página inicial
                return Redirect("~/Home/Index");
            }

            // Inicialização da variável para acumulação do montante total de vendas
            double valorFaturado = 0;

            // Executa uma consulta LINQ para projetar detalhes dos bilhetes vendidos para jogos futuros, incluindo dados de utilizador e jogo
            ViewBag.Bilhete = (from b in db.Bilhetes
                               join u in db.Users on b.AspNetUserId equals u.Id // Junção com tabela de utilizadores para obter detalhes do comprador
                               join j in db.Jogo on b.JogoId equals j.Id // Junção com a tabela de jogos para associar o evento desportivo
                               select new
                               {
                                   Id = b.Id,
                                   DataCompra = b.DataCompra,
                                   NumeroBilhete = b.NumeroBilhete,
                                   UserName = u.UserName,
                                   dataJogo = j.DataJogo,
                                   Adversario = j.Adversario,
                                   NumeroSocio = u.NumeroSocio
                               }).Where(m => m.dataJogo > DateTime.Now).OrderBy(m => m.dataJogo).ToList();

            // Realiza a contagem total de bilhetes vendidos para eventos que ainda não ocorreram
            ViewBag.TotalBilhetes = (from b in db.Bilhetes
                                     join j in db.Jogo on b.JogoId equals j.Id
                                     select new
                                     {
                                         dataJogo = j.DataJogo
                                     }).Where(m => m.dataJogo > DateTime.Now).OrderBy(m => m.dataJogo).Count();

            // Recupera a lista de preços pagos por bilhete para jogos futuros para efeitos de cálculo financeiro
            var bilhetesJogos = (from b in db.Bilhetes
                                 join j in db.Jogo on b.JogoId equals j.Id
                                 select new
                                 {
                                     dataJogo = j.DataJogo,
                                     valorPago = b.PrecoPago
                                 }).Where(m => m.dataJogo > DateTime.Now).OrderBy(m => m.dataJogo).ToList();

            // Itera sobre a lista de transações para calcular o somatório da faturação total de bilhética
            foreach (var item in bilhetesJogos)
            {
                valorFaturado = valorFaturado + item.valorPago;
            }

            // Disponibiliza o montante acumulado à View para apresentação do relatório financeiro
            ViewBag.ValorFaturado = valorFaturado;

            // Renderiza a vista com as coleções e totais calculados
            return View();
        }
    }
}