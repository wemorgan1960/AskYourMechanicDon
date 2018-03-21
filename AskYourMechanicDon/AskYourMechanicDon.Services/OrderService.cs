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

            var results = (from b in orderItems
                           where b.OrderId== Id
                           select new OrderItem()
                           {
                               Id = b.Id,
                               ProductId=b.ProductId,
                               ProductName = b.ProductName,
                               Price = b.Price,
                               Quanity = b.Quanity,
                               Vin = b.Vin,
                               Question = b.Question,
                           }).ToList();

            return results;
        }

        public Order GetOrder(string Id) {
            return orderContext.Find(Id);
        }
        public List<OrderItem> GetOrderItemListFromOrderNumber(string Id)
        {
            List<Order> orders = orderContext.Collection().ToList();
            List<OrderItem> orderItems = orderItemContext.Collection().ToList();

            var results = (from b in orderItems
                           join o in orders on b.OrderId equals o.Id
                           where o.OrderNumber == Id
                           select new OrderItem()
                           {
                               Id = b.Id,
                               ProductId = b.ProductId,
                               ProductName = b.ProductName,
                               Price = b.Price,
                               Quanity = b.Quanity,
                               Vin = b.Vin,
                               Question = b.Question,
                               OrderId =b.OrderId
                           }).ToList();

            return results;
        }

        public decimal OrderTotalFromOrderNumber(string Id)
        {
            List<Order> orders = orderContext.Collection().ToList();
            List<OrderItem> orderItems = orderItemContext.Collection().ToList();

            var result = (from b in orderItems
                           join o in orders on b.OrderId equals o.Id
                           where o.OrderNumber == Id
                           select (b.Price*b.Quanity)  ).Sum();

            return result;
        }

        public Order GetOrderFromOrderNumber(string Id)
        {
            return orderContext.Collection().Where(o => o.OrderNumber.CompareTo(Id) ==0).FirstOrDefault();
            //return orderContext.Collection().Where(o => string.Compare(o.OrderNumber,Id,true)==0).SingleOrDefault();

            //var result = (from o in orderContext.Collection().ToList()
            //              where o.OrderNumber == Id
            //              select new Order()
            //              {
            //                  Id = o.Id,
            //                  CustomerUserId = o.CustomerUserId,
            //                  OrderNumber = o.OrderNumber,
            //                  InvoiceNumber = o.InvoiceNumber,
            //                  PayPalTxnId = o.PayPalTxnId,
            //                  AmountPaid = o.AmountPaid,
            //                  Currency = o.Currency,
            //                  PayPalPaidDate = o.PayPalPaidDate,
            //                  OrderStatusDate = o.OrderStatusDate,
            //                  OrderStatus = o.OrderStatus,
            //                  OrderItems = o.OrderItems
            //              });
            //return result.SingleOrDefault();
        }

        public Boolean IsOrderPayPalTranxFound(string txnId)
        {
            List<Order> orders = orderContext.Collection().ToList();
            var results = (from  o in orders
                           where o.PayPalTxnId == txnId
                           select o.Id
                           ).ToList();

            if(results.Count !=0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void UpdateOrder(Order updatedOrder) {
            orderContext.Update(updatedOrder);
            orderContext.Commit();
        }

    }
}
