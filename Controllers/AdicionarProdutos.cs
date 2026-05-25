using _01.Carlos.Fontes.Data;
using _01.Carlos.Fontes.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace _01.Carlos.Fontes.Controllers
{
    // Restringe o acesso ao controller exclusivamente a utilizadores autenticados no sistema
    [Authorize]
    public class AdicionarProdutos : Controller
    {
        private readonly ApplicationDbContext db;

        // Construtor que procede à injeção da instância do contexto da base de dados
        public AdicionarProdutos(ApplicationDbContext context)
        {
            db = context;
        }

        // Método de ação que processa a visualização principal e a persistência de novos níveis de inventário
        public IActionResult Index(string nome, string clicadoEscolher, string clicadoAdicionar, string tamanho, string quantidade)
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

            // Atribuição da variável local para filtragem do produto por nome
            string nomeProduto = nome;
            // Realiza uma consulta LINQ para obter a primeira ocorrência do produto correspondente ao nome facultado
            var produto = db.Produtos.FirstOrDefault(p => p.Nome == nome);

            // Verifica a ativação do evento de seleção e a validade da string de pesquisa
            if (!clicadoEscolher.IsNullOrEmpty() && !nome.IsNullOrEmpty())
            {
                // Atribui os atributos do objeto produto à ViewBag para apresentação de metadados na interface
                ViewBag.nome = produto.Nome;
                ViewBag.descricao = produto.Descricao;
                ViewBag.tipo = produto.Tipo;
            }

            // Avalia se o gatilho de adição foi acionado e se os parâmetros de entrada são válidos
            if (!clicadoAdicionar.IsNullOrEmpty() && !tamanho.IsNullOrEmpty() && !quantidade.IsNullOrEmpty())
            {
                // Filtra o registo específico que concilia a denominação do produto com a variante de dimensão selecionada
                var adicionarProdutos = db.Produtos.FirstOrDefault(p => p.Nome == nomeProduto && p.Tamanho == tamanho);

                if (adicionarProdutos != null)
                {
                    // Executa a operação aritmética de incremento sobre a propriedade Stock do objeto
                    adicionarProdutos.Stock = adicionarProdutos.Stock + Convert.ToInt32(quantidade);
                }

                // Persiste as alterações efetuadas no estado dos objetos na base de dados relacional
                db.SaveChanges();

                // Armazena uma mensagem de confirmação em TempData para ser consumida pela View no próximo ciclo de renderização
                TempData["Sucesso"] = "Produtos adicionados com sucesso";
            }

            // Devolve o resultado da ação através da renderização da View associada
            return View();
        }
    }
}