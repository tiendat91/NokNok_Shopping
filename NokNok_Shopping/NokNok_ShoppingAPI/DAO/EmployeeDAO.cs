using NokNok_ShoppingAPI.Models;

namespace NokNok_ShoppingAPI.DAO
{
    public class EmployeeDAO
    {
        public static List<Employee> GetEmployees()
        {
            var list = new List<Employee>();
            try
            {
                using (var context = new NokNok_ShoppingContext())
                {
                    list = context.Employees.ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return list;
        }
        public static Employee GetEmployeeById(int id)
        {
            Employee p = new Employee();
            try
            {
                using (var context = new NokNok_ShoppingContext())
                {
                    p = context.Employees.SingleOrDefault(x => x.EmployeeId == id);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return p;
        }
        public static void CreateEmployee(Employee p)
        {
            try
            {
                using (var context = new NokNok_ShoppingContext())
                {
                    context.Employees.Add(p);
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public static void UpdateEmployee(Employee p)
        {
            try
            {
                using (var context = new NokNok_ShoppingContext())
                {
                    context.Entry<Employee>(p).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public static void DeleteEmployee(Employee p)
        {
            try
            {
                using (var context = new NokNok_ShoppingContext())
                {
                    var p1 = context.Employees.SingleOrDefault(x => x.EmployeeId == p.EmployeeId);
                    context.Employees.Remove(p1);
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
