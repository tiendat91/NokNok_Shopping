using NokNok_ShoppingAPI.Models;

namespace NokNok_ShoppingAPI.DAO
{
    public class AccountsDAO
    {
        public static List<Account> GetAccounts()
        {
            var list = new List<Account>();
            try
            {
                using (var context = new NokNok_ShoppingContext())
                {
                    list = context.Accounts.ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return list;
        }

        public static Account GetAccountById(int id)
        {
            Account p = new Account();
            try
            {
                using (var context = new NokNok_ShoppingContext())
                {
                    p = context.Accounts.SingleOrDefault(x => x.AccountId == id);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return p;
        }
        public static void CreateAccount(Account p)
        {
            try
            {
                using (var context = new NokNok_ShoppingContext())
                {
                    context.Accounts.Add(p);
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public static void UpdateAccount(Account p)
        {
            try
            {
                using (var context = new NokNok_ShoppingContext())
                {
                    context.Entry<Account>(p).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public static void DeleteAccount(Account p)
        {
            try
            {
                using (var context = new NokNok_ShoppingContext())
                {
                    var p1 = context.Accounts.SingleOrDefault(x => x.AccountId == p.AccountId);
                    context.Accounts.Remove(p1);
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
