using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TameShop.Data;
using TameShop.Models;
namespace TameShop.Controllers
{
    [Route("[controller]")]
    public class AnimalController : Controller
    {
        private readonly UserDbContext _context;
        public AnimalController(UserDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAnimals()
        {
            var animals = _context.Animals.ToList();
            return Ok(animals);
        }

        [HttpGet("{id}")]
        public IActionResult GetAnimal(int id)
        {
            var animal = _context.Animals.FirstOrDefault(a => a.Id == id);
            if (animal == null)
            {
                return NotFound("Animal not found.");
            }
            return Ok(animal);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        public IActionResult AddAnimal([FromBody] Animal animal)
        {
            if (animal == null || string.IsNullOrWhiteSpace(animal.Name) || animal.Price <= 0)
            {
                return BadRequest("Invalid animal data.");
            }
            _context.Animals.Add(animal);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetAnimal), new { id = animal.Id }, animal);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPut("{id}")]
        public IActionResult UpdateAnimal(int id, [FromBody] Animal animal)
        {
            if (animal == null || string.IsNullOrWhiteSpace(animal.Name) || animal.Price <= 0)
            {
                return BadRequest("Invalid animal data.");
            }
            var existingAnimal = _context.Animals.FirstOrDefault(a => a.Id == id);
            if (existingAnimal == null)
            {
                return NotFound("Animal not found.");
            }
            existingAnimal.Name = animal.Name;
            existingAnimal.Price = animal.Price;
            existingAnimal.Description = animal.Description;
            existingAnimal.ImageUrl = animal.ImageUrl;
            existingAnimal.Category = animal.Category;
            _context.SaveChanges();
            return NoContent();
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpDelete("{id}")]
        public IActionResult DeleteAnimal(int id)
        {
            var animal = _context.Animals.FirstOrDefault(a => a.Id == id);
            if (animal == null)
            {
                return NotFound("Animal not found.");
            }
            _context.Animals.Remove(animal);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpGet("category")]
        public IActionResult GetAnimalsByCategory([FromQuery] string category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                return BadRequest("Category cannot be empty.");
            }
            var animals = _context.Animals.Where(a => a.Category == category).ToList();
            if (animals.Count == 0)
            {
                return NotFound("No animals found in this category.");
            }
            return Ok(animals);
        }
    }
}
