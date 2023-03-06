using Microsoft.EntityFrameworkCore;

namespace Loteria.API.Data.Context
{
    public class LoteriaContext : DbContext
    {
        public LoteriaContext(){}

        public LoteriaContext(DbContextOptions<LoteriaContext> options): base(options){}


        public DbSet<Loteria.API.Entidade.Loteria> Loterias { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Loteria.API.Entidade.Loteria>(entity =>
             {
                 entity.ToTable("LOTERIA");
                 entity.HasKey(p => p.Id).HasName("ID");
                 entity.Property(p => p.Id).HasColumnName("ID").ValueGeneratedOnAdd().HasMaxLength(5);
                 entity.Property(p => p.CodigoLoteria).HasColumnName("LOTERIA").HasMaxLength(30);
                 entity.Property(p => p.Concurso).HasColumnName("CONCURSO").HasMaxLength(8);
                 entity.Property(p => p.Resultado).HasColumnName("RESULTADO");
                 entity.Property(p => p.Dezena1).HasColumnName("DEZENA1").HasMaxLength(6);
                 entity.Property(p => p.Dezena2).HasColumnName("DEZENA2").HasMaxLength(6);
                 entity.Property(p => p.Dezena3).HasColumnName("DEZENA3").HasMaxLength(6);
                 entity.Property(p => p.Dezena4).HasColumnName("DEZENA4").HasMaxLength(6);
                 entity.Property(p => p.Dezena5).HasColumnName("DEZENA5").HasMaxLength(6);
                 entity.Property(p => p.Dezena6).HasColumnName("DEZENA6").HasMaxLength(6);
                 entity.Property(p => p.DataCadastro).HasColumnName("DATA_CADASTRO");
                 entity.Property(p => p.DataProximoConcurso).HasColumnName("DATA_PROXIMO_CONCURSO");

                 builder.Entity<Loteria.API.Entidade.Loteria>()
                     .HasIndex(u => new { u.Concurso, u.CodigoLoteria })
                     .IsUnique();
             });
        }
    }
}
