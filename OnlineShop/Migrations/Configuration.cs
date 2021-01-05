namespace OnlineShop.Migrations
{
    using OnlineShop.DbFactory;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<OnlineShop.DbFactory.ShopDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(OnlineShop.DbFactory.ShopDbContext context)
        {
            SeedOrganisation(context);
            SeedUser(context);
            SeedSize(context);
            SeedColour(context);
            SeedOrderStatus(context);
            SeedPaymentStatus(context);
            SeedCategory(context);
            SeedGenderTags(context);
        }

        private void SeedOrganisation(ShopDbContext context)
        {
            if (!context.Organisation.Any())
            {
                context.Organisation.Add(new DomainEntities.Organisation
                {
                    Name = "Hussnain",
                    RoleName = CommonConstants.Constants.AdminRole,
                    IsActive = true
                });
                context.SaveChanges();
                context.Organisation.Add(new DomainEntities.Organisation
                {
                    Name = "Marketplace",
                    RoleName = CommonConstants.Constants.BuyerRole,
                    IsActive = true
                });
                context.SaveChanges();
            }
        }

        private void SeedUser(ShopDbContext context)
        {
            if (!context.User.Any())
            {
                context.User.Add(new DomainEntities.User
                {
                    FirstName = "Wajahat",
                    LastName = "Naeem",
                    EmailAddress = "wajahat.naeem@outlook.com",
                    Password = Encode("hussnain"),
                    OrganisationId = context.Organisation.Where(x => x.Name == "Hussnain").FirstOrDefault().Id,
                    Phone = "03204194714",
                    City = "Lahore",
                    Country = "Pakistan",
                    Address = "House # 227, Block F2, Wapda Town, Lahore",
                    State = "Punjab",
                    Street = "street 10",
                    PostalCode = "54000",
                    UniqueId = Guid.NewGuid().ToString(),
                    IsActive = true,
                    CreationTime = DateTime.Now
                });
                context.SaveChanges();
            }
        }

        private void SeedSize(ShopDbContext context)
        {
            if (!context.Size.Any())
            {
                context.Size.Add(new DomainEntities.Size
                {
                    Name = "Default"
                });
                context.SaveChanges();
                context.Size.Add(new DomainEntities.Size
                {
                    Name = "S"
                });
                context.SaveChanges();
                context.Size.Add(new DomainEntities.Size
                {
                    Name = "M"
                });
                context.SaveChanges();
                context.Size.Add(new DomainEntities.Size
                {
                    Name = "L"
                });
                context.SaveChanges();
                context.Size.Add(new DomainEntities.Size
                {
                    Name = "XL"
                });
                context.SaveChanges();
                context.Size.Add(new DomainEntities.Size
                {
                    Name = "2XL"
                });
                context.SaveChanges();
            }
        }

        private void SeedColour(ShopDbContext context)
        {
            if (!context.Colour.Any())
            {
                context.Colour.Add(new DomainEntities.Colour
                {
                    Name = "Black",
                    Code= "#FFFFFF"
                });
                context.SaveChanges();
                context.Colour.Add(new DomainEntities.Colour
                {
                    Name = "White",
                    Code = "#F9F7F6"
                });
                context.SaveChanges();
                context.Colour.Add(new DomainEntities.Colour
                {
                    Name = "Brown",
                    Code= "#8B4513"
                });
                context.SaveChanges();
            }
        }

        private void SeedOrderStatus(ShopDbContext context)
        {
            if (!context.OrderStatus.Any())
            {
                context.OrderStatus.Add(new DomainEntities.OrderStatus
                {
                    Name = "Open"
                });
                context.SaveChanges();
                context.OrderStatus.Add(new DomainEntities.OrderStatus
                {
                    Name = "Confirm"
                });
                context.SaveChanges();
                context.OrderStatus.Add(new DomainEntities.OrderStatus
                {
                    Name = "In Process"
                });
                context.SaveChanges();
                context.OrderStatus.Add(new DomainEntities.OrderStatus
                {
                    Name = "Dispatched"
                });
                context.SaveChanges();
                context.OrderStatus.Add(new DomainEntities.OrderStatus
                {
                    Name = "Closed"
                });
                context.SaveChanges();
                context.OrderStatus.Add(new DomainEntities.OrderStatus
                {
                    Name = "Cancelled"
                });
                context.SaveChanges();
            }
        }

        private void SeedPaymentStatus(ShopDbContext context)
        {
            if (!context.PaymentStatus.Any())
            {
                context.PaymentStatus.Add(new DomainEntities.PaymentStatus
                {
                    Name = "Pending"
                });
                context.SaveChanges();
                context.PaymentStatus.Add(new DomainEntities.PaymentStatus
                {
                    Name = "Approved"
                });
                context.SaveChanges();
                context.PaymentStatus.Add(new DomainEntities.PaymentStatus
                {
                    Name = "Cancelled"
                });
                context.SaveChanges();
            }
        }

        private void SeedCategory(ShopDbContext context)
        {
            if (!context.Category.Any())
            {
                context.Category.Add(new DomainEntities.Category
                {
                    Name = "Gothic Wear",
                    DisplayOrder = 4,
                    IsActive = true
                });
                context.SaveChanges();
                SeedChildCategory("Gothic Wear", "Men", context);
                SeedChildCategory("Gothic Wear", "Ladies", context);

                context.Category.Add(new DomainEntities.Category
                {
                    Name = "Boxing Gear",
                    DisplayOrder = 3,
                    IsActive = true
                });
                context.SaveChanges();
                SeedChildCategory("Boxing Gear", "Boxing Gloves", context);
                SeedChildCategory("Boxing Gear", "Protection", context);
                SeedChildCategory("Boxing Gear", "Training Pads", context);
                
                context.Category.Add(new DomainEntities.Category
                {
                    Name = "Apparel",
                    DisplayOrder = 2,
                    IsActive = true
                });
                context.SaveChanges();
                SeedChildCategory("Apparel", "T Shirts", context);
                SeedChildCategory("Apparel", "Hoodies", context);
                SeedChildCategory("Apparel", "Shorts", context);
                SeedChildCategory("Apparel", "Leggings", context);
                context.Category.Add(new DomainEntities.Category
                {
                    Name = "Fitness",
                    DisplayOrder = 1,
                    IsActive = true
                });
                context.SaveChanges();
                SeedChildCategory("Fitness", "Leather Belts", context);
                SeedChildCategory("Fitness", "Neoprene Belts", context);
                SeedChildCategory("Fitness", "Wrist Bands", context);
                SeedChildCategory("Fitness", "Weightlifting Gloves", context);
            }
        }

        private void SeedChildCategory(string parentName, string ChildName,ShopDbContext context)
        {
            var parent = context.Category.Where(x => x.Name == parentName && x.ParentCategoryId == 0).FirstOrDefault();
            DomainEntities.Category cat = new DomainEntities.Category
            {
                Name = ChildName,
                IsActive = true,
                ParentCategoryId = parent.Id
            };
            context.Category.Add(cat);
            context.SaveChanges();
        }

        private void SeedGenderTags(ShopDbContext context)
        {
            if (!context.GenderTag.Any())
            {
                context.GenderTag.Add(new DomainEntities.GenderTag { Name = "All" });
                context.GenderTag.Add(new DomainEntities.GenderTag { Name = "Male" });
                context.GenderTag.Add(new DomainEntities.GenderTag { Name = "Female" });
               
                context.SaveChanges();
            }
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
    }
}
