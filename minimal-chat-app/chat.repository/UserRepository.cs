using Chat.DominModel.Context;
using Chat.DominModel.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ChatDbContext _context; // Replace YourDbContext with your actual DbContext class

        public UserRepository(ChatDbContext context)
        {
            _context = context;
        }

        public async Task<User>  Get(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<IEnumerable<User>>  GetAll()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task Insert(User user)
        {
            await _context.Users.AddAsync(user);
            _context.SaveChanges();
        }

       

        public async Task Delete(int id)
        {
            var user =await _context.Users.FindAsync(id);
            if (user != null)
            {
                  _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }

        public async Task<User> GetByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
    }

}
