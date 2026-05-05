using App.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.Data.Mappings;

public class SiteOrigemConfigMapping : IEntityTypeConfiguration<SiteOrigemConfig>
{
    public void Configure(EntityTypeBuilder<SiteOrigemConfig> builder)
    {
        builder.ToTable("site_origem_config");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .HasColumnName("id")
            .HasColumnType("TEXT")
            .ValueGeneratedNever();

        builder.Property(o => o.SiteDeployConfigId)
            .IsRequired()
            .HasColumnName("site_deploy_config_id")
            .HasColumnType("TEXT");

        builder.Property(o => o.Path)
            .IsRequired()
            .HasColumnName("path")
            .HasColumnType("TEXT")
            .HasMaxLength(500);

        builder.Property(o => o.Conteudo)
            .IsRequired()
            .HasColumnName("conteudo")
            .HasColumnType("INTEGER");

        builder.HasOne(o => o.SiteDeployConfig)
            .WithMany(s => s.Origens)
            .HasForeignKey(o => o.SiteDeployConfigId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
