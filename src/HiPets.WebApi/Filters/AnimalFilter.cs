using HiPets.Domain.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HiPets.WebApi.Filters
{
    public class AnimalFilter : CustomFilter
    {
        [Required(ErrorMessage = "O status do animal é requerido")]
        public AnimalStatus AnimalStatus { get; set; }
    }
}
