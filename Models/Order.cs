namespace Assistant.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public string WithdrawalMethod { get; set; }
        public string PaymentMethod { get; set; }
        public decimal OrderPrice { get; set; }
        public List<OrderItem> Items { get; set; }
    }
}
