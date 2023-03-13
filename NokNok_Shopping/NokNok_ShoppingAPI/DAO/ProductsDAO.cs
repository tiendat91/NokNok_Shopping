using NokNok_ShoppingAPI.Models;

namespace NokNok_ShoppingAPI.DAO
{
    public class ProductsDAO
    {
        public static List<Product> GetProducts()
        {
            var list = new List<Product>();
            try
            {
                using (var context = new NokNok_ShoppingContext())
                {
                    list = context.Products.ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return list;
        }
        public static Product GetProductById(int id)
        {
            Product p = new Product();
            try
            {
                using (var context = new NokNok_ShoppingContext())
                {
                    p = context.Products.SingleOrDefault(x => x.ProductId == id);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return p;
        }

        public static void CreateProduct(Product p)
        {
            try
            {
                using (var context = new NokNok_ShoppingContext())
                {
                    context.Products.Add(p);
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public static void UpdateProduct(Product p)
        {
            try
            {
                using (var context = new NokNok_ShoppingContext())
                {
                    context.Entry<Product>(p).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public static void DeleteProduct(Product p)
        {
            try
            {
                using (var context = new NokNok_ShoppingContext())
                {
                    var p1 = context.Products.SingleOrDefault(x => x.ProductId == p.ProductId);
                    context.Products.Remove(p1);
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
