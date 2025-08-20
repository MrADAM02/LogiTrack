using System.ComponentModel.DataAnnotations;

namespace LogiTrack.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        public DateTime DatePlaced { get; set; } = DateTime.Now;

        // One-to-many: One Order can have many Items
        public List<InventoryItem> Items { get; set; } = new();

        public void AddItem(InventoryItem item)
        {
            item.OrderId = OrderId; // ensure FK is set
            Items.Add(item);
        }

        public void RemoveItem(int itemId)
        {
            var item = Items.FirstOrDefault(i => i.ItemId == itemId);
            if (item != null) Items.Remove(item);
        }

        public string GetOrderSummary()
        {
            return $"Order #{OrderId} for {CustomerName} | Items: {Items.Count} | Placed: {DatePlaced:d}";
        }
    }
}
