using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SalesCar.Application;
using SalesCar.Application.Customers;
using WebPagesLoginLab1.Security;

namespace WebPagesLoginLab1.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IndexModel(ILogger<IndexModel> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public void OnGet()
        {
            string? cookieString = _httpContextAccessor.HttpContext.Request.Cookies[Account.LOGIN_USER_INFO];
            if (cookieString == null)
            {
                return;
            }

            NewCookie? cookieState = NewCookie.GetNewCookieByString(cookieString, Account.LOGIN_USER_INFO);
            if(cookieState != null)
            {
                // 透過 sessionState 取得 Username
                var obj = cookieState.Values.CookieValue["Username"];
            }
        }

        [BindProperty]
        public Customer? Customer { get; set; }

        public IActionResult OnPost(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                string customerName = customer.Name ?? "gelis.wu";

                return Page();
            }

            return RedirectToPage("/Index");
        }
    }
}
