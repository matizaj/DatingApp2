using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Models;

namespace DatingApp.API.Services
{
    public interface IRepository
    {
        Task<IEnumerable<Value>> Values { get; }
    }
}