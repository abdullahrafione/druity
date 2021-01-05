using OnlineShop.DbFactory;
using OnlineShop.DomainEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace OnlineShop.DAL
{
    public class ProductProvider
    {
        public List<OnlineShop.Models.MiniCartModel> GetCart(List<int> ProductStockIds)
        {
            List<OnlineShop.Models.MiniCartModel> list = new List<Models.MiniCartModel>();
            using (ShopDbContext context = new ShopDbContext())
            {
                var products = context.ProductStock.Include("Product").Include(s => s.Size).Include(s => s.Colour).Where(x => ProductStockIds.Contains(x.Id) && x.IsDeleted == false).ToList();
                products.ForEach(x =>
                {
                    OnlineShop.Models.MiniCartModel productModel = new Models.MiniCartModel
                    {
                        ProductName = x.Product.Name,
                        ProductId = x.Product.Id,
                        ColorName = x.Colour.Name,
                        SizeName = x.Size.Name,
                        Price = x.CurrentPrice,
                        CurrencySymbol = x.CurrencySymbol,
                        Quantity = x.StockCount,
                        ProductStockId = x.Id
                    };
                    list.Add(productModel);
                });
            }
            return list;
        }

        public OnlineShop.Models.ProductReponse GetProducts(int currentPage)
        {
            int maxRows = 15;
            using (ShopDbContext context = new ShopDbContext())
            {
                var products = (from product in context.Product
                                where product.IsDeleted == false
                                select product).OrderByDescending(x => x.CreationTime).Skip((currentPage - 1) * maxRows)
                        .Take(maxRows).ToList();

                OnlineShop.Models.ProductReponse reponse = new OnlineShop.Models.ProductReponse();

                double pageCount = (double)((decimal)context.Product.Where(x => x.IsDeleted == false).Count() / Convert.ToDecimal(maxRows));
                reponse.PageCount = (int)Math.Ceiling(pageCount);

                reponse.CurrentPageIndex = currentPage;
                products.ForEach(x =>
                {
                    string CategoryName = context.Category.Where(c => c.Id == x.CategoryId).FirstOrDefault().Name;
                    var model = new Models.Product
                    {
                        CategoryId = x.CategoryId,
                        CategoryName = CategoryName,
                        isActive = x.IsActive,
                        Name = x.Name,
                        Detail = x.Detail,
                        OrganisationId = x.OrganisationId,
                        ProductId = x.Id
                    };
                    reponse.Products.Add(model);
                });

                return reponse;
            }
        }

        public OnlineShop.Models.ProductReponse GetProductsForShop(int currentPage, List<int> genderTagIds , int parentCategoryId = default(int), int childcategory = default(int),string searchText= "")
        {
            
            
            using (ShopDbContext context = new ShopDbContext())
            {
                int maxRows = 15;
                var tags = context.GenderTag.ToList();
                int all = tags.Where(x => x.Name == "All").FirstOrDefault().Id;
                if (!genderTagIds.Any())
                {
                    genderTagIds.AddRange(tags.ToList().Select(x => x.Id).ToList());
                }

                List<Product> products = new List<Product>();
                if (parentCategoryId > 0)
                {
                    
                    var childCategoryIds = context.Category.Where(x => x.ParentCategoryId == parentCategoryId).Select(x => x.Id).ToList();

                    products = (from product in context.Product
                              .Include("ProductStock")
                              .Include("ProductStock.Colour")
                              .Include("ProductStock.Size")
                              .Include("Category")
                                where product.IsDeleted == false && product.IsActive == true && (childCategoryIds.Contains(product.CategoryId) || (genderTagIds.Contains(product.CategoryId)))
                                select product).OrderByDescending(x => x.CreationTime).Skip((currentPage - 1) * maxRows).ToList();
                }

                if (parentCategoryId == 0 && childcategory > 0)
                {
                    products = context.Product
                              .Include("ProductStock")
                              .Include("ProductStock.Colour")
                              .Include("ProductStock.Size")
                              .Include("Category").Where(x => x.IsActive == true && x.CategoryId == childcategory).OrderByDescending(x => x.CreationTime).Skip((currentPage - 1) * maxRows).Take(maxRows).ToList();


                    //          where product.IsDeleted == false && product.IsActive == true && (product.CategoryId==childcategory && (genderTagIds.Contains(product.CategoryId)))
                    //          select product).OrderByDescending(x => x.CreationTime).Skip((currentPage - 1) * maxRows)
                    //.Take(maxRows).ToList();
                }
                else if (searchText != null && searchText != "")
                {
                    products = (from product in context.Product
                                  .Include("ProductStock")
                                  .Include("ProductStock.Colour")
                                  .Include("ProductStock.Size")
                                  .Include("GenderTag")
                                  .Include("Category")
                                where product.IsDeleted == false && product.IsActive == true &&
                                (product.Name.Contains(searchText)
                                   || product.ShortDescription.Contains(searchText)
                                   || product.Detail.Contains(searchText)
                                   || product.Category.Name.Contains(searchText)
                                   || product.GenderTag.Name.Contains(searchText)
                                 )
                                && (genderTagIds.Contains(product.CategoryId))
                                select product).OrderByDescending(x => x.CreationTime).Skip((currentPage - 1) * maxRows)
                          .Take(maxRows).ToList();
                }
                else if (parentCategoryId == 0 && childcategory == 0)
                {
                    products = (from product in context.Product
                                   .Include("ProductStock")
                                   .Include("ProductStock.Colour")
                                   .Include("ProductStock.Size")
                                   .Include("Category")
                                where product.IsDeleted == false && product.IsActive == true && (genderTagIds.Contains(product.GenderTagId))
                                select product).OrderByDescending(x => x.CreationTime).Skip((currentPage - 1) * maxRows)
                           .Take(maxRows).ToList();
                }
                double pageCount = (double)((decimal)context.Product.Where(x => x.IsDeleted == false).Count() / Convert.ToDecimal(maxRows));

                var reponse = MapResponse(products);
                reponse.PageCount = (int)Math.Ceiling(pageCount);
                reponse.CurrentPageIndex = currentPage;

                return reponse;
            }
        }

        //public OnlineShop.Models.ProductReponse GetProductsForShop(int currentPage, List<int> genderTagIds, int parentCategoryId = default(int), int childcategory = default(int), string searchText = "")
        //{
        //    int maxRows = 15;

        //    using (ShopDbContext context = new ShopDbContext())
        //    {
        //        var tags = context.GenderTag.ToList();
        //        int all = tags.Where(x => x.Name == "All").FirstOrDefault().Id;
        //        if (!genderTagIds.Any())
        //        {
        //            genderTagIds.AddRange(tags.ToList().Select(x => x.Id).ToList());
        //        }

        //        List<Product> products = new List<Product>();
        //        if (parentCategoryId > 0)
        //        {
        //            var childCategoryIds = context.Category.Where(x => x.ParentCategoryId == parentCategoryId).Select(x => x.Id).ToList();
        //            products = (from product in context.Product
        //                      .Include("ProductStock")
        //                      .Include("ProductStock.Colour")
        //                      .Include("ProductStock.Size")
        //                      .Include("Category")
        //                        where product.IsDeleted == false && product.IsActive == true || (childCategoryIds.Contains(product.CategoryId) || (genderTagIds.Contains(product.CategoryId)))
        //                        select product).OrderByDescending(x => x.CreationTime).Skip((currentPage - 1) * maxRows)
        //              .Take(maxRows).ToList();
        //        }

        //        if (parentCategoryId == 0 && childcategory > 0)
        //        {
        //            var categoryIds = context.Category.Where(x => x.Id == childcategory).Select(x => x.Id).ToList();
        //            products = (from product in context.Product
        //                      .Include("ProductStock")
        //                      .Include("ProductStock.Colour")
        //                      .Include("ProductStock.Size")
        //                      .Include("Category")
        //                        where product.IsDeleted == false && product.IsActive == true || (categoryIds.Contains(product.CategoryId) || (genderTagIds.Contains(product.CategoryId)))
        //                        select product).OrderByDescending(x => x.CreationTime).Skip((currentPage - 1) * maxRows)
        //              .Take(maxRows).ToList();
        //        }
        //        else if (searchText != null && searchText != "")
        //        {
        //            products = (from product in context.Product
        //                          .Include("ProductStock")
        //                          .Include("ProductStock.Colour")
        //                          .Include("ProductStock.Size")
        //                          .Include("GenderTag")
        //                          .Include("Category")
        //                        where product.IsDeleted == false && product.IsActive == true &&
        //                        (product.Name.Contains(searchText)
        //                           || product.ShortDescription.Contains(searchText)
        //                           || product.Detail.Contains(searchText)
        //                           || product.Category.Name.Contains(searchText)
        //                           || product.GenderTag.Name.Contains(searchText)
        //                         )
        //                        && (genderTagIds.Contains(product.CategoryId))
        //                        select product).OrderByDescending(x => x.CreationTime).Skip((currentPage - 1) * maxRows)
        //                  .Take(maxRows).ToList();
        //        }
        //        else if (parentCategoryId == 0 && childcategory == 0)
        //        {
        //            products = (from product in context.Product
        //                           .Include("ProductStock")
        //                           .Include("ProductStock.Colour")
        //                           .Include("ProductStock.Size")
        //                           .Include("Category")
        //                        where product.IsDeleted == false && product.IsActive == true && (genderTagIds.Contains(product.GenderTagId))
        //                        select product).OrderByDescending(x => x.CreationTime).Skip((currentPage - 1) * maxRows)
        //                   .Take(maxRows).ToList();
        //        }
        //        double pageCount = (double)((decimal)context.Product.Where(x => x.IsDeleted == false).Count() / Convert.ToDecimal(maxRows));

        //        var reponse = MapResponse(products);
        //        reponse.PageCount = (int)Math.Ceiling(pageCount);
        //        reponse.CurrentPageIndex = currentPage;

        //        return reponse;
        //    }
        //}

        public OnlineShop.Models.ProductReponse Filter(Models.FilterCriteria criteria)
        {
            int maxRows = 15;
            using (ShopDbContext context = new ShopDbContext())
            {
                var tags = context.GenderTag.ToList();
                int all = tags.Where(x => x.Name == "All").FirstOrDefault().Id;
                if (criteria.GenderTagId.FirstOrDefault() == all || criteria.GenderTagId.FirstOrDefault() == default(int))
                {
                    criteria.GenderTagId.AddRange(tags.ToList().Select(x => x.Id).ToList());
                }
            }
            if (criteria.CurrentPage == 0)
            {
                criteria.CurrentPage = 1;
            }
            if (!criteria.ParentCategoryIds.Any() && !criteria.ColourIds.Any() && !criteria.SizeIds.Any() && !criteria.ChildCategoryIds.Any() && criteria.SearchText == null)
            {
                return this.GetProductsForShop(criteria.CurrentPage, criteria.GenderTagId);
            }

            using (ShopDbContext context = new ShopDbContext())
            {
                var query = (from cat in context.Category
                             join prod in context.Product on cat.Id equals prod.CategoryId
                             join stock in context.ProductStock on prod.Id equals stock.ProductId
                             join color in context.Colour on stock.ColourId equals color.Id
                             join size in context.Size on stock.SizeID equals size.Id
                             join gender in context.GenderTag on prod.GenderTagId equals gender.Id
                             where ((prod.IsDeleted == false && prod.IsActive == true && stock.IsDeleted == false) && (
                             criteria.ParentCategoryIds.Contains(prod.CategoryId)
                             || prod.Name.ToLower().Contains(criteria.SearchText.ToLower())
                             || prod.Detail.ToLower().Contains(criteria.SearchText.ToLower())
                             || size.Name.ToLower().Contains(criteria.SearchText.ToLower())
                             || color.Name.ToLower().Contains(criteria.SearchText.ToLower())
                             || criteria.ColourIds.Contains(color.Id)
                             || criteria.SizeIds.Contains(size.Id)
                           //  || criteria.GenderTagId.Contains(prod.GenderTagId)
                             ))
                             select new { prod, stock, size, color, cat });

                List<Product> products = new List<Product>();

                var list = query.ToList();
                if (criteria.ParentCategoryIds.Any())
                {
                    if (criteria.SizeIds.Any())
                    {
                        list = list.Where(x => criteria.SizeIds.Contains(x.stock.SizeID) && criteria.ParentCategoryIds.Contains(x.cat.Id)).ToList();
                        if (criteria.ColourIds.Any())
                        {
                            list = list.Where(x => criteria.ColourIds.Contains(x.stock.ColourId) && criteria.SizeIds.Contains(x.stock.SizeID)).ToList();
                        }
                    }
                    else if (criteria.ColourIds.Any() && !criteria.SizeIds.Any())
                    {
                        list = list.Where(x => criteria.ColourIds.Contains(x.stock.ColourId) && criteria.ParentCategoryIds.Contains(x.cat.Id)).ToList();
                    }
                }

                if (!criteria.ParentCategoryIds.Any() && criteria.SizeIds.Any() && criteria.ColourIds.Any())
                {
                    list = list.Where(x => criteria.ColourIds.Contains(x.stock.ColourId) && criteria.SizeIds.Contains(x.stock.SizeID)).ToList();
                }
                if (criteria.GenderTagId.Any())
                {
                    list = list.Where(x => criteria.GenderTagId.Contains(x.prod.GenderTagId)).ToList();
                    if (!list.Any())
                    {
                        criteria.GenderTagId = null;
                    }
                }
                list.ForEach(x =>
            {
                Product product = new Product();
                product.Id = x.prod.Id;
                product.Name = x.prod.Name;
                product.CategoryId = x.prod.CategoryId;
                product.Category = x.prod.Category;
                product.Detail = x.prod.Detail;
                product.OrganisationId = x.prod.OrganisationId;
                product.CreationTime = x.prod.CreationTime;
                product.IsActive = x.prod.IsActive;
                product.ProductStock = x.prod.ProductStock;
                product.ProductStock.FirstOrDefault().Colour = x.color;
                product.ProductStock.FirstOrDefault().Size = x.size;

                products.Add(product);
            });

                var DistinctList = products.GroupBy(w => w.Id).Select(g => g.First()).ToList();
                var productsForResponse = DistinctList.OrderByDescending(x => x.CreationTime).Skip((criteria.CurrentPage - 1) * maxRows).Take(maxRows).ToList();

                double pageCount = (double)((decimal)DistinctList.Count() / Convert.ToDecimal(maxRows));

                var response = MapResponse(productsForResponse);
                response.PageCount = (int)Math.Ceiling(pageCount);
                response.CurrentPageIndex = criteria.CurrentPage;
                return response;
            }
        }

        public OnlineShop.Models.ProductReponse GetbyId(int ProductId)
        {
            using (ShopDbContext context = new ShopDbContext())
            {
                var product = context.Product.Include("Category").Include("ProductStock").Include("ProductStock.Colour").Include("ProductStock.Size").Where(x => x.Id == ProductId && x.IsDeleted == false && x.IsActive == true).ToList();
                OnlineShop.Models.ProductReponse response = new Models.ProductReponse();
                response = MapResponse(product);

                return response;
            }
        }

        public OnlineShop.Models.ColorsSizes GetColorsSizes(int productId)
        {
            using (ShopDbContext context = new ShopDbContext())
            {
                var colorsSizes = context.ProductStock.Include(z => z.Colour).Include(z => z.Size).Where(x => x.ProductId == productId).ToList();
                var sizes = colorsSizes.Select(x => x.Size).ToList();
                var colors = colorsSizes.Select(x => x.Colour).ToList();

                OnlineShop.Models.ColorsSizes model = new Models.ColorsSizes();
                model.ProductId = productId;
                model.Colours = MapColourToModel(colors);
                model.Sizes = MapSizeToModel(sizes);

                return model;
            }
        }

        public int GetProductStockbyColorSize(int productId, int qty)
        {
            using (ShopDbContext context = new ShopDbContext())
            {
                var productStock = context.ProductStock.Where(x => x.ProductId == productId).FirstOrDefault();
                if (productStock != null)
                {
                    if (productStock.StockCount >= qty)
                    {
                        return productStock.Id;
                    }
                    return -1;
                }
                return 0;
            }
        }

        //public int GetProductStockbyColorSize(int productId, int sizeId, int colorId, int qty)
        //{
        //    using (ShopDbContext context = new ShopDbContext())
        //    {
        //        var productStock = context.ProductStock.Where(x => x.ProductId == productId && x.ColourId == colorId && x.SizeID == sizeId).FirstOrDefault();
        //        if (productStock != null)
        //        {
        //            if (productStock.StockCount >= qty)
        //            {
        //                return productStock.Id;
        //            }
        //            return -1;
        //        }
        //        return 0;
        //    }
        //}

        public int AddProduct(Product product)
        {
            using (ShopDbContext context = new ShopDbContext())
            {
                var addedProduct = context.Product.Add(product);
                context.SaveChanges();

                return addedProduct.Id;
            }
        }

        public List<Models.Category> GetCategories()
        {
            using (ShopDbContext context = new ShopDbContext())
            {
                return MapCategoryToModel(context.Category.Where(x => x.IsDeleted == false).OrderBy(x => x.DisplayOrder).ToList());
            }
        }

        public List<Models.GenderTag> GetGenderTags()
        {
            List<Models.GenderTag> list = new List<Models.GenderTag>();
            using (ShopDbContext context = new ShopDbContext())
            {
                var tags = context.GenderTag.ToList();
                tags.ForEach(x =>
                {
                    list.Add(new Models.GenderTag
                    {
                        GenderName = x.Name,
                        GenderId = x.Id
                    });
                });
            }
            return list;
        }

        public OnlineShop.Models.ProductStockRespose ConfigureProduct(int organisationId, int productid)
        {
            OnlineShop.Models.ProductStockRespose respose = new Models.ProductStockRespose();
            using (ShopDbContext context = new ShopDbContext())
            {
                var colours = context.Colour.ToList();
                var sizes = context.Size.ToList();
                var categories = context.Category.Where(x => x.IsDeleted == false).ToList();
                var product = context.Product
                    .Include(e => e.Category)
                    .Include(e => e.ProductStock)
                    .Include("ProductStock.Colour")
                    .Include("ProductStock.Size")
                    .Where(x => x.Id == productid && x.OrganisationId == organisationId && x.IsDeleted == false).FirstOrDefault();

                respose.Categories = MapCategoryToModel(categories);
                respose.Colours = MapColourToModel(colours);
                respose.Sizes = MapSizeToModel(sizes);
                respose.Product = MapProductToModel(product);
                respose.ProductStock = MapProductStockToModel(product);
                respose.Categories = respose.Categories.Where(x => x.ParentCategoryId > 0).ToList();
            }
            return respose;
        }

        public Models.CategoriesSizeColoursFilter GetCategoriesSizeColoursFilter(int parentcategoryId = default(int), bool main = false)
        {
            using (ShopDbContext context = new ShopDbContext())
            {
                var colours = context.Colour.ToList();
                var sizes = context.Size.ToList();
                List<Models.Category> relatedCategories = new List<Models.Category>();
                var categories = context.Category.Where(x => x.IsDeleted == false && x.ParentCategoryId != 0).ToList();
                if (parentcategoryId > 0 && main)
                {
                    relatedCategories = MapCategoryToModel(categories.Where(x => x.ParentCategoryId == parentcategoryId).ToList());
                    categories = categories.Where(x => x.ParentCategoryId != parentcategoryId).OrderByDescending(x => x.ParentCategoryId).ToList();
                }
                Models.CategoriesSizeColoursFilter filterModel = new Models.CategoriesSizeColoursFilter
                {
                    Categories = MapCategoryToModel(categories),
                    Colours = MapColourToModel(colours),
                    Sizes = MapSizeToModel(sizes),
                    RetlatedCategories = relatedCategories
                };
                return filterModel;
            }
        }

        public void AddProductStock(DomainEntities.ProductStock stock)
        {
            using (ShopDbContext context = new ShopDbContext())
            {
                var productStock = context.ProductStock.Where(x => x.ProductId == stock.ProductId && x.ColourId == stock.ColourId && x.SizeID == stock.SizeID && x.IsDeleted == false).FirstOrDefault();
                if (productStock != null)
                {
                    throw new Exception("this combination already exist please try to update stock only ");
                }
                context.ProductStock.Add(stock);
                context.SaveChanges();
            }
        }

        public void UpdateStock(OnlineShop.Models.ProductStock stock)
        {
            using (ShopDbContext context = new ShopDbContext())
            {
                var stockToUpdate = context.ProductStock.Where(x => x.Id == stock.ProductStockId).FirstOrDefault();

                stockToUpdate.ColourId = stock.ColourId;
                stockToUpdate.CurrentPrice = stock.CurrentPrice;
                stockToUpdate.IsFeatured = stock.IsFeatured;
                stockToUpdate.IsActive = true;
                stockToUpdate.OnSale = stock.OnSale;
                stockToUpdate.ProductId = stock.ProductId;
                stockToUpdate.OldPrice = stock.OldPrice;
                stockToUpdate.SizeID = stock.SizeID;
                stockToUpdate.TopRated = stock.TopRated;
                stockToUpdate.StockCount = stock.StockCount;
                stockToUpdate.CurrencySymbol = CommonConstants.Constants.Pound;
                context.SaveChanges();
            }
        }

        public void DeleteStock(int organisationId, int productStockId)
        {
            using (ShopDbContext context = new ShopDbContext())
            {
                var stocktoRemove = context.ProductStock.Include(s => s.Product).Where(x => x.Id == productStockId).FirstOrDefault();
                if (stocktoRemove.Product.OrganisationId == organisationId)
                {
                    if (!context.OrderDetail.Where(x => x.ProductStockId == productStockId).Any())
                    {
                        context.ProductStock.Remove(stocktoRemove);
                        context.SaveChanges();
                    }
                    else
                    {
                        stocktoRemove.IsDeleted = true;
                        context.SaveChanges();
                    }
                }
            }
        }

        public void UpdateProduct(Models.Product product)
        {
            using (ShopDbContext context = new ShopDbContext())
            {
                var prod = context.Product.Where(x => x.Id == product.ProductId).FirstOrDefault();

                prod.CategoryId = product.CategoryId;
                prod.Detail = product.Detail;
                prod.Name = product.Name;
                prod.OrganisationId = product.OrganisationId;
                prod.Id = product.ProductId;
                prod.ShortDescription = product.ShortDescription;

                context.SaveChanges();
            }
        }

        public void MarkProductActive(int productId)
        {
            using (ShopDbContext context = new ShopDbContext())
            {
                var prod = context.Product.Where(x => x.Id == productId).FirstOrDefault();

                prod.IsActive = true;
                context.SaveChanges();
            }
        }

        public void MarkProduct_DeActive(int productId)
        {
            using (ShopDbContext context = new ShopDbContext())
            {
                var prod = context.Product.Where(x => x.Id == productId).FirstOrDefault();

                prod.IsActive = false;
                context.SaveChanges();
            }
        }

        public void ReduceProductStock(List<Models.MiniCartModel> items)
        {
            using (ShopDbContext context = new ShopDbContext())
            {
                items.ForEach(x =>
                {
                    var item = context.ProductStock.Where(v => v.Id == x.ProductStockId).FirstOrDefault();
                    item.StockCount = item.StockCount - x.Quantity;
                    context.SaveChanges();
                });
            }
        }

        public void ReAddProductStock(List<Models.MiniCartModel> items)
        {
            using (ShopDbContext context = new ShopDbContext())
            {
                items.ForEach(x =>
                {
                    var item = context.ProductStock.Where(v => v.Id == x.ProductStockId).FirstOrDefault();
                    item.StockCount = item.StockCount + x.Quantity;
                    context.SaveChanges();
                });
            }
        }

        public List<OnlineShop.Models.Colour> GetColors(int sizeid, int productId)
        {
            using (ShopDbContext context = new ShopDbContext())
            {
                var stock = context.ProductStock.Include(x => x.Colour).Where(x => x.SizeID == sizeid && x.ProductId == productId).ToList();
                var colors = stock.Select(x => x.Colour).ToList();
                return MapColourToModel(colors);
            }
        }

        public OnlineShop.Models.ProductReponse GetProductsForLandingPage()
        {
            using (ShopDbContext context = new ShopDbContext())
            {
                List<Product> products = new List<Product>();

                products = (from product in context.Product
                          .Include("ProductStock")
                          .Include("ProductStock.Colour")
                          .Include("ProductStock.Size")
                          .Include("Category")
                            where product.IsDeleted == false && product.IsActive == true
                            select product).OrderByDescending(x => x.CreationTime).ToList();

                var reponse = MapResponse(products);

                return reponse;
            }
        }

        #region Private
        private List<OnlineShop.Models.ProductStock> MapProductStockToModel(DomainEntities.Product product)
        {
            List<OnlineShop.Models.ProductStock> mappedList = new List<Models.ProductStock>();
            product.ProductStock.Where(x => x.IsDeleted == false).ToList().ForEach(x =>
                {
                    mappedList.Add(new Models.ProductStock
                    {
                        ColourId = x.ColourId,
                        ColourName = x.Colour.Name,
                        CurrencySymbol = x.CurrencySymbol,
                        CurrentPrice = x.CurrentPrice,
                        ProductStockId = x.Id,
                        IsFeatured = x.IsFeatured,
                        OldPrice = x.OldPrice,
                        OnSale = x.OnSale,
                        StockCount = x.StockCount,
                        SizeID = x.SizeID,
                        SizeName = x.Size.Name,
                        TopRated = x.TopRated,
                        ProductId = x.ProductId,
                        ColourCode = x.Colour.Code
                    });
                });
            return mappedList;
        }

        private OnlineShop.Models.ProductStock MapProductStockListToModel(DomainEntities.ProductStock x)
        {

            return new Models.ProductStock
            {
                ColourId = x.ColourId,
                ColourName = x.Colour.Name,
                CurrencySymbol = x.CurrencySymbol,
                CurrentPrice = x.CurrentPrice,
                ProductStockId = x.Id,
                IsFeatured = x.IsFeatured,
                OldPrice = x.OldPrice,
                OnSale = x.OnSale,
                StockCount = x.StockCount,
                SizeID = x.SizeID,
                SizeName = x.Size.Name,
                TopRated = x.TopRated,
                ProductId = x.ProductId,
                ColourCode = x.Colour.Code
            };
        }

        private OnlineShop.Models.Product MapProductToModel(DomainEntities.Product product)
        {
            OnlineShop.Models.Product prod = new Models.Product();
            prod.CategoryId = product.CategoryId;
            prod.Name = product.Name;
            prod.Detail = product.Detail;
            prod.isActive = product.IsActive;
            prod.OrganisationId = product.OrganisationId;
            prod.CategoryName = product.Category.Name;
            prod.ProductId = product.Id;
            prod.isActive = product.IsActive;
            prod.ShortDescription = product.ShortDescription;
            return prod;
        }

        private List<OnlineShop.Models.Category> MapCategoryToModel(List<DomainEntities.Category> categories)
        {
            List<OnlineShop.Models.Category> mappedList = new List<Models.Category>();
            categories.ForEach(x =>
            {
                mappedList.Add(new Models.Category
                {
                    CategoryId = x.Id,
                    CategoryName = x.Name,
                    ParentCategoryId = x.ParentCategoryId
                });
            });
            return mappedList;
        }

        private List<OnlineShop.Models.Colour> MapColourToModel(List<DomainEntities.Colour> colours)
        {
            List<OnlineShop.Models.Colour> mappedList = new List<Models.Colour>();
            colours.ForEach(x =>
            {
                mappedList.Add(new Models.Colour
                {
                    Name = x.Name,
                    ColourId = x.Id,
                    Code = x.Code
                });
            });
            return mappedList;
        }

        private List<OnlineShop.Models.Size> MapSizeToModel(List<DomainEntities.Size> Sizes)
        {
            List<OnlineShop.Models.Size> mappedList = new List<Models.Size>();
            Sizes.ForEach(x =>
            {
                mappedList.Add(new Models.Size
                {
                    Name = x.Name,
                    SizeId = x.Id
                });
            });
            return mappedList;
        }

        private Models.ProductReponse MapResponse(List<Product> products)
        {
            OnlineShop.Models.ProductReponse reponse = new OnlineShop.Models.ProductReponse();
            products.ForEach(x =>
            {

                var model = new Models.Product
                {
                    CategoryId = x.CategoryId,
                    CategoryName = x.Category.Name,
                    ParentCategoryId = x.Category.ParentCategoryId,
                    isActive = x.IsActive,
                    Name = x.Name,
                    Detail = x.Detail,
                    OrganisationId = x.OrganisationId,
                    ShortDescription = x.ShortDescription,
                    ProductId = x.Id
                };

                List<OnlineShop.Models.ProductStock> stockList = new List<OnlineShop.Models.ProductStock>();
                x.ProductStock.Where(d => d.IsDeleted == false).ToList().ForEach(p =>
                {
                    OnlineShop.Models.ProductStock stockModel = new Models.ProductStock
                    {
                        ProductStockId = p.Id,
                        IsFeatured = p.IsFeatured,
                        OnSale = p.OnSale,
                        TopRated = p.TopRated,
                        ColourCode = p.Colour.Code,
                        ColourId = p.ColourId,
                        ProductId = p.ProductId,
                        ColourName = p.Colour.Name,
                        CurrencySymbol = p.CurrencySymbol,
                        CurrentPrice = p.CurrentPrice,
                        OldPrice = p.OldPrice,
                        SizeID = p.SizeID,
                        SizeName = p.Size.Name,
                        StockCount = p.StockCount
                    };
                    stockList.Add(stockModel);
                });
                model.ProductStock.AddRange(stockList);
                reponse.Products.Add(model);
            });
            return reponse;
        }

        #endregion
    }
}