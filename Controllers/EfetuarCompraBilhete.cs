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
    public class EfetuarCompraBilhete : Controller
    {
        private readonly ApplicationDbContext db;

        // Construtor que procede à injeção da instância do contexto da base de dados
        public EfetuarCompraBilhete(ApplicationDbContext context)
        {
            db = context;
        }

        // Método de ação que processa a visualização e a persistência da aquisição de bilhetes
        public IActionResult Index(String quantidade, String clicado)
        {
            // Recupera o próximo evento desportivo agendado através de uma consulta LINQ filtrada por data
            var jogo = db.Jogo.Where(m => m.DataJogo > DateTime.Now).OrderBy(m => m.DataJogo).FirstOrDefault();
            
            // Atribui o valor pecuniário base do jogo à variável local de preço
            double precoBilhete = jogo.PrecoBase;

            // Disponibiliza o preço unitário à camada de visualização através da ViewBag
            ViewBag.preco = precoBilhete;

            // Avalia se o gatilho de submissão não foi acionado para calcular apenas o valor total projetado
            if (clicado.IsNullOrEmpty())
            {
                // Realiza o cálculo do montante total a pagar com base na quantidade solicitada e no preço unitário
                ViewBag.totalPagar = Convert.ToInt16(quantidade) * precoBilhete;

                // Devolve o resultado da ação através da renderização da View associada
                return View();
            }
            else
            {
                if( Convert.ToInt16(quantidade) <= jogo.BilhetesDisponiveis)
                {
                    // Converte o parâmetro de entrada da quantidade para o tipo de dados inteiro
                    int quantidadeBilhete = Convert.ToInt16(quantidade);

                    // Recupera o identificador único do utilizador através das Claims do contexto de segurança
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    // Contabiliza o número de bilhetes já adquiridos pelo utilizador para o evento específico na base de dados
                    var bilhetesUtilizador = db.Bilhetes.Count(b => b.AspNetUserId == userId && b.JogoId == jogo.Id);

                    // Valida a regra de negócio que limita a posse de no máximo quatro bilhetes por utilizador
                    if (bilhetesUtilizador + quantidadeBilhete > 4)
                    {
                        // Armazena uma mensagem de erro em TempData caso o limite de aquisição seja excedido
                        TempData["Erro"] = $"Limite excedido. Você já tem {bilhetesUtilizador} bilhetes e tentou comprar mais {quantidade}.";

                        // Executa o redirecionamento para o próprio método de ação para notificar o utilizador
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        // Itera sobre a quantidade de bilhetes solicitada para criar registos individuais
                        for (int i = 0; i < quantidadeBilhete; i++)
                        {
                            // Recupera a coleção de números de bilhetes existentes para determinar a próxima sequência
                            var numerobilhete = db.Bilhetes.Select(m => m.NumeroBilhete).ToList();

                            // Instancia um novo objeto da entidade Bilhete
                            Bilhete b = new Bilhete();
                            // Define a marca temporal da transação como o instante atual
                            b.DataCompra = DateTime.Now;
                            // Atribui o valor pecuniário efetivamente pago pelo bilhete
                            b.PrecoPago = precoBilhete;
                            // Incrementa o identificador sequencial do bilhete com base no volume total de registos existentes
                            b.NumeroBilhete = 1 + numerobilhete.Count;
                            // Associa o identificador do utilizador autenticado ao registo do bilhete
                            b.AspNetUserId = userId;
                            // Vincula o identificador do jogo correspondente ao bilhete
                            b.JogoId = jogo.Id;
                            // Adiciona a nova instância do bilhete ao contexto de dados
                            db.Bilhetes.Add(b);

                           
                        }
                        jogo.BilhetesDisponiveis = jogo.BilhetesDisponiveis - Convert.ToInt32(quantidade);
                        // Persiste as alterações efetuadas no estado dos objetos na base de dados relacional
                        db.SaveChanges();
                    }
               

                    // Redireciona o fluxo de execução para a interface de gestão de bilhetes pessoais após o sucesso da operação
                    return Redirect("~/Identity/Account/Manage/MeusBilhetes");
                }
                else if(jogo.BilhetesDisponiveis == 0 )
                {
                    TempData["Erro"] = $"Bilhetes Esgotados";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Erro"] = $"Só existe {jogo.BilhetesDisponiveis} bilhetes disponiveis";
                    return RedirectToAction("Index");
                }
            }
        }
    }
}