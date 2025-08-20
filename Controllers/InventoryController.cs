using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LogiTrack.Data;
using LogiTrack.Models;
using LogiTrack.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace LogiTrack.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly LogiTrackContext _context;
        private readonly IMemoryCache _cache;

        public InventoryController(LogiTrackContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        // GET: api/inventory
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetInventory()
        {
            var items = await LoadInventoryAsync();
            return Ok(items);
        }

        // POST: api/inventory
        [HttpPost]
        public async Task<ActionResult<InventoryItemDto>> AddItem([FromBody] InventoryItemCreateDto dto)
        {
            var item = new InventoryItem
            {
                Name = dto.Name,
                Quantity = dto.Quantity,
                Location = dto.Location
            };

            _context.InventoryItems.Add(item);
            await _context.SaveChangesAsync();

            var resultDto = new InventoryItemDto
            {
                ItemId = item.ItemId,
                Name = item.Name,
                Quantity = item.Quantity,
                Location = item.Location
            };

            return CreatedAtAction(nameof(GetInventory), new { id = item.ItemId }, resultDto);
        }

        // DELETE: api/inventory/{id}
        [Authorize(Roles = "Manager")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var item = await _context.InventoryItems.FindAsync(id);
            if (item == null) return NotFound(new { message = $"Item {id} not found" });

            _context.InventoryItems.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpGet("timed")]
        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetInventoryTimed()
        {
            var stopwatch = Stopwatch.StartNew();

            var items = await LoadInventoryAsync();

            stopwatch.Stop();
            Console.WriteLine($"Query + Cache retrieval took: {stopwatch.ElapsedMilliseconds} ms");

            return Ok(items);
        }

        private async Task<IEnumerable<InventoryItemDto>> LoadInventoryAsync()
        {
            const string cacheKey = "inventory_list";

            if (!_cache.TryGetValue(cacheKey, out List<InventoryItem>? inventory))
            {
                inventory = await _context.InventoryItems
                                          .AsNoTracking()
                                          .ToListAsync();

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(30));

                _cache.Set(cacheKey, inventory, cacheOptions);
            }

            return inventory!.Select(i => new InventoryItemDto
            {
                ItemId = i.ItemId,
                Name = i.Name,
                Quantity = i.Quantity,
                Location = i.Location
            });
        }


    }
}
