using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace HiPets.Domain.Interfaces
{
    public interface IUser
    {
        string Name { get; }
        Guid GetUserId();
        bool IsAuthenticated();
        IEnumerable<Claim> GetClaimsIdentity();
        int? GetRole();
        bool IsAdminUser { get; }
    }
}
