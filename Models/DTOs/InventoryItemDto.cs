namespace LogiTrack.Models.DTOs;

public class InventoryItemDto
{
    public int ItemId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Location { get; set; } = string.Empty;
}

public class InventoryItemCreateDto
{
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Location { get; set; } = string.Empty;
}