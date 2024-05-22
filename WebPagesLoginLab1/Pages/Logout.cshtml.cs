using EasyArchitect.PageModel.AuthExtensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SalesCar.Application;

namespace WebPagesLoginLab1.Pages
{
    public class LogoutModel : PageModelBase
    {
        public LogoutModel(IConfiguration configuration, IHttpContextAccessor httpContextAccessor) 
            : base(configuration, httpContextAccessor)
        {
        }

        public void OnGet()
        {
        }

        public void OnPost()
        {
            LogoutProcess();
        }
    }
}
