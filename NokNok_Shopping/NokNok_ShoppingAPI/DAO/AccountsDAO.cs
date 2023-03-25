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
    }
}
