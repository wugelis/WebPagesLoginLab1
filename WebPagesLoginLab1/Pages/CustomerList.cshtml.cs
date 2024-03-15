using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebPagesLoginLab1.Pages
{
    public class CustomerListModel : PageModel
    {
        public List<CustomerItems> Customers { get; set; }

        public class CustomerItems
        {
            public int CustomerId { get; set; }
            public string CustomerName { get; set; }
            public string CustomerEmail { get; set; }
            public string CustomerPhone { get; set; }
            public string CustomerCity { get; set; }
            public string CustomerRegion { get; set; }
        }
        

        public void OnGet()
        {
            Customers = new CustomerItems[]
            {
                new CustomerItems()
                { 
                    CustomerId = 1, 
                    CustomerName = "gelis.wu", 
                    CustomerEmail = ""
                },
                new CustomerItems()
                {
                    CustomerId = 2, 
                    CustomerName = "Allan.wu", 
                    CustomerEmail = ""
                },
                new CustomerItems()
                {
                    CustomerId = 3, 
                    CustomerName = "Mary.wu", 
                    CustomerEmail = ""
                }
            }.ToList();
        }
    }
}
