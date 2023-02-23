using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OfficeOpenXml;
using System.Data;

namespace MyRazorPage.Pages.Admin
{
    [Authorize(Roles = "1")]

    public class OrderModel : PageModel
    {
        private readonly PRN221DBContext dBContext;
        [BindProperty]
        public PaginatedList<Order> OrderedList { get; set; }
        public List<Order> OrderExport = new List<Order>();
        [BindProperty]
        public int TotalPage { get; set; }
        [BindProperty]
        public DateTime CurrentFromDate { get; set; }
        [BindProperty]
        public DateTime CurrentToDate { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public OrderModel(PRN221DBContext dBContext)
        {
            this.dBContext = dBContext;
        }
        public async Task<IActionResult> OnGet(string? orderId, DateTime? fromDate, DateTime? toDate, DateTime? currentFromDate, DateTime? currentToDate, int? pageIndex, string? isExport)
        {
            //Cancel Order
            if (orderId != null)
            {
                var x = dBContext.Orders.SingleOrDefault(s => s.OrderId == Int32.Parse(orderId));
                if (x != null)
                {
                    x.RequiredDate = null;
                    dBContext.Orders.Update(x);
                    await dBContext.SaveChangesAsync();
                }
            }

            IQueryable<Order> OrderesIQ = dBContext.Orders.Include(s => s.Customer).Include(s => s.Employee).OrderByDescending(s => s.OrderId);
            if (fromDate == null)
            {
                fromDate = currentFromDate;
            }
            currentFromDate = fromDate;
            /////////////////////
            if (toDate == null)
            {
                toDate = currentToDate;
            }
            currentToDate = toDate;

            if (fromDate != null)
            {
                OrderesIQ = OrderesIQ.Where(s => s.OrderDate >= fromDate);
            }
            else if (toDate != null)
            {
                OrderesIQ = OrderesIQ.Where(s => s.OrderDate <= toDate);
            }
            if (fromDate != null && toDate != null)
            {
                OrderesIQ = OrderesIQ.Where(s => (s.OrderDate >= fromDate) && (s.OrderDate <= toDate));
            }

            OrderExport = OrderesIQ.ToList();

            OrderedList = await PaginatedList<Order>.CreateAsync(OrderesIQ.AsNoTracking(), pageIndex ?? 1, 10);

            TotalPage = OrderedList.TotalPages;
            return Page();
        }


        public FileResult OnGetExport(DateTime? fromDate, DateTime? toDate)
        {
            DataTable dt = new DataTable("OrdersExport");
            var orders = dBContext.Orders.Include(s => s.Customer).Include(s => s.Employee).OrderByDescending(s => s.OrderId);

            dt.Columns.AddRange(new DataColumn[13] { new DataColumn("OrderId"),
                                    new DataColumn("CustomerId"),
                                    new DataColumn("EmployeeId"),
                                    new DataColumn("OrderDate"),
                                    new DataColumn("RequiredDate"),
                                    new DataColumn("ShippedDate"),
                                    new DataColumn("Freight"),
                                    new DataColumn("ShipName"),
                                    new DataColumn("ShipAddress"),
                                    new DataColumn("ShipCity"),
                                    new DataColumn("ShipRegion"),
                                    new DataColumn("ShipPostalCode"),
                                    new DataColumn("ShipCountry")
            });

            foreach (var order in orders)
            {
                dt.Rows.Add(order.OrderId, order.CustomerId,
                    order.EmployeeId, order.OrderDate,
                    order.RequiredDate, order.ShippedDate,
                    order.Freight, order.ShipName,
                    order.ShipAddress, order.ShipCity,
                    order.ShipRegion, order.ShipPostalCode,
                    order.ShipCountry);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "", "OrdersExport.xlsx");
                }
            }
        }

    }
}
