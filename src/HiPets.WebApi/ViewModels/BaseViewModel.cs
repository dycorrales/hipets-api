using HiPets.Domain.Helpers;
using System;

namespace HiPets.WebApi.ViewModels
{
    public abstract class BaseViewModel
    {
        public Guid Id { get; set; }
        public Status Status { get; set; }
    }
}
