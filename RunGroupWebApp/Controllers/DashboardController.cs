using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using RunGroopWebApp;
using RunGroopWebApp.Models;
using RunGroupWebApp.Data;
using RunGroupWebApp.Interfaces;
using RunGroupWebApp.ViewModels;

namespace RunGroupWebApp.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboardRepository _dashboardRepository;
        private readonly IPhotoService _photoService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DashboardController(IDashboardRepository dashboardRepository, IPhotoService photoService, IHttpContextAccessor httpContextAccessor)
        {
            _dashboardRepository = dashboardRepository;
            _photoService = photoService;
            _httpContextAccessor = httpContextAccessor;
        }

        private void MapUserEdit(AppUser user,EditUserDashboardViewModel editVm,ImageUploadResult photoResult)
        {
            user.Id = editVm.Id;
            user.Pace = editVm.Pace;
            user.Mileage = editVm.Mileage;
            user.ProfileImageUrl = photoResult.Url.ToString();
            user.City = editVm.City;
            user.State = editVm.State;
        }

        public async Task<IActionResult> Index()
        {
            var userRaces = await _dashboardRepository.GetAllUserRaces();
            var userClubs = await _dashboardRepository.GetAllUserClubs();

            var dashboardViewModel = new DashboardViewModel()
            {
                Races = userRaces,
                Clubs = userClubs,
            };

            return View(dashboardViewModel);
        }

        public async Task<IActionResult> EditUserProfile()
        {
            var currentUserId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _dashboardRepository.GetUserById(currentUserId); 

            if(user == null)
            {
                return View("Error");
            }

            var editUserDashboardViewModel = new EditUserDashboardViewModel()
            {
                Id = currentUserId,
                Pace = user.Pace,
                Mileage = user.Mileage,
                ProfileImageUrl = user.ProfileImageUrl,
                City = user.City,
                State = user.State,
            };

            return View(editUserDashboardViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditUserProfile(EditUserDashboardViewModel editVm)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit profile");
                return View("EditUserProfile",editVm);
            }

            var user = await _dashboardRepository.GetByIdNoTracking(editVm.Id);

            if(user == null)
            {
                return View("Error");
            }

            if(editVm.Image != null)
            {
                var photoResult = await _photoService.AddPhotoAsync(editVm.Image);

                MapUserEdit(user,editVm, photoResult);

                _dashboardRepository.Update(user);

                return RedirectToAction("Index");   
            }
            else
            {
                try
                {
                    await _photoService.DeletePhotoAsync(user.ProfileImageUrl);
                }catch(Exception ex)
                {
                    ModelState.AddModelError("", "Could not delete photo");
                    return View(editVm);
                }

                var photoResult = await _photoService.AddPhotoAsync(editVm.Image);

                MapUserEdit(user, editVm, photoResult);

                _dashboardRepository.Update(user);

                return RedirectToAction("Index");
            }

            return View(editVm);
        }

    }
}
