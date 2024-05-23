using EasyArchitect.OutsideManaged.AuthExtensions.Models;
using EasyArchitectCore.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace EasyArchitect.PageModel.AuthExtensions
{
    public class PageModelBase: Microsoft.AspNetCore.Mvc.RazorPages.PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PageModelBase(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }
        /// <summary>
        /// 處理登入與產生 Token 作業（當外部使用者存在時）
        /// </summary>
        /// <param name="account"></param>
        protected virtual bool ProcessLogin(AuthenticateRequest? account)
        {
            bool result = true;

            ClaimsPrincipal principal = new ClaimsPrincipal(
            new ClaimsIdentity(
                new[]
                {
                        new Claim(ClaimTypes.Name, account.Username),
                        new Claim(ClaimTypes.Role, "Admin")
                },
                CookieAuthenticationDefaults.AuthenticationScheme
            ));

            try
            {
                _httpContextAccessor.HttpContext.SignInAsync(principal);

                int timeoutExpires = _configuration.GetSection("AppSettings").GetValue<int>("TimeoutMinutes");
                CookieOptions cookieOptions = new CookieOptions()
                {
                    Expires = DateTime.Now.AddMinutes(timeoutExpires),
                    HttpOnly = true
                };

                NewCookie gqsCookie = new NewCookie(UserInfo.LOGIN_USER_INFO);
                gqsCookie.Values.Add("Username", account.Username);

                string jsonString = NewCookie.GetJsonByNewCookie(gqsCookie);

                _httpContextAccessor.HttpContext.Response.Cookies.Append(UserInfo.LOGIN_USER_INFO, jsonString, cookieOptions);

                //_redisCacheProvider.Put($"{Account.LOGIN_USER_INFO}_{account.Username}", new Account() { Username = account.Username }, new TimeSpan(0, timeoutExpires, 0));
            }
            catch (Exception ex)
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// 進行登出作業
        /// </summary>
        /// <returns></returns>
        //[AllowAnonymous]
        public void LogoutProcess()
        {
            _httpContextAccessor.HttpContext.SignOutAsync().GetAwaiter();

            _httpContextAccessor.HttpContext.Response.Cookies.Delete(UserInfo.LOGIN_USER_INFO);

            Response.Redirect("/Login");
        }
    }
}
