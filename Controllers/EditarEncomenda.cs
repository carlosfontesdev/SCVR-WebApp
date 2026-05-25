using _01.Carlos.Fontes.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace _01.Carlos.Fontes.Controllers
{
    // Restringe o acesso ao controller exclusivamente a utilizadores autenticados no sistema
    [Authorize]
    public class EditarEncomenda : Controller
    {
        private readonly ApplicationDbContext db;

        // Construtor que procede à injeção da instância do contexto da base de dados
        public EditarEncomenda(ApplicationDbContext context)
        {
            db = context;
        }

        // Método de ação que processa a visualização principal e a modificação de registos de encomendas e produtos associados
        public IActionResult Index(String Id, String clicado, String Tamanho, String MoradaEnvio, String EstadoPagamento)
        {
            // Recupera o identificador único do utilizador através das Claims do contexto de segurança
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Procura na tabela de utilizadores o registo correspondente ao ID obtido
            var user = db.Users.FirstOrDefault(m => m.Id == userId);

            // Valida a existência do utilizador e verifica se o mesmo detém privilégios de administrador
            if (user == null || user.IsAdmin == false)
            {
                // Em caso de ausência de privilégios, executa o redirecionamento para o ponto de entrada da aplicação
                return Redirect("~/Home/Index");
            }

            // Avalia se o gatilho de submissão foi acionado através da verificação da string de controlo
            if (!clicado.IsNullOrEmpty())
            {
                // Filtra a coleção de encomendas que correspondem ao identificador facultado, convertendo o resultado numa lista
                var editarEncomenda = db.Encomendas.Where(m => m.Id == Convert.ToInt32(Id)).ToList();

                // Itera sobre a lista de encomendas obtidas para proceder à atualização dos dados
                foreach (var item in editarEncomenda)
                {
                    // Atualiza o atributo relativo ao estado de liquidação da encomenda com o valor proveniente dos parâmetros
                    item.EstadoPagamento = EstadoPagamento;
                    // Atualiza o atributo da localização para expedição da encomenda com o valor facultado
                    item.MoradaEnvio = MoradaEnvio;

                    // Recupera os registos da tabela de ligação Encomenda_Produto que referenciam o identificador da encomenda atual
                    var Encomenda_Produto = db.Encomenda_Produto.Where(m => m.EncomendaId == item.Id).ToList();

                    // Itera sobre a coleção de relações entre encomendas e produtos
                    foreach (var enc in Encomenda_Produto)
                    {
                        // Identifica o registo do produto associado através do identificador de produto presente na tabela de ligação
                        var Produto = db.Produtos.Where(m => m.Id == enc.ProdutoId).ToList();

                        // Percorre a coleção de produtos identificados para aplicar as edições necessárias
                        foreach (var pro in Produto)
                        {
                            // Sobrescreve o atributo de dimensão (tamanho) do produto com a nova especificação
                            pro.Tamanho = Tamanho;
                        }
                    }
                }

                // Persiste as alterações efetuadas no estado dos objetos na base de dados relacional
                db.SaveChanges();

                // Armazena uma mensagem de confirmação em TempData para ser consumida pela View no próximo ciclo de renderização
                TempData["Sucesso"] = "Encomenda Editada com Sucesso";
            }

            // Executa uma consulta LINQ complexa com junções (joins) para agregar dados de utilizadores, encomendas e produtos
            ViewBag.Encomenda = (from e in db.Encomendas
                                 join u in db.Users on e.AspNetUserId equals u.Id // Liga User à Encomenda
                                 join ep in db.Encomenda_Produto on e.Id equals ep.EncomendaId // Liga Encomenda à tabela de ligação
                                 join p in db.Produtos on ep.ProdutoId equals p.Id // Liga tabela de ligação ao Produto
                                 select new
                                 {
                                     id = e.Id,
                                     UserName = u.UserName,
                                     NumeroSocio = u.NumeroSocio,
                                     MoradaEnvio = e.MoradaEnvio,
                                     EstadoPagamento = e.EstadoPagamento,
                                     NomeProduto = p.Nome,
                                     Tamanho = p.Tamanho
                                 }).Where(m => m.id == Convert.ToInt32(Id)).ToList();

            // Devolve o resultado da ação através da renderização da View associada
            return View();
        }
    }
}