
namespace HiPets.WebApi.Filters
{
    public class PagingFilter
    {
        private int? _page;
        private int? _pageSize;

        public int? Page
        {
            get
            {
                if (_page is null || _page == 0)
                    return 1;
                return _page;
            }
            set => _page = value;
        }

        public int? PageSize
        {
            get
            {
                if (_pageSize is null || _pageSize == 0)
                    return 10;
                return _pageSize;
            }
            set => _pageSize = value;
        }
    }
}
