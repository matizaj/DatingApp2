using DatingApp.API.Data;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.Services
{
    public class Repository : IRepository
    {
        private readonly DataContext _context;

        public Repository(DataContext context) => _context = context;

        public IEnumerable<Value> Values => _context.Values.ToList();

        Task<IEnumerable<Value>> IRepository.Values
        {
            get
            {
                return Task.FromResult<IEnumerable<Value>>(_context.Values);
            }
        }
    }
}
