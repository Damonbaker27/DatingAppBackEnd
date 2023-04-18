using API.Extensions;
using API.Interface;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Helper
{
    public class LogUserActivity : IAsyncActionFilter
    {
        private readonly IUserRepository _repository;

        public LogUserActivity(IUserRepository repository)
        {
            _repository = repository;
        }



        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //get the context AFTER the api action has completed.
            var resultContext = await next();
           
            if(!resultContext.HttpContext.User.Identity.IsAuthenticated) return;

            var userId = resultContext.HttpContext.User.GetId();
            var username = resultContext.HttpContext.User.GetUsername();
                 

            if (username != null)
            {   

                var user = await _repository.GetByNameAsync(username);

                user.LastActive = DateTime.UtcNow;

                await _repository.SaveAllAsync();          

            }


        }
    }
}
