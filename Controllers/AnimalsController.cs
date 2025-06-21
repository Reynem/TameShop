using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace TameShop.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AnimalsController : ControllerBase
    {
        private ApplicationContext db;
        public AnimalsController()
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
            if (animal == null || string.IsNullOrEmpty(animal.Name) || animal.Cost < 0)
            {
                return BadRequest("Invalid animal data.");
            }
            db.Animals.Add(animal);
            db.SaveChanges();

            return CreatedAtAction(nameof(Index), animal);
        }

    }
}
