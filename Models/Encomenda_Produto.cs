namespace _01.Carlos.Fontes.Models
{
    public class Encomenda_Produto
    {
        public int Id { get; set; }
        public int Quantidade { get; set; }
        public int PrecoVenda { get; set; }
        public int EncomendaId { get; set; }
        public int ProdutoId { get; set; }
    }
}
