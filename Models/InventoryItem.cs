using System.ComponentModel.DataAnnotations;

namespace LogiTrack.Models
{
    public class InventoryItem
    {
        [Key]
        public int ItemId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int Quantity { get; set; }

        [Required]
        public string Location { get; set; } = string.Empty;

        // Foreign key for Order
        public int? OrderId { get; set; }
        public Order? Order { get; set; }

        public string DisplayInfo()
        {
            return $"Item: {Name} | Quantity: {Quantity} | Location: {Location}";
        }
    }
}
