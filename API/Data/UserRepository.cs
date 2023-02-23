using API.DTO;
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


        public async Task<MemberDTO> GetByIdAsync(int id)
        {
           var user = await _context.Users.FindAsync(id);

           return user == null ? null : user;

        }

        public async Task<MemberDTO> GetByNameAsync(string username)
        {
            var user = await _context.Users.Include(p => p.Photos)
                .SingleOrDefaultAsync(x => x.UserName == username);

            return user == null ? null : user;

        }

        public async Task<IEnumerable<MemberDTO>> GetUsersAsync()
        {
            var users = await _context.Users.Include(x => x.Photos).ToListAsync();

            return users == null ? null : users;
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Update(MemberDTO user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }
    }
}
