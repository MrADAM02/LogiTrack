using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LogiTrack.Data;
using LogiTrack.Models;
using LogiTrack.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;

namespace LogiTrack.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly LogiTrackContext _context;
        private readonly IMemoryCache _cache;

        public OrderController(LogiTrackContext context,
                               IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        // GET: api/orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
        {
            // Try to get cached value
            if (!_cache.TryGetValue("orders_cache", out List<OrderDto>? dtos))
            {
                var orders = await _context.Orders
                                           .AsNoTracking()
                                           .Include(o => o.Items)
                                           .ToListAsync();

                dtos = orders.Select(o => new OrderDto
                {
                    OrderId = o.OrderId,
                    CustomerName = o.CustomerName,
                    DatePlaced = o.DatePlaced,
                    Items = o.Items.Select(i => new InventoryItemDto
                    {
                        ItemId = i.ItemId,
                        Name = i.Name,
                        Quantity = i.Quantity,
                        Location = i.Location
                    }).ToList()
                }).ToList();

                _cache.Set("orders_cache", dtos, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30),
                    SlidingExpiration = TimeSpan.FromSeconds(15)
                });
            }

            return Ok(dtos);
        }

        // GET: api/orders/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {

            string cacheKey = $"order_{id}";

            if (!_cache.TryGetValue(cacheKey, out OrderDto? dto))
            {
                dto = await _context.Orders
                    .AsNoTracking()
                    .Include(o => o.Items)
                    .Where(o => o.OrderId == id)
                    .Select(o => new OrderDto
                    {
                        OrderId = o.OrderId,
                        CustomerName = o.CustomerName,
                        DatePlaced = o.DatePlaced,
                        Items = o.Items.Select(i => new InventoryItemDto
                        {
                            ItemId = i.ItemId,
                            Name = i.Name,
                            Quantity = i.Quantity,
                            Location = i.Location
                        }).ToList()
                    })
                    .FirstOrDefaultAsync();

                if (dto == null)
                    return NotFound(new { message = $"Order {id} not found" });

                // Cache for 30 seconds
                _cache.Set(cacheKey, dto, TimeSpan.FromSeconds(30));
            }

            return Ok(dto);
        }

        // POST: api/orders
        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] OrderCreateDto dto)
        {
            var order = new Order
            {
                CustomerName = dto.CustomerName,
                DatePlaced = DateTime.Now,
                Items = dto.Items.Select(i => new InventoryItem
                {
                    Name = i.Name,
                    Quantity = i.Quantity,
                    Location = i.Location
                }).ToList()
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Clear cache
            _cache.Remove($"order_{order.OrderId}");
            _cache.Remove("orders_cache");

            var resultDto = new OrderDto
            {
                OrderId = order.OrderId,
                CustomerName = order.CustomerName,
                DatePlaced = order.DatePlaced,
                Items = order.Items.Select(i => new InventoryItemDto
                {
                    ItemId = i.ItemId,
                    Name = i.Name,
                    Quantity = i.Quantity,
                    Location = i.Location
                }).ToList()
            };

            return CreatedAtAction(nameof(GetOrder), new { id = order.OrderId }, resultDto);
        }

        // DELETE: api/orders/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders
                                      .Include(o => o.Items)
                                      .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null) return NotFound(new { message = $"Order {id} not found" });

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            // Clear cache
            _cache.Remove($"order_{id}");
            _cache.Remove("orders_cache");

            return NoContent();
        }
    }
}
