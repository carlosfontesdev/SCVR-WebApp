using _01.Carlos.Fontes.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace _01.Carlos.Fontes.Controllers
{
    // Controller responsável por gerir a visualização do catálogo da loja e o fluxo de redirecionamento de compra
    public class Loja : Controller
    {
        private readonly ApplicationDbContext db;

        // Construtor que procede à injeção da instância do contexto da base de dados
        public Loja(ApplicationDbContext context)
        {
            db = context;
        }

        // Action principal da Loja. O parâmetro 'clicado' indica se o utilizador tentou iniciar um processo de compra
        public IActionResult Index(String clicado)
        {
            // Variáveis locais para armazenar temporariamente os preços convertidos em texto
            string precoCamisolaCasa = "";
            string precoCamisolaFora = "";
            string precoCachecol = "";

            // 1. Valida se a intenção de compra foi disparada e se o utilizador já possui uma sessão ativa
            if (!clicado.IsNullOrEmpty() && User.Identity.IsAuthenticated)
            {
                // Redireciona para o workflow de finalização de encomenda
                return Redirect("~/EfetuarCompraProduto/Index");
            }
            // 2. Caso a intenção de compra exista mas o utilizador seja anónimo, redireciona para autenticação
            else if (!clicado.IsNullOrEmpty() && !User.Identity.IsAuthenticated)
            {
                // Encaminha para a área de Login do sistema Identity
                return Redirect("~/Identity/Account/Login");
            }
            // 3. Fluxo padrão: Carregamento inicial da montra de produtos com consulta à base de dados
            else
            {
                // Recupera a coleção de produtos relativos à camisola principal (Casa)
                var camisolaCasa = db.Produtos.Where(c => c.Nome == "Camisola Casa SCVR").ToList();
                foreach (var c in camisolaCasa)
                {
                    // Extrai o valor do preço para a variável de exibição
                    precoCamisolaCasa = c.Preco.ToString();
                }

                // Recupera a coleção de produtos relativos à camisola secundária (Fora)
                var camisolaFora = db.Produtos.Where(f => f.Nome == "Camisola Fora SCVR").ToList();
                foreach (var f in camisolaFora)
                {
                    precoCamisolaFora = f.Preco.ToString();
                }

                // Recupera a coleção de produtos relativos ao cachecol oficial
                var cachecol = db.Produtos.Where(c => c.Nome == "Cachecol Oficial").ToList();
                foreach (var f in cachecol)
                {
                    precoCachecol = f.Preco.ToString();
                }

                // Disponibiliza os valores à View via ViewBag para renderização dinâmica no front-end
                ViewBag.precoCamisolaCasa = precoCamisolaCasa;
                ViewBag.precoCamisolaFora = precoCamisolaFora;
                ViewBag.precoCachecol = precoCachecol;

                // Devolve o resultado da ação através da renderização da View da loja
                return View();
            }
        }
    }
}