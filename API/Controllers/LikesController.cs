using API.DTO;
using API.Entities;
using API.Extensions;
using API.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikesController : BaseApiController
    {
        private ILikesRepository _LikeRepository;
        private IUserRepository _userRepo;

        public LikesController( ILikesRepository repository, IUserRepository userRepository)
        {
            _LikeRepository = repository;
            _userRepo = userRepository;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserLikeDTO>>> GetAllLikes(string predicate)
        {

            var users = await _LikeRepository.GetUsersLikes(predicate, User.GetId());

            return Ok(users);

        }

        [HttpPost("{username}")]
        public async Task<ActionResult>AddLike(string username)
        {

            AppUser sourceUser = await _LikeRepository.GetUserWithLikes(User.GetId());
            AppUser targetUser = await _userRepo.GetByNameAsync(username);
            

            if(targetUser == null) return NotFound(); 

            if (username == sourceUser.UserName) return BadRequest("you cannot like yourself.");

            var userLike = await _LikeRepository.GetUserLike(sourceUser.Id, targetUser.Id);

            if (userLike != null) return BadRequest("user has been liked already");

            
            userLike = new UserLike
            {             
                SourceUserId = sourceUser.Id,              
                TargetUserId = targetUser.Id
            };

            sourceUser.LikedUsers.Add(userLike);

            _LikeRepository.AddLike(userLike);

            if(await _LikeRepository.SaveAllAsync()) return Ok("User Liked");   


            return BadRequest("something went wrong");


        }
        







    }
}
