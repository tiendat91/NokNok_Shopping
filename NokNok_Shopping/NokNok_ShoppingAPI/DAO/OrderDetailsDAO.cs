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
                    p.ProductId = 1;//Fix cứng sản phẩm đầu tiên
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
