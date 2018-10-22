using HiPets.Domain.Helpers;
using System.ComponentModel.DataAnnotations;

namespace HiPets.WebApi.ViewModels
{
    public class AnimalViewModel
    {
        [
            Required(ErrorMessage = "O nome do animal é requerido"), 
            StringLength(250, ErrorMessage = "O nome pode ter até 250 caracteres")
        ]
        public string Name { get; set; }

        [
            Required(ErrorMessage = "A raça do animal é requerida"), 
            StringLength(50, ErrorMessage = "A raça pode ter até 50 caracteres")
        ]
        public string Breed { get; set; }

        [
            Required(ErrorMessage = "A idade do animal é requerida"),
            Range(1, 10, ErrorMessage = "A idade deve estar entre 1 e 10 anos")
        ]
        public int Age { get; set; }

        [Required(ErrorMessage = "A cor predominante do animal é requerida")]
        public Color PrevalentColor { get; set; }

        [
            Required(ErrorMessage = "O comportamento do animal é requerido"),
            StringLength(250, ErrorMessage = "O comportamento pode ter até 250 caracteres")
        ]
        public string Behavior { get; set; }

        [Required(ErrorMessage = "O tipo de animal é requerido")]
        public AnimalType AnimalType { get; set; }

        [StringLength(500, ErrorMessage = "A url da imagem pode ter até 500 caracteres")]
        public string PictureUrl { get; set; }
    }
}
