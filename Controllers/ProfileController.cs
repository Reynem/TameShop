using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using TameShop.Data;
using TameShop.Models;

namespace TameShop.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProfileController : ControllerBase
    {
        private ApplicationContext db;
        public ProfileController()
        {
            db = new();
            db.Database.EnsureCreated();
        }


        [HttpGet]
        public ActionResult Index()
        {
            List<Animal> animals = db.Animals.ToList();
            return Ok(animals);
        }

        [HttpPost]
        public ActionResult Create([FromBody] Animal animal)
        {
            if (animal == null || string.IsNullOrEmpty(animal.Name))
            {
                return BadRequest("Invalid animal data.");
            }
            db.Animals.Add(animal);
            db.SaveChanges();

            return CreatedAtAction(nameof(Index), animal);
        }

        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            var animal = db.Animals.Find(id);
            if (animal == null)
            {
                return NotFound();
            }
            return Ok(animal);
        }

        [HttpPut("{id}")]
        public ActionResult Update(int id, [FromBody] Animal animal)
        {
            if (animal == null || string.IsNullOrEmpty(animal.Name))
            {
                return BadRequest("Invalid animal data.");
            }
            var existingAnimal = db.Animals.Find(id);
            if (existingAnimal == null)
            {
                return NotFound();
            }
            existingAnimal.Name = animal.Name;
            existingAnimal.Description = animal.Description;
            existingAnimal.BirthTime = animal.BirthTime;
            existingAnimal.ProfilePicture = animal.ProfilePicture;
            db.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var animal = db.Animals.Find(id);
            if (animal == null)
            {
                return NotFound();
            }
            db.Animals.Remove(animal);
            db.SaveChanges();
            return NoContent();
        }
        


    }
}
