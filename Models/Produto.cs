namespace _01.Carlos.Fontes.Models
{
    public class Produto
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public string? Descricao { get; set; }
        public string? Tipo { get; set; }
        public string? Tamanho { get; set; }

        public decimal Preco { get; set; }
        public int Stock { get; set; }
       
    }
}
