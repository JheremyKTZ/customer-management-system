using Stark.Common.Models;
using Stark.Generators.Interfaces;
using System.Collections.Generic;

namespace Stark.BL.Repositories
{
    public class OrderItemRepository
    {
        private List<Order> _orders = new List<Order>();
        private List<OrderItem> _orderItems = new List<OrderItem>();

        public OrderItemRepository()
        { }

        public OrderItemRepository(int customers, int products, int orders)
        {
            (_orders, _, _) = PartialBuilder.CreateOrders(
                customers,
                products,
                orders);
        }

        public OrderItem Retrieve(int orderItemId, int orderId)
        {
            OrderItem item = new OrderItem(orderItemId);

            if (orderItemId == 101)
            {
                item.ProductId = 1;
                item.PurchasePrice = (decimal?)25.25;
                item.Quantity = 5;
            }
            return item;
        }

        public bool Save()
        {
            return true;
        }
    }
}
