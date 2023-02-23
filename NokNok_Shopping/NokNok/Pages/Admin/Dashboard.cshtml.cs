using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MyRazorPage.Pages.Admin
{
    [Authorize(Roles = "1")]
    public class DashboardModel : PageModel
    {
        private readonly PRN221DBContext dBContext;

        public DashboardModel(PRN221DBContext dBContext)
        {
            this.dBContext = dBContext;
        }
        [BindProperty]
        public List<Customer> Customer { get; set; }
        [BindProperty]
        public List<OrderDetail> OrderDetails { get; set; }
        [BindProperty]
        public double TotalOrders { get; set; }
        [BindProperty]
        public int TotalCustomers { get; set; }
        [BindProperty]
        public int TotalGuest { get; set; }
        [BindProperty]
        public int NewCustomers { get; set; }
        public SelectList SelectedYear { get; set; }
        [BindProperty]
        public List<int> StatisticOrdersByMonth { get; set; }
        [BindProperty]
        public int WeeklySales { get; set; }

        public async Task OnGet(string? selectedYear)
        {
            SelectedYear = new SelectList(dBContext.Orders.Select(s => new
            {
                YearKey = s.OrderDate.Value.Year.ToString(),
                YearValue = s.OrderDate.Value.Year.ToString()
            }).Distinct(), "YearKey", "YearValue", SelectedYear);

            Customer = dBContext.Customers.ToList();


            //Total Orders (tổng số tiền order details = số tiền mỗi đơn hàng - discount từng đơn)
            OrderDetails = dBContext.OrderDetails.Include(s => s.Order).Include(p => p.Product).ToList();
            foreach (var item in OrderDetails)
            {
                double discount = (double)item.Discount * (double)item.Product.UnitPrice * item.Quantity;
                TotalOrders += Math.Round(((double)item.Product.UnitPrice * (double)item.Quantity) - discount);
            }
            TotalOrders = (int)dBContext.Orders.Count();

            //Total Customer (số acccount - đi admin)
            TotalCustomers = dBContext.Accounts.Where(s => s.CustomerId != null).Count(); //Null chưa ăn


            //Total Guest (số customer ko có account) 
            TotalGuest = dBContext.Customers.Count() - TotalCustomers;

            //New Customer in 30days
            NewCustomers = dBContext.Accounts.Include(s => s.Customer).Where(s => s.Customer.CreatedDate > (DateTime.Now.AddDays(-30))).Count();

            //Statistic Orders (số lượng đơn hàng theo tháng)
            if (selectedYear == null)
            {
                selectedYear = "1996";
            }
            List<int> temp = new List<int>();
            for (int i = 1; i <= 12; i++)
            {
                int NumberOrderEachMonth = dBContext.OrderDetails.Include(s => s.Order)
                    .Where(s => s.Order.OrderDate.Value.Month == i && s.Order.OrderDate.Value.Year == Int32.Parse(selectedYear)).Count();
                temp.Add(NumberOrderEachMonth);
            }
            StatisticOrdersByMonth = temp;

            //Weekly Sales (Số hàng trung bình bán được theo tuần) - tam tinh theo thang
            foreach (var item in StatisticOrdersByMonth)
            {
                WeeklySales += item;
            }
            WeeklySales = (int)Math.Floor((double)WeeklySales / StatisticOrdersByMonth.Count());
        }
    }
}
