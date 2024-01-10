using Stark.Common.Models;
using Stark.Generators.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Stark.BL.Repositories
{
    public class OrderRepository : IRepository<Order>
    {
        private List<Order> _orders = new List<Order>();

        public OrderRepository(List<Order> orders)
        {
            _orders = orders;
        }

        public OrderRepository(int ordersQuantity)
        {
            if (_orders.Any())
            {
                return;
            }

            (_orders, _, _) = PartialBuilder.CreateOrders(
                ordersQuantity,
                ordersQuantity,
                ordersQuantity);
        }

        public Order Retrieve(int orderId)
        {
            return _orders
                .FirstOrDefault(o => o.OrderId == orderId) ?? new Order();
        }

        public IList<Order> RetrieveAll() => _orders;

        public bool Delete(int orderId)
        {
            var foundedOrder = _orders.FindIndex(a => a.OrderId == orderId);
            if (foundedOrder == -1)
            {
                return false;
            }

            _orders.RemoveAt(foundedOrder);
            return true;
        }

        public bool Save(Order order)
        {
            if (!order.HasChanges)
            {
                return false;
            }

            if (!order.IsValid)
            {
                return false;
            }

            if (order.IsNew)
            {
                _orders.Add(order);
                return true;
            }

            var orderIndex = _orders
                .FindIndex(a => a.OrderId == order.OrderId);
            if (orderIndex == -1)
            {
                return false;
            }

            _orders[orderIndex] = order;
            return true;
        }
    }
}
