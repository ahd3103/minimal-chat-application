using Chat.DominModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Repository
{
    public interface IUserRepository
    {
        Task<User> Get(Guid id);
        Task<User> CheckUser(string email, string password);
        Task<IEnumerable<User>> GetAll();
        Task Insert(User user);
        Task<User> GetByEmail(string email);
    }

}
