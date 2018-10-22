using System.ComponentModel.DataAnnotations;

namespace HiPets.WebApi.ViewModels
{
    public class AdopterViewModel
    {
        [
            Required(ErrorMessage = "O nome é requerido"), 
            StringLength(250, ErrorMessage = "O nome pode ter até 250 caracteres")
        ]
        public string Name { get; set; }

        [
            DataType(DataType.PhoneNumber, ErrorMessage = "O telefone é inválido"), 
            Required(ErrorMessage = "O telefone é requerido"),
            StringLength(11, ErrorMessage = "O telefone pode ter até 11 caracteres")
        ]
        public string PhoneNumber { get; set; }

        [
            DataType(DataType.EmailAddress, ErrorMessage = "O email é inválido"),
            Required(ErrorMessage = "O email é requerido"), 
            StringLength(150, ErrorMessage = "O email pode ter até 150 caracteres")
        ]
        public string Email { get; set; }
    }
}
