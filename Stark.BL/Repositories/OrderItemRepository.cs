using Stark.Common.Models;

namespace Stark.BL.Repositories
{
    public class OrderItemRepository
    {
        public OrderItem Retrieve(int orderItemId)
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
