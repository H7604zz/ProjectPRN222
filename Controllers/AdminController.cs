using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectPrn222.Service.Iterface;

namespace ProjectPrn222.Controllers
{
	//[Authorize(Roles = "Admin")]
	public class AdminController : Controller
	{
		private readonly IUserService _userService;
		public AdminController(IUserService userService)
		{
			_userService = userService;
		}

		public ActionResult ManageUser()
		{
			var listUsers = _userService.GetAllUsers()
                .Where(u => u.RoleName != "Admin").ToList();
			return View(listUsers);
		}
	}
}
