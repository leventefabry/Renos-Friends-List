using Microsoft.AspNetCore.Mvc;
using RenosFriendsList.API.Services;

namespace RenosFriendsList.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OwnersController : ControllerBase
    {
        private readonly IOwnerRepository _ownerRepository;

        public OwnersController(IOwnerRepository ownerRepository)
        {
            _ownerRepository = ownerRepository;
        }

        [HttpGet]
        public IActionResult GetOwners()
        {
            var ownersFromRepo = _ownerRepository.GetOwners();
            return Ok(ownersFromRepo);
        }

        [HttpGet("{ownerId}")]
        public IActionResult GetOwner(int ownerId)
        {
            var ownerFromRepo = _ownerRepository.GetOwner(ownerId);

            if (ownerFromRepo == null)
            {
                return NotFound();
            }

            return Ok(ownerFromRepo);
        }
    }
}