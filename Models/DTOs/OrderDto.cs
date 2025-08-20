using LogiTrack.Models.DTOs;

public class OrderDto
{
    public int OrderId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public DateTime DatePlaced { get; set; }
    public List<InventoryItemDto> Items { get; set; } = new();
}

public class OrderCreateDto
{
    public string CustomerName { get; set; } = string.Empty;
    public List<InventoryItemCreateDto> Items { get; set; } = new();
}