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
        private readonly int ITEM_PER_PAGE = 10;
        public AdminController(IUserService userService)
		{
			_userService = userService;
		}

		public ActionResult ManageUser(string? keyword, int currentPage)
		{
            var listUserQuery = !string.IsNullOrEmpty(keyword)
                ? _userService.SearchUser(keyword)
                : _userService.GetAllUsers()
                .ToList().Where(u => u.RoleName != "Admin");

            int totalProduct = listUserQuery.Count(); 
            int totalPage = (int)Math.Ceiling((double)totalProduct / ITEM_PER_PAGE); // Tổng số trang

            // Đảm bảo currentPage trong khoảng hợp lệ
            currentPage = Math.Max(1, Math.Min(currentPage, totalPage));

            ViewBag.keyword = keyword;
            ViewBag.CurrentPage = currentPage;
            ViewBag.CountPage = totalPage;

            // Phân trang
            listUserQuery = listUserQuery
                .Skip((currentPage - 1) * ITEM_PER_PAGE)
                .Take(ITEM_PER_PAGE)
                .ToList();

            return View(listUserQuery);

		}
	}
}
