using HiPets.Domain.Helpers;

namespace HiPets.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        Response Commit();
    }
}
