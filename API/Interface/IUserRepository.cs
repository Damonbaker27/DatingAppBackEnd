using API.DTO;
using API.Entities;

namespace API.Interface
{
    public interface IUserRepository
    {

        void Update(MemberDTO user);

        Task<bool> SaveAllAsync();

        Task<IEnumerable<MemberDTO>> GetUsersAsync();

        Task<MemberDTO> GetByIdAsync(int id);

        Task<MemberDTO> GetByNameAsync(string username);



    }
}
