using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess.Domain;
using DataAccess.DbFactory;
using System.Data.Entity;

namespace DataAccess.Providers
{
    public class UserProvider
    {
        public string Register(User user)
        {
            using (DataDbContext context = new DataDbContext())
            {
                var org = context.Organisation.Add(new Organisation
                {
                    CreationTime = DateTime.Now,
                    Name = user.EmailAddress,
                    RoleName = "Buyer"
                });
                context.SaveChanges();
                user.OrganisationId = org.Id;

                var userToAdd = context.User.Add(user);
                context.SaveChanges();

                return userToAdd.UniqueId;
            }
        }

        public User Authenticate(string userName, string password)
        {
            using (DataDbContext context = new DataDbContext())
            {
                var user = context.User.Include(x => x.Organisation).Where(x => x.EmailAddress == userName && x.Password == password).FirstOrDefault();

                if(user.OrganisationId == 1)
                {
                    return user;
                }

                return null;
            }
        }

        public User GetUser(string UniqueId)
        {
            using (DataDbContext context = new DataDbContext())
            {
                var user = context.User.Where(x => x.UniqueId == UniqueId).FirstOrDefault();
                user.Password = null;
                return user;
            }
        }

        public bool IsUserExist(string emailAddress)
        {
            using (DataDbContext context = new DataDbContext())
            {
                if (context.User.Where(x => x.EmailAddress == emailAddress).Any())
                    return true;
                return false;
            }
        }

        public int GetUserId(string emailAddress)
        {
            using (DataDbContext context = new DataDbContext())
            {
                return context.User.Where(x => x.EmailAddress == emailAddress).FirstOrDefault().Id;
            }
        }

        public void UpdatePasswordByEmail(string emailAddress, string password)
        {
            using (DataDbContext context = new DataDbContext())
            {
                User user = context.User.Where(x => x.EmailAddress.ToLower() == emailAddress.ToLower()).FirstOrDefault();
                user.Password = password;
                context.SaveChanges();
            }
        }

        public string GetUserPasswordByEmail(string emailAddress)
        {
            using (DataDbContext context = new DataDbContext())
            {
                return context.User.Where(x => x.EmailAddress == emailAddress).FirstOrDefault().Password;
            }
        }

        public User GetUserByEmail(string emailAddress)
        {
            using (DataDbContext context = new DataDbContext())
            {
                return context.User.Where(x => x.EmailAddress == emailAddress).FirstOrDefault();
            }
        }

        public void UpdateUserByEmail(User user)
        {
            using (DataDbContext context = new DataDbContext())
            {
                var existingUser = context.User.Where(usr => usr.EmailAddress == user.EmailAddress).FirstOrDefault();

                existingUser.FirstName = user.FirstName;
                existingUser.LastName = user.LastName;
                existingUser.Address = user.Address;
                existingUser.City = user.City;
                existingUser.Country = user.Country;
                existingUser.Phone = user.Phone;
                existingUser.PostalCode = user.PostalCode;
                existingUser.State = user.State;
                existingUser.Street = user.Street;

                context.SaveChanges();
            }
        }

        public List<User> GetUsers()
        {
            using (DataDbContext context = new DataDbContext())
            {
                return context.User.Include(o => o.Organisation).Where(x => x.Organisation.RoleName == "Buyer").ToList();
            }
        }
    }
}