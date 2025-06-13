using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace TameShop.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AnimalsController : ControllerBase
    {
        [HttpGet]
        public ActionResult Index()
        {
            var animals = new List<Animal>
            {
                new Animal { Id = 1, Name = "Dog", Cost = 100.0, Type = "Mammal", Description = "A friendly dog.", ImageData = null },
                new Animal { Id = 2, Name = "Cat", Cost = 80.0, Type = "Mammal", Description = "A cute cat.", ImageData = null },
                new Animal { Id = 3, Name = "Parrot", Cost = 50.0, Type = "Bird", Description = "A colorful parrot.", ImageData = null }
            };
            return Ok(animals);
        }
    }
}
