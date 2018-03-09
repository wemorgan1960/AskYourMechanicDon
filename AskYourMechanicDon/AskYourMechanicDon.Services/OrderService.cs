using AskYourMechanicDon.Core.Contracts;
using AskYourMechanicDon.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AskYourMechanicDon.Core.ViewModels;

namespace AskYourMechanicDon.Services
{
    public class OrderService : IOrderService
    {
        IRepository<Order> orderContext;
        public OrderService(IRepository<Order> OrderContext) {
            this.orderContext = OrderContext;
        }

        public void CreateOrder(Order baseOrder, List<BasketItemViewModel> basketItems)
        {
            foreach (var item in basketItems) {
                baseOrder.OrderItems.Add(new OrderItem()
                {
                    ProductId = item.Id,
                    Price = item.Price,
                    ProductName = item.ProductName,
                    Quanity = item.Quanity,
                    Vin = item.Vin,
                    Question = item.Question,
                });
            }
            orderContext.Insert(baseOrder);
            orderContext.Commit();
        }

        public List<Order> GetOrderList() {
            return orderContext.Collection().ToList();
        }

        public Order GetOrder(string Id) {
            return orderContext.Find(Id);
        }

        public void UpdateOrder(Order updatedOrder) {
            orderContext.Update(updatedOrder);
            orderContext.Commit();
        }
    }
}
