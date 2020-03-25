using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RenosFriendsList.API.Entities;
using RenosFriendsList.API.Models;
using RenosFriendsList.API.Services;

namespace RenosFriendsList.API.Controllers
{
    [ApiController]
    [Route("api/owners/{ownerId}/dogs")]
    public class DogsForOwnerController : ControllerBase
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
        public ActionResult UpdateDogForOwner(int ownerId, int dogId, DogForUpdateDto dog)
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
        public ActionResult PartiallyUpdateDogForOwner(int ownerId, int dogId,
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

        [HttpDelete("{dogId}")]
        public ActionResult DeleteDogForOwner(int ownerId, int dogId)
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

        public override ActionResult ValidationProblem(
            [ActionResultObjectValue] ModelStateDictionary modelStateDictionary)
        {
            var options = HttpContext.RequestServices
                .GetRequiredService<IOptions<ApiBehaviorOptions>>();
            return (ActionResult)options.Value.InvalidModelStateResponseFactory(ControllerContext);
        }
    }
}
