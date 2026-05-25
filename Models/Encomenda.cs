using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace _01.Carlos.Fontes.Models
{
    public class Encomenda
    {
        public int Id { get; set; }
        
        public DateTime DataEncomenda   { get; set; }
        public string? MoradaEnvio { get; set; }
        public float Total {  get; set; }
        public string? EstadoPagamento { get; set; }
        public bool Ativa { get; set; }
        public string? AspNetUserId { get; set; }

    }
}
