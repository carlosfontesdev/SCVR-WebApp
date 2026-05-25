namespace _01.Carlos.Fontes.Models
{
    public class Bilhete
    {
        public int Id { get; set; }
        public DateTime DataCompra { get; set; }
        public double PrecoPago {  get; set; }
        public int NumeroBilhete { get; set; }
        public string? AspNetUserId { get; set; }
        public int JogoId { get; set; }
    }
}
