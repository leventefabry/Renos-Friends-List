using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RenosFriendsList.API.Entities;
using RenosFriendsList.API.Models;
using RenosFriendsList.API.Services;

namespace RenosFriendsList.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        public ActionResult<IEnumerable<Owner>> GetOwners()
        {
            var ownersFromRepo = _ownerRepository.GetOwners();
            return Ok(_mapper.Map<IEnumerable<OwnerDto>>(ownersFromRepo));
        }

        [HttpGet("{ownerId}")]
        public ActionResult<Owner> GetOwner(int ownerId)
        {
            var ownerFromRepo = _ownerRepository.GetOwner(ownerId);

            if (ownerFromRepo == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<OwnerDto>(ownerFromRepo));
        }
    }
}