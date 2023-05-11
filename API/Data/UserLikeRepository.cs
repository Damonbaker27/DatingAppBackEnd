using API.DTO;
using API.Entities;
using API.Extensions;
using API.Interface;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserLikeRepository : ILikesRepository
    {
        private DataContext _context;

        public UserLikeRepository(DataContext dataContext)
        {
            _context = dataContext;
            
        }

        /// <summary>
        /// Returns a specific single like entity
        /// </summary>
        /// <param name="sourceUserId"></param>
        /// <param name="likedUserId"></param>
        /// <returns></returns>
        public async Task<UserLike> GetUserLike(int sourceUserId, int likedUserId)
        {
            var userLikes = await _context.likes.FindAsync(sourceUserId, likedUserId);

            return userLikes;
        }


        /// <summary>
        /// gets either the likes made by the user or the likes targeting that user based on the predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<UserLikeDTO>> GetUsersLikes(string predicate, int userId)
        {

            //builds up a query in memory but does not execute it yet.
            var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();

            var likes = _context.likes.AsQueryable();

            if(predicate == "liked")
            {
                //get all likes the user has had made on other users
                likes = likes.Where(like => like.SourceUserId == userId);
                users = likes.Select(like => like.TargetUser);
            }

            if (predicate == "likedBy")
            {
                //gets all likes the user has received.
                likes = likes.Where(like => like.TargetUserId == userId);
                users = likes.Select(like => like.SourceUser);
            }

            // Return a newly created like DTO.
            return await users.Select(user => new UserLikeDTO
            {
                UserName = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain).Url,
                City = user.City,
                Id = user.Id
            }).ToListAsync();


        }

        // returns the user and will include all of the likes made by that user.
        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await _context.Users
                .Include(x => x.LikedUsers)
                .FirstOrDefaultAsync(x => x.Id == userId);
        }


        public void AddLike(UserLike like) 
        {
            _context.likes.Add(like);
        }

        public async Task<bool> SaveAllAsync()
        {     
            return await _context.SaveChangesAsync() > 0;
        }

        public void RemoveLike(UserLike like)
        {
            _context.Entry(like).State= EntityState.Deleted;
        }

 
    }
}
