using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SalesCar.Application;

namespace WebPagesLoginLab1.Pages
{
    public class LogoutModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LogoutModel(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void OnGet()
        {
        }

        public void OnPost()
        {
            LogoutProcess();
        }

        /// <summary>
        /// 進行登出作業
        /// </summary>
        /// <returns></returns>
        //[AllowAnonymous]
        public void LogoutProcess()
        {
            _httpContextAccessor.HttpContext.SignOutAsync().GetAwaiter();

            _httpContextAccessor.HttpContext.Response.Cookies.Delete(Account.LOGIN_USER_INFO);

            Response.Redirect("/Login");
        }
    }
}
