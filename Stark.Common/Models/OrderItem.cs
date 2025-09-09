namespace Stark.Common.Models
{
    public record OrderItem
    {
        public OrderItem()
        {
        }

        public OrderItem(int orderItemId, int orderId)
        {
            OrderItemId = orderItemId;
            OrderId = orderId;
        }

        public int OrderId { get; private set; }
        public int OrderItemId { get; private set; }
        public int ProductId { get; set; }
        public decimal? PurchasePrice { get; set; }
        public int Quantity { get; set; }

        public bool Validate()
        {
            bool isValid = Quantity <= 0 || ProductId <= 0 || PurchasePrice == null ? false : true;
            return isValid;
        }

        // Navegaciones EF Core
        public Order Order { get; set; }
        public Product Product { get; set; }
    }
}
