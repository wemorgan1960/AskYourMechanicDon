using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AskYourMechanicDon.Core.Contracts;
using AskYourMechanicDon.Core.Models;
using AskYourMechanicDon.Core.ViewModels;

namespace AskYourMechanicDon.Services
{
    public class BasketService : IBasketService
    {
        IRepository<Product> productContext;
        IRepository<Basket> basketContext;
        IRepository<BasketItem> basketItemContext;


        public const string BasketSessionName = "eAskYourMechanicDonBasket";

        public BasketService(IRepository<Product> product, IRepository<Basket> basket, IRepository<BasketItem> basketItem)
        {
            this.basketContext = basket;
            this.productContext = product;
            this.basketItemContext = basketItem;
        }

        private Basket GetBasket(HttpContextBase httpContext, bool createIfNull)
        {
            HttpCookie cookie = httpContext.Request.Cookies.Get(BasketSessionName);

            Basket basket = new Basket();

            if (cookie != null)
            {
                string basketId = cookie.Value;
                if (!string.IsNullOrEmpty(basketId))
                {
                    basket = basketContext.Find(basketId);
                    if (basket == null)
                    {
                        basket = CreateNewBasket(httpContext);
                    }
                }
                else
                {
                    if (createIfNull)
                    {
                        basket = CreateNewBasket(httpContext);
                    }
                }
            }
            else
            {
                if (createIfNull)
                {
                    basket = CreateNewBasket(httpContext);
                }
            }
            return basket;
        }


        private Basket CreateNewBasket(HttpContextBase httpContext)
        {
            Basket basket = new Basket();
            basketContext.Insert(basket);
            basketContext.Commit();

            HttpCookie cookie = new HttpCookie(BasketSessionName)
            {
                Value = basket.Id,
                //Expires = DateTime.Now.AddDays(1)
                Expires = DateTime.Now.AddMinutes(4)
            };
            httpContext.Response.Cookies.Add(cookie);

            return basket;
        }
        public void AddToBasket(HttpContextBase httpContext, string productId, string vin, string question)
        {
            Basket basket = GetBasket(httpContext, true);
            BasketItem item = basket.BasketItems.FirstOrDefault(i => i.ProductId == productId);
            if(item==null)
            {
                item = new BasketItem()
                {
                    BasketId = basket.Id,
                    ProductId = productId,
                    Quanity = 1,
                    Vin = vin,
                    Question = question
                };
                basket.BasketItems.Add(item);
            }
            else
            {
                if (item.Question == question)
                {
                    item.Quanity = 1;
                }
                else
                {
                    BasketItem newitem = new BasketItem()
                    {
                        BasketId = basket.Id,
                        ProductId = productId,
                        Quanity = 1,
                        Vin = vin,
                        Question = question
                    };
                    basket.BasketItems.Add(newitem);
                }
            }

            basketContext.Commit();
        }
        public void RemoveFromBasket(HttpContextBase httpContext, string itemId)
        {
            Basket basket = GetBasket(httpContext, true);
            BasketItem item = basket.BasketItems.FirstOrDefault(i => i.Id == itemId);

            if (item != null)
            {
                basket.BasketItems.Remove(item);
                basketContext.Commit();
            }
        }
        public List<BasketItemViewModel> GetBasketItems(HttpContextBase httpContext)
        {
            Basket basket = GetBasket(httpContext, false);

            if (basket != null)
            {
                List<BasketItem> basketItems = basketItemContext.Collection().Where(b=> b.BasketId == basket.Id).ToList();
                List<Product> products = productContext.Collection().ToList();


                var results = (from b in basketItems
                               join p in productContext.Collection() 
                                    on b.ProductId equals p.Id
                               where b.BasketId == basket.Id
                               select new BasketItemViewModel()
                               {
                                   Id = b.Id,
                                   Quanity = b.Quanity,
                                   ProductName = p.Name,
                                   Vin = b.Vin,
                                   Question = b.Question,
                                   Image = p.Image,
                                   Price = p.Price
                               }).ToList();
                return results;
            }
            else
            {
                return new List<BasketItemViewModel>();
            }
        }
        public BasketSummaryViewModel GetBasketSummary(HttpContextBase httpContext)
        {
            Basket basket = GetBasket(httpContext, false);
            BasketSummaryViewModel model = new BasketSummaryViewModel(0, 0);

            if (basket != null)
            {
                int? basketCount = (from item in basket.BasketItems
                                    select item.Quanity).Sum();

                decimal? basketTotal = (from item in basket.BasketItems
                                        join p in productContext.Collection() on item.ProductId equals p.Id
                                        select item.Quanity * p.Price).Sum();
                model.BasketCount = basketCount ?? 0;
                model.BasketTotal = basketTotal ?? decimal.Zero;

                return model;
            }
            else
            {
                return model;
            }
        }
        public void ClearBasket(HttpContextBase httpContext)
        {
            Basket basket = GetBasket(httpContext, false);
            basket.BasketItems.Clear();
            basketContext.Commit();
        }
    }
}
