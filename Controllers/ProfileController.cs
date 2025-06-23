using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TameShop.Data;
using TameShop.Models;

namespace TameShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private ApplicationContext db;
        private readonly UserManager<User> userManager;
        public ProfileController(ApplicationContext _db, UserManager<User> _userManager)
        {
            db = _db;
            userManager = _userManager;
        }


        [HttpGet]
        public ActionResult Index()
        {
            List<Profile> Profiles = db.Profiles.ToList();
            return Ok(Profiles);
        }

        [HttpGet("{id}")]
        public ActionResult GetProfile(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized("Invalid user identificator");
            }

            var profile = db.Profiles.Find(id);
            if (profile == null)
            {
                return NotFound("Please create profile for current user");
            }
            return Ok(profile);
        }

        [HttpPost]
        public ActionResult CreateProfile([FromBody] Profile profile)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized("Invalid user identificator");
            }

            var existingProfile = db.Profiles.FirstOrDefault(p => p.Id == userId);
            if (existingProfile != null)
            {
                return Conflict("Profile already exists for this user. Use PUT to update.");
            }


            if (profile == null || string.IsNullOrEmpty(profile.Name))
            {
                return BadRequest("Invalid profile data.");
            }

            var newProfile = new Profile { 
                Id = userId,
                Name = profile.Name,
                Description = profile.Description,
                BirthDate = profile.BirthDate
            };

            db.Profiles.Add(profile);
            db.SaveChanges();

            return CreatedAtAction(nameof(Index), profile);
        }

        [HttpPut("{id}")]
        public ActionResult UpdateProfile(int id, [FromBody] Profile profile)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized("Invalid user identificator");
            }

            if (profile == null || string.IsNullOrEmpty(profile.Name))
            {
                return BadRequest("Invalid animal data.");
            }
            var existingProfile = db.Profiles.Find(id);
            if (existingProfile == null)
            {
                return NotFound();
            }
            existingProfile.Name = profile.Name;
            existingProfile.Description = profile.Description;
            existingProfile.BirthDate = profile.BirthDate;
            existingProfile.ProfilePicture = profile.ProfilePicture;
            db.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteProfile(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized("Invalid user identificator");
            }

            var profile = db.Profiles.Find(id);
            if (profile == null)
            {
                return NotFound();
            }
            db.Profiles.Remove(profile);
            db.SaveChanges();
            return NoContent();
        }
        


    }
}
