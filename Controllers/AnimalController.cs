using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TameShop.Data;
using TameShop.Models;
using TameShop.ViewModels;
namespace TameShop.Controllers
{
    [ApiController]
    [Route("api/animals")]
    public class AnimalController : Controller
    {
        private readonly TameShopDbContext _context;
        public AnimalController(TameShopDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAnimals()
        {
            var animals = await _context.Animals.ToListAsync();
            return Ok(animals);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAnimal(int id)
        {
            var animal = await _context.Animals.FirstOrDefaultAsync(a => a.Id == id);
            if (animal == null)
            {
                return NotFound("Animal not found.");
            }
            return Ok(animal);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        public async Task<IActionResult> AddAnimal([FromBody] AnimalDTO animalDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid animal data.");
            }
            var animal = new Animal
            {
                Name = animalDTO.Name,
                Price = animalDTO.Price,
                Description = animalDTO.Description,
                ImageUrl = animalDTO.ImageUrl,
                Category = animalDTO.Category
            };
            await _context.Animals.AddAsync(animal);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAnimal), new { id = animal.Id }, animal);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAnimal(int id, [FromBody] AnimalDTO animalDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid animal data.");
            }
            var existingAnimal = await _context.Animals.FirstOrDefaultAsync(a => a.Id == id);
            if (existingAnimal == null)
            {
                return NotFound("Animal not found.");
            }
            existingAnimal.Name = animalDTO.Name;
            existingAnimal.Price = animalDTO.Price;
            existingAnimal.Description = animalDTO.Description;
            existingAnimal.ImageUrl = animalDTO.ImageUrl;
            existingAnimal.Category = animalDTO.Category;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnimal(int id)
        {
            var animal = await _context.Animals.FirstOrDefaultAsync(a => a.Id == id);
            if (animal == null)
            {
                return NotFound("Animal not found.");
            }
            _context.Animals.Remove(animal);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("category")]
        public async Task<IActionResult> GetAnimalsByCategory([FromQuery] string category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                return BadRequest("Category cannot be empty.");
            }
            var animals = await _context.Animals.Where(a => a.Category == category).ToListAsync();
            if (animals.Count == 0)
            {
                return NotFound("No animals found in this category.");
            }
            return Ok(animals);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchAnimals([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Name cannot be empty.");
            }

            var animals = await _context.Animals
                .Where(a => a.Name.ToLower().Contains(name.ToLower()))
                .ToListAsync();

            if (!animals.Any())
            {
                return NotFound("No animals found with this name.");
            }

            return Ok(animals);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> FilterByPrice(
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] string sortOrder = "asc",
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10) // "asc" or "desc"
        {
            var query = _context.Animals.AsQueryable();

            if (minPrice.HasValue && maxPrice.HasValue && minPrice > maxPrice)
            {
                return BadRequest("minPrice cannot be greater than maxPrice.");
            }

            if (minPrice.HasValue)
            {
                query = query.Where(a => a.Price >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(a => a.Price <= maxPrice.Value);
            }

            query = sortOrder.ToLower() switch
            {
                "desc" => query.OrderByDescending(a => a.Price),
                _ => query.OrderBy(a => a.Price)
            };

            var animals = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (!animals.Any())
            {
                return NotFound("No animals found in this price range.");
            }

            return Ok(animals);
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetAnimalsPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return BadRequest("Page and pageSize must be greater than 0.");
            }

            var totalCount = await _context.Animals.CountAsync();
            var animals = await _context.Animals
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                Data = animals
            };

            return Ok(result);
        }
    }
}
