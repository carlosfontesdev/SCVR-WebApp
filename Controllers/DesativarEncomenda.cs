using _01.Carlos.Fontes.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace _01.Carlos.Fontes.Controllers
{
    // Restringe o acesso ao controller exclusivamente a utilizadores autenticados no sistema
    [Authorize]
    public class DesativarEncomenda : Controller
    {
        private readonly ApplicationDbContext db;

        // Construtor que procede à injeção da instância do contexto da base de dados
        public DesativarEncomenda(ApplicationDbContext context)
        {
            db = context;
        }

        // Método de ação que processa a visualização principal e a alteração do estado lógico de encomendas
        public IActionResult Index(String Id, String clicado)
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
                var encomenda = db.Encomendas.Where(m => m.Id == Convert.ToInt16(Id)).ToList();

                // Itera sobre o conjunto de resultados para proceder à desativação lógica do registo
                foreach (var item in encomenda)
                {
                    // Define o atributo de atividade da encomenda como falso para cessar a sua validade no sistema
                    item.Ativa = false;
                }

                // Persiste as alterações efetuadas no estado dos objetos na base de dados relacional
                db.SaveChanges();

                // Redireciona o fluxo de execução para a interface de gestão de encomendas após a atualização bem-sucedida
                return Redirect("~/GerirEncomenda/Index");
            }
            else
            {
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
}