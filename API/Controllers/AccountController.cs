using API.Data;

namespace API.Controllers
{
    public class AccountController :BaseApiController
    {
        private readonly DataContext _context;

        //Inject datacontext class into 
        public AccountController(DataContext context)
        {
            _context = context;
        }




    }
}
