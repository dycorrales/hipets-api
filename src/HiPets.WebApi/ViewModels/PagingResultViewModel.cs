using System.Collections.Generic;

namespace HiPets.WebApi.ViewModels
{
    public class PagingResultViewModel<T>
    {
        private readonly int _pageSize;

        public IEnumerable<T> Elements { get; set; }
        public int ElementsCount { get; set; }
        public int CurrentPage { get; }
        public int TotalPages => ElementsCount == 0 ? 0 : ((ElementsCount - 1) / _pageSize) + 1;

        public PagingResultViewModel(int page = 0, int pageSize = 0)
        {
            Elements = new List<T>();
            CurrentPage = page;
            _pageSize = pageSize;
        }
    }
}
