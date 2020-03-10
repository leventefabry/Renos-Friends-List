using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RenosFriendsList.API.Models;
using RenosFriendsList.API.Services;

namespace RenosFriendsList.API.Controllers
{
    [ApiController]
    [Route("api/owners/{ownerId}/dogs")]
    public class DogsController : ControllerBase
    {
        private readonly IOwnerRepository _ownerRepository;
        private readonly IDogRepository _dogRepository;
        private readonly IMapper _mapper;

        public DogsController(IOwnerRepository ownerRepository,
                              IDogRepository dogRepository,
                              IMapper mapper)
        {
            _ownerRepository = ownerRepository;
            _dogRepository = dogRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<DogDto>> GetDogsForOwner(int ownerId)
        {
            if (!_ownerRepository.OwnerExists(ownerId))
            {
                return NotFound();
            }

            var dogsForOwnerFromRepo = _dogRepository.GetDogs(ownerId);
            return Ok(_mapper.Map<IEnumerable<DogDto>>(dogsForOwnerFromRepo));
        }

        [HttpGet("{dogId}")]
        public ActionResult<DogDto> GetDogForOwner(int ownerId, int dogId)
        {
            if (!_ownerRepository.OwnerExists(ownerId))
            {
                return NotFound();
            }

            var dogForOwnerFromRepo = _dogRepository.GetDog(ownerId, dogId);
            if (dogForOwnerFromRepo == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<DogDto>(dogForOwnerFromRepo));
        }
    }
}
