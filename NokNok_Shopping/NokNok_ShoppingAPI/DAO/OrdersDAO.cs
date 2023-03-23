using Microsoft.EntityFrameworkCore;
using NokNok_ShoppingAPI.Models;
using PE_PRN231_Sum22B1.DTO;

namespace NokNok_ShoppingAPI.DAO
{
    public class OrdersDAO
    {
        public static List<Order> GetOrders()
        {
            var list = new List<Order>();
            try
            {
                using (var context = new NokNok_ShoppingContext())
                {
                    list = context.Orders.Include(s=>s.OrderDetails).ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return list;
        }
        public static Order GetOrderById(int id)
        {
            Order p = new Order();
            try
            {
                using (var context = new NokNok_ShoppingContext())
                {
                    p = context.Orders.Include(s => s.OrderDetails).SingleOrDefault(x => x.OrderId == id);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return p;
        }

        public static void CreateOrder(Models.Order order)
        {
            try
            {
                using (var context = new NokNok_ShoppingContext())
                {
                    context.Orders.Add(order);
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public static void UpdateOrder(Order p)
        {
            try
            {
                using (var context = new NokNok_ShoppingContext())
                {
                    context.Entry<Order>(p).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public static void DeleteOrder(Order p)
        {
            try
            {
                using (var context = new NokNok_ShoppingContext())
                {
                    //DELETE ORDER DETAIL
                    var ods = context.OrderDetails.Where(s=>s.OrderId == p.OrderId).ToList();
                    foreach (var o in ods)
                    {
                        context.OrderDetails.Remove(o);
                    }

                    //DELETE ORDER
                    var p1 = context.Orders.SingleOrDefault(x => x.OrderId == p.OrderId);
                    context.Orders.Remove(p1);
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
