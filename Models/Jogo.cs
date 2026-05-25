using NuGet.Versioning;

namespace _01.Carlos.Fontes.Models
{
    public class Jogo
    {
        public int Id { get; set; }
        public string? Adversario { get; set; }
        public DateTime DataJogo { get; set; }
        public double PrecoBase { get; set; }
        public int LotacaoMaxima { get; set; }
        public int BilhetesDisponiveis { get; set; }

    }
}
