using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace MyRazorPage.Pages.Admin.Product
{
    public class ImportFileModel : PageModel
    {
        private readonly PRN221DBContext dBContext;

        public ImportFileModel(PRN221DBContext dBContext)
        {
            this.dBContext = dBContext;
        }
        public IFormFile file;
        [BindProperty]
        public List<Models.Product> ListImportProduct { get; set; }
        public void OnGet()
        {

        }

        public List<Models.Product> LoadExcel(IFormFile? fileUpload)
        {
            try
            {
                var list = new List<Models.Product>();
                using (var stream = new MemoryStream())
                {
                    fileUpload.CopyTo(stream);
                    stream.Position = 0;
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        var count = 0;
                        do
                        {
                            while (reader.Read())
                            {
                                count++;
                                var data = reader;
                                if (count != 1)
                                {
                                    list.Add(new Models.Product
                                    {
                                        ProductName = reader[0].ToString().Trim(),
                                        CategoryId = Int32.Parse(reader[1].ToString().Trim()),
                                        QuantityPerUnit = reader[2].ToString().Trim(),
                                        UnitPrice = Int32.Parse(reader[3].ToString().Trim()),
                                        UnitsInStock = Int16.Parse(reader[4].ToString().Trim()),
                                        Discontinued = Convert.ToBoolean((reader[5].ToString().Trim())),
                                    });
                                }
                            }
                        } while (reader.NextResult());
                    }

                    return list;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IActionResult> OnPost(IFormFile? fileUpload)
        {
            if (fileUpload != null)
            {
                ListImportProduct = LoadExcel(fileUpload);

                //insert into database
                foreach (var item in ListImportProduct)
                {
                    dBContext.Products.Add(item);
                }
                dBContext.SaveChanges();

            }
            return RedirectToPage("/admin/product/index");
        }

    }
}
