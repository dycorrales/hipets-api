using FluentValidation.Results;
using HiPets.Domain.Entities;
using HiPets.Infra.Data.Extensions;
using HiPets.Infra.Data.Mappings;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace HiPets.Infra.Data.Contexts
{
    public class Context : DbContext
    {
        public DbSet<Animal> Animals { get; set; }
        public DbSet<Adopter> Adopters { get; set; }
        public DbSet<Adoption> Adoptions { get; set; }

        public Context(DbContextOptions<Context> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            modelBuilder.Ignore<ValidationFailure>();

            modelBuilder.AddConfiguration(new AnimalMap());
            modelBuilder.AddConfiguration(new AdopterMap());
            modelBuilder.AddConfiguration(new AdoptionMap());
        }
    }
}
