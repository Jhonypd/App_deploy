using App.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.Data.Mappings;

public class SiteDeployConfigMapping : IEntityTypeConfiguration<SiteDeployConfig>
{
    public void Configure(EntityTypeBuilder<SiteDeployConfig> builder)
    {
        builder.ToTable("site_deploy_config");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasColumnName("id")
            .HasColumnType("TEXT")
            .ValueGeneratedNever();

        builder.Property(s => s.ProjetoIis)
            .IsRequired()
            .HasColumnName("projeto_iis")
            .HasColumnType("TEXT")
            .HasMaxLength(255);

        builder.Property(s => s.Porta)
            .IsRequired()
            .HasColumnName("porta")
            .HasColumnType("INTEGER");

        builder.Property(s => s.Svn)
            .HasColumnName("svn")
            .HasColumnType("TEXT")
            .HasMaxLength(500);

        builder.Property(s => s.Destino)
            .IsRequired()
            .HasColumnName("destino")
            .HasColumnType("TEXT")
            .HasMaxLength(500);

        builder.Property(s => s.UrlManual)
            .HasColumnName("url_manual")
            .HasColumnType("TEXT")
            .HasMaxLength(500);

        builder.Property(s => s.Atualizada)
            .IsRequired()
            .HasColumnName("atualizada")
            .HasColumnType("INTEGER");

        builder.HasMany(s => s.Origens)
            .WithOne(o => o.SiteDeployConfig)
            .HasForeignKey(o => o.SiteDeployConfigId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
