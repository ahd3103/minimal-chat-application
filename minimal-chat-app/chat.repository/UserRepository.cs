using Chat.DominModel.Context;
using Chat.DominModel.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ChatDbContext _context; 

        public UserRepository(ChatDbContext context )
        {
            _context = context; 
        }

        public async Task<User>  Get(Guid id)
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

        public async Task<User> GetByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> CheckUser(string email, string password)
        {
            // Find the user with the provided email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.Password == password); 

            return user;  
        } 
    }

}
