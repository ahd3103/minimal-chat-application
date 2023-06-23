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
        private List<User> _users;

        public UserRepository(ChatDbContext context, List<User> users)
        {
            _context = context;
            _users = users;
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

        public async Task<User> GetByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> CheckUser(string email, string password)
        {
            // Find the user with the provided email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email); 

            return user;  
        }

        // Return all users as an asynchronous operation
        public Task<IEnumerable<User>> GetAllUsers()
        {
            return Task.FromResult<IEnumerable<User>>(_users);
        }

        public Task<User> GetUserById(int userId)
        {
            // Find the user by their ID (replace with your actual data access code)
            var user = _users.FirstOrDefault(u => u.UserId == userId);

            // Return the user as an asynchronous operation
            return Task.FromResult(user);
        }
       
    }

}
