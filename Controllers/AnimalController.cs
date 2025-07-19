using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TameShop.Data;
using TameShop.Models;
using TameShop.ViewModels;
namespace TameShop.Controllers
{
    [ApiController]
    [Route("[controller]")]
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
    }
}
