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




        [HttpGet("get-likes")]
        public async Task<IEnumerable<UserLikeDTO>> GetAllLikes([FromQuery] int id,  string predicate)
        {

            var likes = await _LikeRepository.GetUsersLikes(predicate, id);

            return likes;

        }

        [HttpPost("add-like")]
        public async Task<ActionResult>AddLike(string username)
        {

            AppUser sourceUser = await _userRepo.GetByNameAsync(User.GetUsername());
            AppUser targetUser = await _userRepo.GetByNameAsync(username);


            if (username == sourceUser.UserName) return BadRequest("you cannot like yourself.");

            var like = new UserLike
            {
                SourceUser = sourceUser,
                SourceUserId = sourceUser.Id,
                TargetUser = targetUser,
                TargetUserId = targetUser.Id
            };


            _LikeRepository.AddLike(like);

            if(await _LikeRepository.SaveAllAsync())
            {
                return Ok("User Liked");
            }


            return BadRequest("something went wrong");


        }
        







    }
}
