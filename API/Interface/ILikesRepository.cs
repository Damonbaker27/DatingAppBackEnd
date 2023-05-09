using API.DTO;
using API.Entities;

namespace API.Interface
{
    public interface ILikesRepository
    {
        Task<UserLike> GetUserLike(int sourceUserId, int likedUserId);

        Task<AppUser>GetUserWithLikes(int userId);

        Task<IEnumerable<UserLikeDTO>> GetUsersLikes(string predicate, int userId);

        public void AddLike(UserLike like);

        public void RemoveLike(UserLike like);

        public Task<bool> SaveAllAsync();


    }
}
