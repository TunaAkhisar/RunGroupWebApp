using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RunGroopWebApp.Models;
using RunGroupWebApp.Interfaces;
using RunGroupWebApp.ViewModels;

namespace RunGroupWebApp.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly IPhotoService _photoService;

        public UserController(IUserRepository userRepository, UserManager<AppUser> userManager, IPhotoService photoService)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _photoService = photoService;
        }

        [HttpGet("users")]
        public async Task<IActionResult> Index()
        {
            var users = await _userRepository.GetAllUsers();
            List<UserViewModel> result = new List<UserViewModel>();
            foreach (var user in users)
            {
                var userViewModel = new UserViewModel()
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Pace = user.Pace,
                    Mileage = user.Mileage,
                };
                result.Add(userViewModel);
            }
            return View(result);
        }

        public async Task<IActionResult> Detail(string id)
        {
            var user = await _userRepository.GetUserById(id);
            var userDetailViewModel = new UserDetailViewModel()
            {
                Id = user.Id,
                UserName = user.UserName,
                Pace = user.Pace,
                Mileage = user.Mileage,
            };
            return View(userDetailViewModel);
        }

    }
}
