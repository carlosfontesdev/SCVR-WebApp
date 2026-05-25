using _01.Carlos.Fontes.Data;
using _01.Carlos.Fontes.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace _01.Carlos.Fontes.Controllers
{
    // Restringe o acesso ao controller exclusivamente a utilizadores autenticados no sistema
    [Authorize]
    public class GestaoJogoBilhete : Controller
    {
        private readonly ApplicationDbContext db;

        // Construtor que procede à injeção da instância do contexto da base de dados
        public GestaoJogoBilhete(ApplicationDbContext context)
        {
            db = context;
        }

        // Método de ação principal para listagem e criação de novos eventos desportivos (jogos)
        public IActionResult Index(String adversario, String dataJogo, String lotacaoMaxima, String quantidadeBilhetes, String precoBilhete, String clicado)
        {
            // Recupera o identificador único do utilizador através das Claims do contexto de segurança
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Procura na tabela de utilizadores o registo correspondente ao ID obtido
            var user = db.Users.FirstOrDefault(m => m.Id == userId);

            // Valida a existência do utilizador e verifica se o mesmo detém privilégios administrativos
            if (user == null || user.IsAdmin == false)
            {
                // Redireciona utilizadores não autorizados para a página inicial
                return Redirect("~/Home/Index");
            }

            // Carrega a lista de jogos ordenados por data e a lista de adversários (classificação) para a interface
            ViewBag.Jogo = db.Jogo.OrderBy(m => m.DataJogo).ToList();
            ViewBag.Adversario = db.Classificacaos.ToList();

            // Verifica se o formulário de submissão para um novo jogo foi acionado
            if (!clicado.IsNullOrEmpty())
            {
                if(!adversario.IsNullOrEmpty())
                {
                    // Instancia e preenche um novo objeto da entidade Jogo com os dados convertidos do formulário
                    Jogo novoJogo = new Jogo();
                    novoJogo.Adversario = adversario;
                    novoJogo.BilhetesDisponiveis = Convert.ToInt16(quantidadeBilhetes);
                    novoJogo.DataJogo = Convert.ToDateTime(dataJogo);
                    novoJogo.LotacaoMaxima = Convert.ToInt16(lotacaoMaxima);
                    novoJogo.PrecoBase = Convert.ToInt16(precoBilhete);

                    // Adiciona o novo registo ao contexto e persiste as alterações na base de dados
                    db.Jogo.Add(novoJogo);
                    db.SaveChanges();

                    // Notifica o utilizador sobre o sucesso da operação através de TempData
                    TempData["Sucesso"] = "Jogo Criado com Sucesso";
                }
                else
                {
                    TempData["Erro"] = "Têm que selecionar um adversário";
                } 
            }

            return View();
        }

        // Método de ação responsável pela atualização de dados de um jogo existente
        public IActionResult Editar(String id, String clicado, String dataJogo, String lotacaoMaxima, String quantidadeBilhetes, String precoBilhete)
        {
            // Procura o jogo específico na base de dados com base no identificador fornecido
            var jogo = db.Jogo.Where(m => m.Id == Convert.ToInt16(id)).ToList();
            ViewBag.Jogo = jogo;

            // Verifica se o gatilho de atualização (clicado) foi enviado pelo formulário
            if (!clicado.IsNullOrEmpty())
            {
                // Itera sobre a coleção de resultados (normalmente um único registo) para atualizar as propriedades
                foreach (var item in jogo)
                {
                    item.BilhetesDisponiveis = Convert.ToInt16(quantidadeBilhetes);
                    item.DataJogo = Convert.ToDateTime(dataJogo);
                    item.LotacaoMaxima = Convert.ToInt16(lotacaoMaxima);
                    item.PrecoBase = Convert.ToInt16(precoBilhete);
                }

                // Persiste as modificações efetuadas nos objetos na base de dados relacional
                db.SaveChanges();

                // Armazena mensagem de feedback positivo para o utilizador
                TempData["Sucesso"] = "Jogo Editado com Sucesso";
            }

            return View();
        }
    }
}