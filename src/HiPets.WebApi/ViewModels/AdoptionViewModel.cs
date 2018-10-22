using HiPets.Domain.Helpers;
using System;
using System.ComponentModel.DataAnnotations;

namespace HiPets.WebApi.ViewModels
{
    public class AdoptionViewModel
    {
        [Required(ErrorMessage = "O animal é requerido")]
        public Guid AnimalId { get; set; }
        [Required(ErrorMessage = "O cliente é requerido")]
        public Guid AdopterId { get; set; }
    }
}
