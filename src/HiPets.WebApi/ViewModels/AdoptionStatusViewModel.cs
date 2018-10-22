using HiPets.Domain.Helpers;
using System;
using System.ComponentModel.DataAnnotations;

namespace HiPets.WebApi.ViewModels
{
    public class AdoptionStatusViewModel
    {
        [Required(ErrorMessage = "O status da adoção é requerido")]
        public AdoptionStatus AdoptionStatus { get; set; }
        public string AdoptionObservation { get; set; }
    }
}
