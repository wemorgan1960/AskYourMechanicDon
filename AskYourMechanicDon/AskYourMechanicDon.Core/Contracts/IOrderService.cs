using AskYourMechanicDon.Core.Models;
using AskYourMechanicDon.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskYourMechanicDon.Core.Contracts
{
    public interface IOrderService
    {
        void CreateOrder(Order baseOrder, List<BasketItemViewModel> basketItems);
        List<Order> GetOrderList();
        List<OrderItem> GetOrderItemList(string Id);
        Order GetOrder(string Id);
        Order GetOrderFromOrderNumber(string Id);
        void UpdateOrder(Order updatedOrder);
    }
}
