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
        IRepository<OrderItem> orderItemContext;
        public OrderService(IRepository<Order> OrderContext, IRepository<OrderItem> OrderItemContext)
        {
            this.orderContext = OrderContext;
            this.orderItemContext = OrderItemContext;
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
        public List<OrderItem> GetOrderItemList(string Id)
        {
            List<OrderItem> orderItems = orderItemContext.Collection().ToList();
            List<OrderItem> orderItemsOut = orderItems.Where(o=>o.OrderId.CompareTo(Id)<2).ToList();
            return orderItemsOut;
        }

        public Order GetOrder(string Id) {
            return orderContext.Find(Id);
        }

        public Order GetOrderFromOrderNumber(string Id)
        {
            return orderContext.Collection().Where(o => o.OrderNumber.CompareTo(Id) < 1).FirstOrDefault();
        }

        public void UpdateOrder(Order updatedOrder) {
            orderContext.Update(updatedOrder);
            orderContext.Commit();
        }

    }
}
