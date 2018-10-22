namespace HiPets.WebApi.Filters
{
    public class CustomFilter : PagingFilter
    { 
        public string Search { get; set; }
        public bool? IsActive { get; set; }
    }
}
