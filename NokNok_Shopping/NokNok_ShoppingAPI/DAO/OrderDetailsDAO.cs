using NokNok_ShoppingAPI.Models;

namespace NokNok_ShoppingAPI.DAO
{
    public class OrderDetailsDAO
    {
        public static void CreateOrderDetail(OrderDetail p)
        {
            try
            {
                using (var context = new NokNok_ShoppingContext())
                {
                    context.OrderDetails.Add(p);
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
