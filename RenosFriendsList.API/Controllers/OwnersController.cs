using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RenosFriendsList.API.Entities;
using RenosFriendsList.API.Models.Owner;
using RenosFriendsList.API.ResourceParameters;
using RenosFriendsList.API.Services;

namespace RenosFriendsList.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OwnersController : ControllerBase
    {
        private readonly IOwnerRepository _ownerRepository;
        private readonly IMapper _mapper;

        public OwnersController(IOwnerRepository ownerRepository,
                                IMapper mapper)
        {
            _ownerRepository = ownerRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [HttpHead]
        public ActionResult<IEnumerable<Owner>> GetOwners([FromQuery] OwnersResourceParameters parameters)
        {
            var ownersFromRepo = _ownerRepository.GetOwners(parameters);
            return Ok(_mapper.Map<IEnumerable<OwnerDto>>(ownersFromRepo));
        }

        [HttpGet("{ownerId}", Name = "GetOwner")]
        public ActionResult<Owner> GetOwner(int ownerId)
        {
            var ownerFromRepo = _ownerRepository.GetOwner(ownerId);

            if (ownerFromRepo == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<OwnerDto>(ownerFromRepo));
        }

        [HttpPost]
        public ActionResult<OwnerDto> CreateOwner(OwnerForCreationDto owner)
        {
            if (owner == null)
            {
                return BadRequest();
            }

            var ownerEntity = _mapper.Map<Owner>(owner);
            _ownerRepository.AddOwner(ownerEntity);

            var ownerToReturn = _mapper.Map<OwnerDto>(ownerEntity);
            return CreatedAtRoute("GetOwner", new { ownerId = ownerToReturn.Id } ,ownerToReturn);
        }

        [HttpPut("{ownerId}")]
        public ActionResult UpdateOwner(int ownerId, OwnerForUpdateDto owner)
        {
            var ownerFromRepo = _ownerRepository.GetOwner(ownerId);
            if (ownerFromRepo == null)
            {
                return NotFound();
            }

            _mapper.Map(owner, ownerFromRepo);
            _ownerRepository.UpdateOwner(ownerFromRepo);
            return NoContent();
        }

        [HttpOptions]
        public IActionResult GetOwnersOptions()
        {
            Response.Headers.Add("Allow", "GET,OPTIONS,POST");
            return Ok();
        }

        [HttpDelete("{ownerId}")]
        public ActionResult DeleteOwner(int ownerId)
        {
            var ownerFromRepo = _ownerRepository.GetOwner(ownerId);

            if (ownerFromRepo == null)
            {
                return NotFound();
            }

            _ownerRepository.DeleteOwner(ownerFromRepo);
            return NoContent();
        }
    }
}