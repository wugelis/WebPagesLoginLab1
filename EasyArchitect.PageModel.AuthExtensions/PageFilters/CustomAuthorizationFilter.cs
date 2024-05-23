using EasyArchitect.OutsideManaged.AuthExtensions.Models;
using EasyArchitectCore.Core;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System.Security.Principal;
using System.Text.Json;

namespace EasyArchitect.PageModel.AuthExtensions.PageFilters
{
    /// <summary>
    /// PageModel 的 Authorization 過濾器
    /// 用途：用以檢查使用者是否有權限存取特定頁面
    /// </summary>
    public class CustomAuthorizationFilter : IAsyncPageFilter
    {
        private readonly IConfiguration _configuration;

        public CustomAuthorizationFilter(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        /// <summary>
        /// 回呼事件：當 PageHandler 執行時檢查身分權限
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            bool isAuthorized = CheckUserAuthorization(context);
            if (!isAuthorized)
            {
                // 如果不滿足權限的要求，可以重定向到 Index 頁面或導向錯誤頁面
                context.HttpContext.Response.Redirect(_configuration.GetValue<string>("LoginPage"));
            }

            // 如果檢查通過，繼續執行後續的頁面處理器或過濾器
            await next();
        }
        /// <summary>
        /// 檢查身分權限
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private bool CheckUserAuthorization(PageHandlerExecutingContext context)
        {
            context.HttpContext.Request.Cookies.TryGetValue(UserInfo.LOGIN_USER_INFO, out string? cookieString);
            string? username = context.HttpContext.User.Identity!.Name;
            if(username == null)
            {
                return false;
            }

            NewCookie cookie = NewCookie.GetNewCookieByString(cookieString, UserInfo.LOGIN_USER_INFO);
            AuthenticateRequest? accSate = cookie.Values["Username"] != null ? new AuthenticateRequest() { Username = cookie.Values["Username"].ToString() } : null;

            return accSate != null;    
        }

        public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {
            return Task.CompletedTask;
        }
    }
}
