using OnlineShop.DAL;
using OnlineShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web.Http;

namespace WebAPI.Controllers
{
    public class AccountController : ApiController
    {
        private readonly UserProvider userProvider;
        public AccountController()
        {
            userProvider = new UserProvider();
        }


        public IHttpActionResult Register(RegisterSignin registerUser)
        {
            if (ModelState.IsValid)
            {
                if (!userProvider.IsUserExist(registerUser.EmailAddress))
                {
                    string uniqueId = userProvider.Register(MapUserToDomain(registerUser));

                    try
                    {
                        SendEmail(registerUser.EmailAddress, "Your account has registered successfully");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Email sending failed");
                    }

                    return Ok(uniqueId);
                }
            }
            throw new Exception("something went wrong");
        }

        #region Private
        private OnlineShop.DomainEntities.User MapUserToDomain(OnlineShop.Models.RegisterSignin user)
        {
            return new OnlineShop.DomainEntities.User
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmailAddress = user.EmailAddress,
                Address = user.Address,
                City = user.City,
                Country = user.Country,
                Phone = user.Phone,
                PostalCode = user.PostalCode,
                State = user.State,
                Street = user.Street,
                CreationTime = DateTime.Now,
                IsActive = true,
                Password = Encode(user.Password),
                UniqueId = Guid.NewGuid().ToString()
            };
        }

        private void SendEmail(string recipient, string message)
        {
            MailMessage mail = new MailMessage();
            mail.To.Add(recipient);
            mail.From = new MailAddress("myemail");
            mail.Subject = "Forget Password";
            mail.Body = message;// "send passsword in this body";
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential("myemail", "password");
            smtp.Send(mail);
        }

        private static string Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        private static string Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        #endregion
    }
}
