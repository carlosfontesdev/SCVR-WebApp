using _01.Carlos.Fontes.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace _01.Carlos.Fontes.Controllers
{
    // Restringe o acesso ao controller exclusivamente a utilizadores autenticados no sistema
    [Authorize]
    public class GerirEncomenda : Controller
    {
        private readonly ApplicationDbContext db;

        // Construtor que procede à injeção da instância do contexto da base de dados
        public GerirEncomenda(ApplicationDbContext context)
        {
            db = context;
        }

        // Método de ação que gere a listagem de encomendas, permitindo filtragem por estado e cálculos de faturação
        public IActionResult Index(String filtro, String clicado)
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
            float TotalFaturado = 0;
            // Obtém a lista de todas as encomendas que se encontram em estado ativo
            var todasEncomendas = db.Encomendas.Where(m => m.Ativa == true).ToList();

            // Verifica se a ação de filtragem ou pesquisa foi despoletada pelo utilizador
            if (!clicado.IsNullOrEmpty())
            {
                // Caso exista um critério de filtragem definido (ex: Estado de Pagamento)
                if (!filtro.IsNullOrEmpty())
                {
                    ViewBag.Encomenda = (from e in db.Encomendas
                                         join u in db.Users on e.AspNetUserId equals u.Id // Junção com tabela de utilizadores
                                         join ep in db.Encomenda_Produto on e.Id equals ep.EncomendaId // Junção com a tabela de ligação
                                         join p in db.Produtos on ep.ProdutoId equals p.Id // Junção com a tabela de produtos
                                         select new
                                         {
                                             Id = e.Id,
                                             Ativa = e.Ativa,
                                             UserName = u.UserName,
                                             NumeroSocio = u.NumeroSocio,
                                             MoradaEnvio = e.MoradaEnvio,
                                             EstadoPagamento = e.EstadoPagamento,
                                             NomeProduto = p.Nome,
                                             Tamanho = p.Tamanho
                                         }).Where(m => m.EstadoPagamento == filtro).OrderByDescending(m => m.Ativa).ToList();

                    // Contabiliza o número total de encomendas ativas para fins estatísticos
                    ViewBag.TotalEncomendas = db.Encomendas.Count(m => m.Ativa == true);

                    // Itera sobre a lista de encomendas para calcular o somatório da faturação total
                    foreach (var item in todasEncomendas)
                    {
                        TotalFaturado = TotalFaturado + item.Total;
                    }
                    ViewBag.TotalFaturado = TotalFaturado;

                    return View();
                }
                else
                {
                    // Caso o botão tenha sido clicado mas sem filtro, apresenta a listagem completa
                    ViewBag.Encomenda = (from e in db.Encomendas
                                         join u in db.Users on e.AspNetUserId equals u.Id
                                         join ep in db.Encomenda_Produto on e.Id equals ep.EncomendaId
                                         join p in db.Produtos on ep.ProdutoId equals p.Id
                                         select new
                                         {
                                             Id = e.Id,
                                             Ativa = e.Ativa,
                                             UserName = u.UserName,
                                             NumeroSocio = u.NumeroSocio,
                                             MoradaEnvio = e.MoradaEnvio,
                                             EstadoPagamento = e.EstadoPagamento,
                                             NomeProduto = p.Nome,
                                             Tamanho = p.Tamanho
                                         }).OrderByDescending(m => m.Ativa).ToList();
                    // Contabiliza o número total de encomendas ativas para fins estatísticos
                    ViewBag.TotalEncomendas = db.Encomendas.Count(m => m.Ativa == true);

                    // Itera sobre a lista de encomendas para calcular o somatório da faturação total
                    foreach (var item in todasEncomendas)
                    {
                        TotalFaturado = TotalFaturado + item.Total;
                    }
                    ViewBag.TotalFaturado = TotalFaturado;

                    return View();
                }
            }

            // Fluxo padrão: Carregamento inicial da página com todos os dados e métricas financeiras
            ViewBag.Encomenda = (from e in db.Encomendas
                                 join u in db.Users on e.AspNetUserId equals u.Id
                                 join ep in db.Encomenda_Produto on e.Id equals ep.EncomendaId
                                 join p in db.Produtos on ep.ProdutoId equals p.Id
                                 select new
                                 {
                                     Id = e.Id,
                                     Ativa = e.Ativa,
                                     UserName = u.UserName,
                                     NumeroSocio = u.NumeroSocio,
                                     MoradaEnvio = e.MoradaEnvio,
                                     EstadoPagamento = e.EstadoPagamento,
                                     NomeProduto = p.Nome,
                                     Tamanho = p.Tamanho
                                 }).OrderByDescending(m => m.Ativa).ToList();

            ViewBag.TotalEncomendas = db.Encomendas.Count(m => m.Ativa == true);

            foreach (var item in todasEncomendas)
            {
                TotalFaturado = TotalFaturado + item.Total;
            }
            ViewBag.TotalFaturado = TotalFaturado;

            // Renderiza a vista com as coleções e totais calculados
            return View();
        }
    }
}