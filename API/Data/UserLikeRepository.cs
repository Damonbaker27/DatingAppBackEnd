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
        private Mapper _mapper;

        public UserLikeRepository(DataContext dataContext, Mapper mapper)
        {
            _context = dataContext;
            _mapper = mapper;
        }

        /// <summary>
        /// gets The userLike entity that matches the passed in primary key values.
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
        /// get either the users liked or liked by users
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

            // Return a newly created DTO.
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

        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await _context.Users
                .Include(x => x.LikedUsers)
                .FirstOrDefaultAsync(x => x.Id == userId);
        }
    }
}
