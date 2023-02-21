using API.Entities;
using API.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;

        public UserRepository(DataContext context)
        {
            _context = context;
        }


        public async Task<AppUser> GetByIdAsync(int id)
        {
           var user = await _context.Users.FindAsync(id);

           return user == null ? null : user;

        }

        public async Task<AppUser> GetByNameAsync(string username)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == username);

            return user == null ? null : user;

        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            var users = await _context.Users.ToListAsync();

            return users == null ? null : users;
        }

        public Task<bool> SaveAllAsync()
        {
            throw new NotImplementedException();
        }

        public void Update(AppUser user)
        {
            throw new NotImplementedException();
        }
    }
}
