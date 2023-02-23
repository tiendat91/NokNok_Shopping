using ClosedXML.Excel;
using ExcelDataReader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MyRazorPage.Models;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.ExpressionGraph;
using System.Data;

namespace MyRazorPage.Pages.Product
{
    [Authorize(Roles = "1")]

    public class IndexModel : PageModel
    {
        private readonly PRN221DBContext dBContext;
        [BindProperty]
        public PaginatedList<Models.Product> ProductsList { get; set; }
        [BindProperty]
        public int TotalPage { get; set; }
        [BindProperty]
        public SelectList Categories { get; set; }
        [BindProperty]
        public string SelectedCategory { get; set; }
        public string SearchName { get; set; }
        public string CurrentFilterName { get; set; }
        public string CurrentFilterCat { get; set; }
        [BindProperty]
        public List<Models.Product> listProductImport { get; set; }
        [BindProperty]
        public IFormFile FileUpload { get; set; }

        public IndexModel(PRN221DBContext dBContext)
        {
            this.dBContext = dBContext;
        }
        public async Task OnGet(string? SelectedCategory, string? searchName, string? currentFilterName, string? currentFilterCat, int? pageIndex)
        {

            Categories = new SelectList(dBContext.Categories.AsNoTracking(), "CategoryId", "CategoryName", SelectedCategory);
            IQueryable<Models.Product> ProductsIQ = dBContext.Products.Include(c => c.Category).OrderByDescending(p => p.ProductId);
            ////////////////////////
            if (searchName == null)
            {
                searchName = currentFilterName;
            }
            CurrentFilterName = searchName;
            ///////////////////////
            if (SelectedCategory == null)
            {
                SelectedCategory = currentFilterCat;
            }
            CurrentFilterCat = SelectedCategory;

            if (SelectedCategory != null && searchName != null)
            {
                ProductsIQ = ProductsIQ.Where(s => s.CategoryId == Int32.Parse(SelectedCategory)
                                        && s.ProductName.ToLower().Contains(searchName.ToLower().Trim()));
            }
            else if (SelectedCategory != null)
            {
                ProductsIQ = ProductsIQ.Where(s => s.CategoryId == Int32.Parse(SelectedCategory));
            }

            else if (searchName != null)
            {
                ProductsIQ = ProductsIQ.Where(s => s.ProductName.ToLower().Contains(searchName.ToLower().Trim()));
            }

            ProductsList = await PaginatedList<Models.Product>.CreateAsync(ProductsIQ.AsNoTracking(), pageIndex ?? 1, 10);

            TotalPage = ProductsList.TotalPages;

        }

        public async Task<List<Models.Product>> Import(IFormFile file)
        {
            var list = new List<Models.Product>();
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0;
                using (var reader = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = reader.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;
                    for (int row = 2; row < rowCount; row++)
                    {
                        list.Add(new Models.Product
                        {
                            ProductName = worksheet.Cells[row, 1].Value.ToString().Trim(),
                            CategoryId = Int32.Parse(worksheet.Cells[row, 2].Value.ToString().Trim()),
                            QuantityPerUnit = worksheet.Cells[row, 3].Value.ToString().Trim(),
                            UnitPrice = Int32.Parse(worksheet.Cells[row, 4].Value.ToString().Trim()),
                            UnitsInStock = Int16.Parse(worksheet.Cells[row, 5].Value.ToString().Trim()),
                            Discontinued = Boolean.Parse(worksheet.Cells[row, 6].Value.ToString().Trim()),
                        });
                    }
                }
                list.Sort();
            }
            return list;
        }

        public FileResult OnGetExport()
        {
            DataTable dt = new DataTable("ProductExport");
            dt.Columns.AddRange(new DataColumn[10] { new DataColumn("ProductId"),
                                    new DataColumn("ProductName"),
                                    new DataColumn("CategoryId"),
                                    new DataColumn("QuantityPerUnit"),
                                    new DataColumn("UnitPrice"),
                                    new DataColumn("UnitsInStock"),
                                    new DataColumn("UnitsOnOrder"),
                                    new DataColumn("ReorderLevel"),
                                    new DataColumn("Discontinued"),
                                    new DataColumn("Image"),
            });

            var products = dBContext.Products.ToList();

            foreach (var product in products)
            {
                dt.Rows.Add(product.ProductId, product.ProductName,
                    product.CategoryId, product.QuantityPerUnit,
                    product.UnitPrice, product.UnitsInStock,
                    product.UnitsOnOrder, product.ReorderLevel,
                    product.Discontinued, product.Image);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ProductExport.xlsx");
                }
            }
        }
    }
}
