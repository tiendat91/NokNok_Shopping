using EmailService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace MyRazorPage.Pages.Account
{
    public class ForgotModel : PageModel
    {
        private readonly PRN221DBContext db;
        private readonly Random _random = new Random();
        private readonly IEmailSender _emailSender;
        public ForgotModel(IEmailSender emailSender, PRN221DBContext db)
        {
            this._emailSender = emailSender;
            this.db = db;
        }
        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPost(string? email)
        {
            //check email exist?
            bool checkExist = false;
            if (email != null)
            {
                foreach (var item in db.Accounts)
                {
                    if (item.Email.ToLower() == email.ToLower().Trim())
                    {
                        checkExist = true;
                    }
                }
                if (checkExist)
                {
                    //Tao mat khau moi
                    var acc = db.Accounts.Include(s => s.Customer).SingleOrDefault(s => s.Email.ToLower().Equals(email.ToLower()));
                    acc.Password = GeneratePassword();
                    db.Accounts.Update(acc);
                    db.SaveChanges();

                    String contentSend = $"Hi {acc.Customer.ContactName}, <br><br>" + "You recently requested to reset password for PRN221_RazorPage account." +
                            "\nYour password is now change to: " + $"<h2 style='color:red'>{acc.Password}</h2>" + "<br><br>Thank you,<br>dtd";

                    var message = new Message(new string[] { email }, "CHECK TO RESET PASSWORD", contentSend, null);
                    _emailSender.SendEmail(message);
                }
                else
                {
                    ViewData["msg"] = "This account does not exist.";
                    return Page();
                }
            }
            return RedirectToPage("/account/signin");
        }

        public string GeneratePassword()
        {
            int size = 5;
            var generatedID = new StringBuilder(size);
            for (var i = 0; i < size; i++)
            {
                var @char = (char)_random.Next(65, 90);
                generatedID.Append(@char);
            }
            return generatedID.ToString().ToLower();
        }


    }
}
