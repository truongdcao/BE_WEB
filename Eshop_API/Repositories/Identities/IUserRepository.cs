using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eshop_api.Entities;
using Eshop_API.Repositories.Generics;

namespace Eshop_API.Repositories.Identities
{
    public interface IUserRepository: IGenericRepository<User>
    {
        
    }
}