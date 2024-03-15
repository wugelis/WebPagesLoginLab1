using Microsoft.AspNetCore.Mvc.Filters;
using SalesCar.Application;

namespace WebPagesLoginLab1.PageFilters
{
    /// <summary>
    /// PageModel 的 Authorization 過濾器
    /// 用途：用以檢查使用者是否有權限存取特定頁面
    /// </summary>
    public class CustomAuthorizationFilter : IAsyncPageFilter
    {
        public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            bool isAuthorized = CheckUserAuthorization(context);
            if (!isAuthorized)
            {
                // 如果不滿足權限的要求，可以重定向到 Index 頁面或導向錯誤頁面
                context.HttpContext.Response.Redirect("/Login");
            }

            // 如果檢查通過，繼續執行後續的頁面處理器或過濾器
            await next();
        }

        private bool CheckUserAuthorization(PageHandlerExecutingContext context)
        {
            context.HttpContext.Request.Cookies.TryGetValue(Account.LOGIN_USER_INFO, out string? cookieString);

            return cookieString != null;    
        }

        public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {
            return Task.CompletedTask;
        }
    }
}
