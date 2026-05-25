using _01.Carlos.Fontes.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace _01.Carlos.Fontes.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Bilhete> Bilhetes { get; set; }
        public DbSet<Encomenda> Encomendas { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Classificacao> Classificacaos { get; set; }
        public DbSet<Socio> Socio { get; set; }
        public DbSet<Jogo> Jogo { get; set; }
        public DbSet<Encomenda_Produto> Encomenda_Produto { get; set; }



    }
}
