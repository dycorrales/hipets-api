using HiPets.Domain.Entities;
using HiPets.Infra.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace HiPets.Infra.Data.Mappings
{
    public abstract class BaseMap<T> : EntityTypeConfiguration<T> where T : Entity
    {
        public override void Map(EntityTypeBuilder<T> builder)
        {
            builder.HasKey(entity => entity.Id);

            builder.Property(entity => entity.Id)
                .HasColumnType("uniqueidentifier")
                .IsRequired();

            builder.Property(entity => entity.Status)
                .HasColumnType("int")
                .IsRequired();

            builder.Property(entity => entity.CreatedAt)
                .HasColumnType("datetime")
                .HasDefaultValue(DateTime.Now)
                .IsRequired();

            builder.Ignore(entity => entity.ValidationResult);
        }
    }
}
