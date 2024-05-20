using EasyArchitect.Infrastructure.Cache;
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
    public class LoginModel: PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly IRedisCacheProvider _redisCacheProvider;

        public LoginModel(IHttpContextAccessor httpContextAccessor, 
            IConfiguration configuration, 
            IRedisCacheProvider redisCacheProvider)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _redisCacheProvider = redisCacheProvider;
        }

        [BindProperty]
        public Account? AccountData { get; set; } = new Account();

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

        /// <summary>
        /// 處理登入與產生 Token 作業（當外部使用者存在時）
        /// </summary>
        /// <param name="account"></param>
        protected virtual bool ProcessLogin(Account? account)
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

                NewCookie gqsCookie = new NewCookie(Account.LOGIN_USER_INFO);
                gqsCookie.Values.Add("Username", account.Username);

                string jsonString = NewCookie.GetJsonByNewCookie(gqsCookie);

                _httpContextAccessor.HttpContext.Response.Cookies.Append(Account.LOGIN_USER_INFO, jsonString, cookieOptions);

                _redisCacheProvider.Put($"{Account.LOGIN_USER_INFO}_{account.Username}", new Account() { Username = account.Username }, new TimeSpan(0, timeoutExpires, 0));
            }
            catch (Exception ex)
            {
                result = false;
            }

            return result;
        }
    }
}
