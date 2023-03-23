using NokNok_ShoppingAPI.Models;

namespace NokNok_ShoppingAPI.DAO
{
    public class CustomersDAO
    {
        public static List<Customer> GetCustomers()
        {
            var list = new List<Customer>();
            try
            {
                using (var context = new NokNok_ShoppingContext())
                {
                    list = context.Customers.ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return list;
        }
        public static Customer GetCustomerById(string id)
        {
            Customer p = new Customer();
            try
            {
                using (var context = new NokNok_ShoppingContext())
                {
                    p = context.Customers.SingleOrDefault(x => x.CustomerId == id);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return p;
        }
        public static void CreateCustomer(Customer p)
        {
            try
            {
                using (var context = new NokNok_ShoppingContext())
                {
                    context.Customers.Add(p);
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public static void UpdateCustomer(Customer p)
        {
            try
            {
                using (var context = new NokNok_ShoppingContext())
                {
                    context.Entry<Customer>(p).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public static void DeleteCustomer(Customer p)
        {
            try
            {
                using (var context = new NokNok_ShoppingContext())
                {
                    var p1 = context.Customers.SingleOrDefault(x => x.CustomerId == p.CustomerId);
                    context.Customers.Remove(p1);
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
