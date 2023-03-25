using NokNok_ShoppingAPI.Models;

namespace NokNok_ShoppingAPI.DAO
{
    public class DepartmentDAO
    {
        public static List<Department> GetDepartments()
        {
            var list = new List<Department>();
            try
            {
                using (var context = new NokNok_ShoppingContext())
                {
                    list = context.Departments.ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return list;
        }
    }
}
