using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using RenosFriendsList.API.Entities;
using RenosFriendsList.API.Models.Dog;
using RenosFriendsList.API.Services;

namespace RenosFriendsList.API.Controllers
{
    [ApiController]
    [Route("api/owners/{ownerId}/dogs")]
    public class DogsForOwnerController : ApiControllerBase
    {
        private readonly IOwnerRepository _ownerRepository;
        private readonly IDogRepository _dogRepository;
        private readonly IMapper _mapper;

        public DogsForOwnerController(IOwnerRepository ownerRepository,
                              IDogRepository dogRepository,
                              IMapper mapper)
        {
            _ownerRepository = ownerRepository;
            _dogRepository = dogRepository;
            _mapper = mapper;
        }

        [HttpHead]
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

        [HttpHead]
        [HttpGet("{dogId}", Name = "GetDogForOwner")]
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

        [HttpPost]
        public ActionResult<DogDto> CreateDogForOwner(int ownerId, DogForCreationDto dog)
        {
            if (!_ownerRepository.OwnerExists(ownerId))
            {
                return NotFound();
            }

            var dogEntity = _mapper.Map<Dog>(dog);
            _dogRepository.AddDog(ownerId, dogEntity);

            var dogToReturn = _mapper.Map<DogDto>(dogEntity);
            return CreatedAtRoute("GetDogForOwner", new {ownerId = ownerId, dogId = dogToReturn.Id}, dogToReturn);
        }

        [HttpPut("{dogId}")]
        public IActionResult UpdateDogForOwner(int ownerId, int dogId, DogForUpdateDto dog)
        {
            if (!_ownerRepository.OwnerExists(ownerId))
            {
                return NotFound();
            }

            var dogFromRepo = _dogRepository.GetDog(ownerId, dogId);
            if (dogFromRepo == null)
            {
                return NotFound();
            }

            _mapper.Map(dog, dogFromRepo);
            _dogRepository.UpdateDog(dogFromRepo);
            return NoContent();
        }

        [HttpPatch("{dogId}")]
        public IActionResult PartiallyUpdateDogForOwner(int ownerId, int dogId,
            JsonPatchDocument<DogForUpdateDto> patchDocument)
        {
            if (!_ownerRepository.OwnerExists(ownerId))
            {
                return NotFound();
            }

            var dogFromRepo = _dogRepository.GetDog(ownerId, dogId);
            if (dogFromRepo == null)
            {
                return NotFound();
            }

            var dogToPatch = _mapper.Map<DogForUpdateDto>(dogFromRepo);
            patchDocument.ApplyTo(dogToPatch, ModelState);

            if (!TryValidateModel(dogToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(dogToPatch, dogFromRepo);
            _dogRepository.UpdateDog(dogFromRepo);
            return NoContent();
        }

        [HttpOptions]
        public IActionResult GetDogsForOwnerOptions()
        {
            Response.Headers.Add("Allow", "GET,OPTIONS,POST,PUT,PATCH,DELETE");
            return Ok();
        }

        [HttpDelete("{dogId}")]
        public IActionResult DeleteDogForOwner(int ownerId, int dogId)
        {
            if (!_ownerRepository.OwnerExists(ownerId))
            {
                return NotFound();
            }

            var dogFromRepo = _dogRepository.GetDog(ownerId, dogId);
            if (dogFromRepo == null)
            {
                return NotFound();
            }

            _dogRepository.DeleteDog(dogFromRepo);
            return NoContent();
        }
    }
}
