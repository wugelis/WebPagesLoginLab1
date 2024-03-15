using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesCar.Application.Customers
{
    /// <summary>
    /// 
    /// </summary>
    public class CustomerServices
    {
        private static List<Customer> _customerList = new List<Customer>();
        public CustomerServices()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public int AddCustomer(Customer customer)
        {
            _customerList.Add(customer);
            return customer.Id;
        }
    }
}
