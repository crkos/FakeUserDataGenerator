using FakeUserDataGenerator.RequestModels;
using Microsoft.AspNetCore.Mvc;

namespace FakeUserDataGenerator.Controllers;
public class UserController : Controller
{
    private UserDataService _userDataService;

    public UserController()
    {
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    // For some reason when i sent it as Form request floats are sent as integers, so i instead sent as JSON...
    [HttpPost]
    public IActionResult Generate([FromBody] GenerateRequest request)
    {
        // At least now it works...
        _userDataService = new UserDataService(request.Locale);
        var users = _userDataService.GenerateUsers(request.Count, request.Errors, request.Seed, request.Page);
        return Json(users);
    }
}
