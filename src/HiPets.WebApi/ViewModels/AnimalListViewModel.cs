namespace HiPets.WebApi.ViewModels
{
    public class AnimalListViewModel : BaseViewModel
    {
        public string Name { get; set; }
        public string AnimalType { get; set; }
        public string Breed { get; set; }
        public string Behavior { get; set; }
        public int Age { get; set; }
        public string AdopterName { get; set; }
        public string PictureUrl { get; set; }
    }
}
