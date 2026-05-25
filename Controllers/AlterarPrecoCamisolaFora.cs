using _01.Carlos.Fontes.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace _01.Carlos.Fontes.Controllers
{
    // Restringe o acesso ao controller exclusivamente a utilizadores autenticados no sistema
    [Authorize]
    public class AlterarPrecoCamisolaFora : Controller
    {
        private readonly ApplicationDbContext db;

        // Construtor que procede à injeção da instância do contexto da base de dados
        public AlterarPrecoCamisolaFora(ApplicationDbContext context)
        {
            db = context;
        }

        // Método de ação que processa a visualização principal e a atualização do preçário do produto
        public IActionResult Index(string novoPreco, string clicado)
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
                // Filtra o conjunto de registos na entidade Produtos que correspondem à denominação "Camisola Fora SCVR"
                var produto = db.Produtos.Where(p => p.Nome == "Camisola Fora SCVR");

                // Itera sobre a coleção de resultados para proceder à atualização do atributo de valor unitário
                foreach (var item in produto)
                {
                    // Realiza a conversão explícita do valor de entrada para o tipo decimal e atribui ao campo Preco
                    item.Preco = Convert.ToDecimal(novoPreco);
                }

                // Persiste as alterações efetuadas no estado dos objetos na base de dados relacional
                db.SaveChanges();

                // Armazena uma mensagem de confirmação em TempData para ser consumida pela View no próximo ciclo de renderização
                TempData["Sucesso"] = "Preço Alterado com Sucesso";

            }

            // Recupera o valor pecuniário atual do produto via LINQ para exibição informativa na interface de utilizador
            ViewBag.precoAtualCamisolaFora = db.Produtos.Where(p => p.Nome == "Camisola Fora SCVR").Select(p => p.Preco).FirstOrDefault();

            // Devolve o resultado da ação através da renderização da View associada
            return View();
        }
    }
}