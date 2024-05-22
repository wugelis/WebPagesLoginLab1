using EasyArchitect.Infrastructure.Cache;
using EasyArchitect.OutsideManaged.AuthExtensions.Models;
using EasyArchitect.PageModel.AuthExtensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SalesCar.Application;
using System.Security.Claims;
using System.Text.Json;
using WebPagesLoginLab1.Security;

namespace WebPagesLoginLab1.Pages
{
    /// <summary>
    /// 登入 View 模型
    /// </summary>
    public class LoginModel: PageModelBase
    {
        public LoginModel(IConfiguration configuration, IHttpContextAccessor httpContextAccessor) 
            : base(configuration, httpContextAccessor)
        {
        }

        [BindProperty]
        public AuthenticateRequest? AccountData { get; set; } = new AuthenticateRequest();

        public void OnGet()
        {
            AccountData.Username = "gelis.wu";
        }

        public void OnPost()
        {
            if (ModelState.IsValid)
            {
                if (AccountData.Username == "gelis.wu" && AccountData.Password == "123456")
                {
                    if(ProcessLogin(AccountData))
                    {
                        Response.Redirect("/Index");
                    }
                }
            }
        }
    }
}
