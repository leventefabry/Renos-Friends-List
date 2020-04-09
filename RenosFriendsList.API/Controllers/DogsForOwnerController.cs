using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using RenosFriendsList.API.Entities;
using RenosFriendsList.API.Helpers;
using RenosFriendsList.API.Models;
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
        [HttpGet(Name = "GetDogsForOwner")]
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

            var dogForOwnerFromRepo = _dogRepository.GetDogByOwner(ownerId, dogId);
            if (dogForOwnerFromRepo == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<DogDto>(dogForOwnerFromRepo));
        }

        [HttpPost(Name = "CreateDogForOwner")]
        public ActionResult<DogDto> CreateDogForOwner(int ownerId, DogForCreationDto dog)
        {
            if (!_ownerRepository.OwnerExists(ownerId))
            {
                return NotFound();
            }

            var dogEntity = _mapper.Map<Dog>(dog);
            _dogRepository.AddDog(ownerId, dogEntity);

            var dogToReturn = _mapper.Map<DogDto>(dogEntity);
            var linkedResourceToReturn = GetLinkedResourceToReturn(dogToReturn);

            return CreatedAtRoute("GetDogForOwner",
                new { ownerId = linkedResourceToReturn["OwnerId"], dogId = linkedResourceToReturn["Id"] },
                linkedResourceToReturn);
        }

        [HttpPut("{dogId}", Name = "UpdateDogForOwner")]
        public IActionResult UpdateDogForOwner(int ownerId, int dogId, DogForUpdateDto dog)
        {
            if (!_ownerRepository.OwnerExists(ownerId))
            {
                return NotFound();
            }

            var dogFromRepo = _dogRepository.GetDogByOwner(ownerId, dogId);
            if (dogFromRepo == null)
            {
                return NotFound();
            }

            _mapper.Map(dog, dogFromRepo);
            _dogRepository.UpdateDog(dogFromRepo);
            return NoContent();
        }

        [HttpPatch("{dogId}", Name = "PartiallyUpdateDogForOwner")]
        public IActionResult PartiallyUpdateDogForOwner(int ownerId, int dogId,
            JsonPatchDocument<DogForUpdateDto> patchDocument)
        {
            if (!_ownerRepository.OwnerExists(ownerId))
            {
                return NotFound();
            }

            var dogFromRepo = _dogRepository.GetDogByOwner(ownerId, dogId);
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

        [HttpDelete("{dogId}", Name = "DeleteDogForOwner")]
        public IActionResult DeleteDogForOwner(int ownerId, int dogId)
        {
            if (!_ownerRepository.OwnerExists(ownerId))
            {
                return NotFound();
            }

            var dogFromRepo = _dogRepository.GetDogByOwner(ownerId, dogId);
            if (dogFromRepo == null)
            {
                return NotFound();
            }

            _dogRepository.DeleteDog(dogFromRepo);
            return NoContent();
        }

        private IEnumerable<LinkDto> CreateLinksForDogForOwner(int dogId, int ownerId)
        {
            var links = new List<LinkDto>();
            links.Add(new LinkDto(Url.Link("GetDogForOwner", new { ownerId, dogId }), "self", "GET"));
            links.Add(new LinkDto(Url.Link("GetDogsForOwner", new { ownerId }), "get_dogs_for_owner", "GET"));
            links.Add(new LinkDto(Url.Link("CreateDogForOwner", new { ownerId }), "create_dog_for_owner", "POST"));
            links.Add(new LinkDto(Url.Link("UpdateDogForOwner", new { ownerId, dogId }), "update_dog_for_owner", "PUT"));
            links.Add(new LinkDto(Url.Link("PartiallyUpdateDogForOwner", new { ownerId, dogId }), "partially_update_dog_for_owner", "PATCH"));
            links.Add(new LinkDto(Url.Link("DeleteDogForOwner", new { ownerId, dogId }), "delete_dog_for_owner", "DELETE"));

            return links;
        }

        private IDictionary<string, object> GetLinkedResourceToReturn(DogDto dogToReturn)
        {
            var links = CreateLinksForDogForOwner(dogToReturn.Id, dogToReturn.OwnerId);
            var linkedResourceToReturn = dogToReturn.ShapeData(null) as IDictionary<string, object>;
            linkedResourceToReturn.Add("links", links);

            return linkedResourceToReturn;
        }
    }
}
