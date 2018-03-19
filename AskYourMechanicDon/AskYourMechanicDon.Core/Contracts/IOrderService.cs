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
        List<OrderItem> GetOrderItemListFromOrderNumber(string Id);
        Order GetOrder(string Id);
        Order GetOrderFromOrderNumber(string Id);
        Boolean IsOrderPayPalTranxFound(string txnId);
        void UpdateOrder(Order updatedOrder);
        decimal OrderTotalFromOrderNumber(string Id);
    }
}
