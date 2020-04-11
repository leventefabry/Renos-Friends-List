using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using RenosFriendsList.API.ActionConstraints;
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
        private readonly IPropertyCheckerService _propertyCheckerService;

        public DogsForOwnerController(IOwnerRepository ownerRepository,
                              IDogRepository dogRepository,
                              IMapper mapper,
                              IPropertyCheckerService propertyCheckerService)
        {
            _ownerRepository = ownerRepository;
            _dogRepository = dogRepository;
            _mapper = mapper;
            _propertyCheckerService = propertyCheckerService;
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
        [Produces("application/json",
            "application/vnd.marvin.hateoas+json",
            "application/vnd.marvin.dogforowner.full+json",
            "application/vnd.marvin.dogforowner.full.hateoas+json",
            "application/vnd.marvin.dogforowner.primary+json",
            "application/vnd.marvin.dogforowner.primary.hateoas+json")]
        public ActionResult<DogDto> GetDogForOwner(int ownerId, int dogId, string fields, [FromHeader(Name = "Accept")]string mediaType)
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue parsedMediaType))
            {
                return BadRequest();
            }

            if (!_propertyCheckerService.TypeHasProperties<DogDto>(fields))
            {
                return BadRequest();
            }

            if (!_ownerRepository.OwnerExists(ownerId))
            {
                return NotFound();
            }

            var dogForOwnerFromRepo = _dogRepository.GetDogByOwner(ownerId, dogId);
            if (dogForOwnerFromRepo == null)
            {
                return NotFound();
            }

            var includeLinks =
                parsedMediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);

            IEnumerable<LinkDto> links = new List<LinkDto>();
            if (includeLinks)
            {
                links = CreateLinksForDogForOwner(dogId, dogForOwnerFromRepo.OwnerId, fields);
            }

            var primaryMediaType = includeLinks ?
                parsedMediaType.SubTypeWithoutSuffix
                    .Substring(0, parsedMediaType.SubTypeWithoutSuffix.Length - 8)
                : parsedMediaType.SubTypeWithoutSuffix;

            if (primaryMediaType == "vnd.marvin.dogforowner.primary")
            {
                var primaryResourceToReturn =
                    _mapper.Map<DogPrimaryDto>(dogForOwnerFromRepo).ShapeData(fields) as IDictionary<string, object>;
                if (includeLinks)
                {
                    primaryResourceToReturn.Add("links", links);
                }

                return Ok(primaryResourceToReturn);
            }

            var fullResourceToReturn = _mapper.Map<DogDto>(dogForOwnerFromRepo).ShapeData(fields) as IDictionary<string, object>;
            if (includeLinks)
            {
                fullResourceToReturn.Add("links", links);
            }

            return Ok(fullResourceToReturn);
        }

        [HttpPost(Name = "CreateDogForOwner")]
        [RequestHeaderMatchesMediaType("Content-Type",
            "application/json",
            "application/vnd.marvin.dogforcreation+json")]
        [Consumes("application/json",
            "application/vnd.marvin.dogforcreation+json")]
        public ActionResult<DogDto> CreateDogForOwner(int ownerId, DogForCreationDto dog)
        {
            if (!_ownerRepository.OwnerExists(ownerId))
            {
                return NotFound();
            }

            var dogEntity = _mapper.Map<Dog>(dog);
            _dogRepository.AddDog(ownerId, dogEntity);

            var dogToReturn = _mapper.Map<DogDto>(dogEntity);
            var linkedResourceToReturn = GetLinkedResourceToReturn(dogToReturn, null);

            return CreatedAtRoute("GetDogForOwner",
                new { ownerId = linkedResourceToReturn["OwnerId"], dogId = linkedResourceToReturn["Id"] },
                linkedResourceToReturn);
        }

        [HttpPost(Name = "CreateDogForOwnerWithDateOfBirth")]
        [RequestHeaderMatchesMediaType("Content-Type",
            "application/vnd.marvin.dogforcreationwithdateofbirth+json")]
        [Consumes("application/vnd.marvin.dogforcreationwithdateofbirth+json")]
        public ActionResult<DogDto> CreateDogForOwnerWithDateOfBirth(int ownerId, DogForCreationWithDateOfBirthDto dog)
        {
            if (!_ownerRepository.OwnerExists(ownerId))
            {
                return NotFound();
            }

            var dogEntity = _mapper.Map<Dog>(dog);
            _dogRepository.AddDog(ownerId, dogEntity);

            var dogToReturn = _mapper.Map<DogDto>(dogEntity);
            var linkedResourceToReturn = GetLinkedResourceToReturn(dogToReturn, null);

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

        private IEnumerable<LinkDto> CreateLinksForDogForOwner(int dogId, int ownerId, string fields)
        {
            var links = new List<LinkDto>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(new LinkDto(Url.Link("GetDogForOwner", new { dogId, ownerId }), "self", "GET"));
            }
            else
            {
                links.Add(new LinkDto(Url.Link("GetDogForOwner", new { dogId, ownerId, fields }), "self", "GET"));
            }

            links.Add(new LinkDto(Url.Link("GetDogsForOwner", new { ownerId }), "get_dogs_for_owner", "GET"));
            links.Add(new LinkDto(Url.Link("CreateDogForOwnerWithDateOfBirth", new { ownerId }), "create_dog_for_owner_with_date_of_birth", "POST"));
            links.Add(new LinkDto(Url.Link("CreateDogForOwner", new { ownerId }), "create_dog_for_owner", "POST"));
            links.Add(new LinkDto(Url.Link("UpdateDogForOwner", new { ownerId, dogId }), "update_dog_for_owner", "PUT"));
            links.Add(new LinkDto(Url.Link("PartiallyUpdateDogForOwner", new { ownerId, dogId }), "partially_update_dog_for_owner", "PATCH"));
            links.Add(new LinkDto(Url.Link("DeleteDogForOwner", new { ownerId, dogId }), "delete_dog_for_owner", "DELETE"));

            return links;
        }

        private IDictionary<string, object> GetLinkedResourceToReturn(DogDto dogToReturn, string fields)
        {
            var links = CreateLinksForDogForOwner(dogToReturn.Id, dogToReturn.OwnerId, fields);
            var linkedResourceToReturn = dogToReturn.ShapeData(null) as IDictionary<string, object>;
            linkedResourceToReturn.Add("links", links);

            return linkedResourceToReturn;
        }
    }
}
