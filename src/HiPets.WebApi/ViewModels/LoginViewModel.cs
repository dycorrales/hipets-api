using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HiPets.WebApi.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email é requerido")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Email inválido")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Senha é requerida")]
        public string Password { get; set; }
    }
}
