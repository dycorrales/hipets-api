namespace HiPets.WebApi.ViewModels
{
    public class AdoptionListViewModel : BaseViewModel
    {
        public string AnimalName { get; set; }
        public string AnimalBreed { get; set; }
        public string AnimalType { get; set; }
        public string AnimalPictureUrl { get; set; }
        public string AdopterName { get; set; }
        public string AdopterPhoneNumber { get; set; }
        public string AdopterEmail { get; set; }
        public string AdoptionStatus { get; set; }
        public string AdoptionObservation { get; set; }
    }
}
