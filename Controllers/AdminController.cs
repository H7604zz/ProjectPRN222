using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectPrn222.Service.Implement;
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

		public ActionResult ManageUser(string? keyword)
		{
            var listUserQuery = !string.IsNullOrEmpty(keyword)
                ? _userService.SearchUser(keyword)
                : _userService.GetAllUsers()
				.ToList().Where(u => u.RoleName != "Admin"); 

            ViewBag.keyword = keyword;

            return View(listUserQuery);
		}
	}
}
