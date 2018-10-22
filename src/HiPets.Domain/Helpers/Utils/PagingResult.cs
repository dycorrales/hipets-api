using System.Collections.Generic;

namespace HiPets.Domain.Helpers.Utils
{
    public class PagingResult<T>
    {
        public IEnumerable<T> Elements { get; set; }
        public int ElementsCount { get; set; }

        public PagingResult()
        {
            Elements = new List<T>();
            ElementsCount = 0;
        }
    }
}
