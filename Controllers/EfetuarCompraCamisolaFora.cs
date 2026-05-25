using _01.Carlos.Fontes.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using _01.Carlos.Fontes.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;

namespace _01.Carlos.Fontes.Controllers
{
    // Restringe o acesso ao controller exclusivamente a utilizadores autenticados no sistema
    [Authorize]
    public class EfetuarCompraCamisolaFora : Controller
    {
        private readonly ApplicationDbContext db;

        // Construtor que procede à injeção da instância do contexto da base de dados
        public EfetuarCompraCamisolaFora(ApplicationDbContext context)
        {
            db = context;
        }

        // Método de ação que processa a visualização principal e a persistência da aquisição de artigos de vestuário
        public IActionResult Index(String tamanho, String MoradaEnvio, String preco, String clicado)
        {
            // Avalia se o gatilho de submissão foi acionado através da verificação da string de controlo
            if (clicado.IsNullOrEmpty())
            {
                // Devolve o resultado da ação através da renderização da View associada
                return View();
            }
            else
            {
                // Recupera o identificador único do utilizador através das Claims do contexto de segurança
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Procura na tabela de produtos o registo correspondente à denominação específica do artigo e à variante de dimensão selecionada
                var IdCamisola = db.Produtos.FirstOrDefault(m => m.Tamanho == tamanho && m.Nome == "Camisola Fora SCVR");

                // Instancia novos objetos para as entidades de encomenda e relação produto-encomenda
                Encomenda encomenda = new Encomenda();
                Encomenda_Produto encomendaProduto = new Encomenda_Produto();

                // Valida a disponibilidade de inventário para o produto selecionado
                if (IdCamisola.Stock != 0)
                {
                    // Verifica a validade do parâmetro de entrada relativo à localização para expedição
                    if (MoradaEnvio.IsNullOrEmpty())
                    {
                        // Armazena uma mensagem de advertência em TempData caso a morada não tenha sido facultada
                        TempData["ErroEncomendar"] = $"Necessário preencher morada de envio";
                    }
                    else
                    {
                        // Executa a operação aritmética de decremento sobre a propriedade Stock do objeto
                        IdCamisola.Stock = IdCamisola.Stock - 1;

                        // Procede ao preenchimento dos atributos do objeto encomenda com os dados da transação
                        encomenda.MoradaEnvio = MoradaEnvio;
                        encomenda.EstadoPagamento = "Pendente";
                        encomenda.DataEncomenda = DateTime.Now;
                        encomenda.AspNetUserId = userId;
                        encomenda.Ativa = true;
                        encomenda.Total = Convert.ToInt16(IdCamisola.Preco);

                        // Adiciona a nova instância de encomenda ao contexto de dados
                        db.Encomendas.Add(encomenda);

                        // Persiste as alterações efetuadas no estado dos objetos na base de dados relacional
                        db.SaveChanges();

                        // Estabelece a ligação entre a encomenda gerada e o produto adquirido na tabela de relação
                        encomendaProduto.EncomendaId = encomenda.Id;
                        encomendaProduto.ProdutoId = IdCamisola.Id;
                        encomendaProduto.PrecoVenda = Convert.ToInt16(IdCamisola.Preco);
                        encomendaProduto.Quantidade = 1;

                        // Adiciona o registo de detalhe da encomenda ao contexto de persistência
                        db.Encomenda_Produto.Add(encomendaProduto);

                        // Persiste as alterações efetuadas no estado dos objetos na base de dados relacional
                        db.SaveChanges();

                        // Armazena uma mensagem de confirmação em TempData para ser consumida pela View no próximo ciclo de renderização
                        TempData["Sucesso"] = "Compra Efetuada com Sucesso";
                    }
                }
                else
                {
                    // Armazena uma notificação de rutura de stock caso o produto não esteja disponível para venda
                    TempData["Erro"] = $"Tamanho esgotado.";
                }

                // Devolve o resultado da ação através da renderização da View associada
                return View();
            }
        }
    }
}