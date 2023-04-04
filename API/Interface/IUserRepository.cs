using API.DTO;
using API.Entities;
using API.Helper;

namespace API.Interface
{
    public interface IUserRepository
    {

        void Update(AppUser user);
        void DeleteUser(AppUser user);

        void AddUser(AppUser user);

        Task<bool> UserExist(string userName);

        Task<bool> SaveAllAsync();

        Task<IEnumerable<AppUser>> GetUsersAsync();

        Task<AppUser> GetByIdAsync(int id);

        Task<AppUser> GetByNameAsync(string username);

        Task<PagedList<MemberDTO>> GetMembersAsync(UserParams userParams);

        Task<MemberDTO> GetMemberAsync(string username);




    }
}
