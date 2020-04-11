using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.Net.Http.Headers;
using RenosFriendsList.API.Entities;
using RenosFriendsList.API.Helpers;
using RenosFriendsList.API.Models;
using RenosFriendsList.API.Models.Dog;
using RenosFriendsList.API.ResourceParameters;
using RenosFriendsList.API.Services;
using RenosFriendsList.API.Services.PropertyMapping;

namespace RenosFriendsList.API.Controllers
{
    [ApiController]
    [Route("api/dogs")]
    public class DogsController : ApiControllerBase
    {
        private readonly IDogRepository _dogRepository;
        private readonly IMapper _mapper;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly IPropertyCheckerService _propertyCheckerService;

        public DogsController(IDogRepository dogRepository,
            IMapper mapper,
            IPropertyMappingService propertyMappingService,
            IPropertyCheckerService propertyCheckerService)
        {
            _dogRepository = dogRepository;
            _mapper = mapper;
            _propertyMappingService = propertyMappingService;
            _propertyCheckerService = propertyCheckerService;
        }

        [HttpHead]
        [HttpGet(Name = "GetDogs")]
        public IActionResult GetDogs([FromQuery]DogsResourceParameters parameters, [FromHeader(Name = "Accept")]string mediaType)
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue parsedMediaType))
            {
                return BadRequest();
            }

            if (!_propertyMappingService.ValidMappingExistsFor<DogDto, Dog>(parameters.OrderBy))
            {
                return BadRequest();
            }

            if (!_propertyCheckerService.TypeHasProperties<DogDto>(parameters.Fields))
            {
                return BadRequest();
            }

            var dogsFromRepo = _dogRepository.GetAllDogs(parameters);

            Response.AddPagination(dogsFromRepo.TotalCount, dogsFromRepo.PageSize, dogsFromRepo.CurrentPage,
                dogsFromRepo.TotalPages, null, null);

            if (parsedMediaType.MediaType == "application/vnd.marvin.hateoas+json")
            {
                var linkedCollectionResource = GetLinkedCollectionResource(parameters, dogsFromRepo);
                return Ok(linkedCollectionResource);
            }

            return Ok(_mapper.Map<IEnumerable<DogDto>>(dogsFromRepo).ShapeData(parameters.Fields));
        }

        [HttpHead]
        [HttpGet("{dogId}", Name = "GetDog")]
        [Produces("application/json",
            "application/vnd.marvin.hateoas+json",
            "application/vnd.marvin.dog.full+json",
            "application/vnd.marvin.dog.full.hateoas+json",
            "application/vnd.marvin.dog.primary+json",
            "application/vnd.marvin.dog.primary.hateoas+json")]
        public ActionResult<DogDto> GetDog(int dogId, string fields, [FromHeader(Name = "Accept")]string mediaType)
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue parsedMediaType))
            {
                return BadRequest();
            }

            if (!_propertyCheckerService.TypeHasProperties<DogDto>(fields))
            {
                return BadRequest();
            }

            var dogFromRepo = _dogRepository.GetDog(dogId);
            if (dogFromRepo == null)
            {
                return NotFound();
            }

            var includeLinks =
                parsedMediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);

            IEnumerable<LinkDto> links = new List<LinkDto>();
            if (includeLinks)
            {
                links = CreateLinksForDog(dogId, dogFromRepo.OwnerId, fields);
            }

            var primaryMediaType = includeLinks ?
                parsedMediaType.SubTypeWithoutSuffix
                    .Substring(0, parsedMediaType.SubTypeWithoutSuffix.Length - 8)
                : parsedMediaType.SubTypeWithoutSuffix;

            if (primaryMediaType == "vnd.marvin.dog.primary")
            {
                var primaryResourceToReturn =
                    _mapper.Map<DogPrimaryDto>(dogFromRepo).ShapeData(fields) as IDictionary<string, object>;
                if (includeLinks)
                {
                    primaryResourceToReturn.Add("links", links);
                }

                return Ok(primaryResourceToReturn);
            }

            var fullResourceToReturn = _mapper.Map<DogDto>(dogFromRepo).ShapeData(fields) as IDictionary<string, object>;
            if (includeLinks)
            {
                fullResourceToReturn.Add("links", links);
            }

            return Ok(fullResourceToReturn);
        }

        [HttpOptions(Name = "GetDogsOptions")]
        public IActionResult GetDogsOptions()
        {
            Response.Headers.Add("Allow", "GET,OPTIONS,HEAD");
            return Ok();
        }

        private string CreateDogsResourceUri(DogsResourceParameters parameters, ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetDogs", new
                    {
                        fields = parameters.Fields,
                        orderBy = parameters.OrderBy,
                        pageNumber = parameters.PageNumber - 1,
                        pageSize = parameters.PageSize,
                        name = parameters.Name,
                        renoLikesIt = parameters.RenoLikesIt,
                        bodyType = parameters.BodyType,
                        gender = parameters.Gender
                    });
                case ResourceUriType.NextPage:
                    return Url.Link("GetDogs", new
                    {
                        fields = parameters.Fields,
                        orderBy = parameters.OrderBy,
                        pageNumber = parameters.PageNumber + 1,
                        pageSize = parameters.PageSize,
                        name = parameters.Name,
                        renoLikesIt = parameters.RenoLikesIt,
                        bodyType = parameters.BodyType,
                        gender = parameters.Gender
                    });
                case ResourceUriType.Current:
                default:
                    return Url.Link("GetDogs", new
                    {
                        fields = parameters.Fields,
                        orderBy = parameters.OrderBy,
                        pageNumber = parameters.PageNumber,
                        pageSize = parameters.PageSize,
                        name = parameters.Name,
                        renoLikesIt = parameters.RenoLikesIt,
                        bodyType = parameters.BodyType,
                        gender = parameters.Gender
                    });
            }
        }

        private IEnumerable<LinkDto> CreateLinksForDog(int dogId, int ownerId, string fields)
        {
            var links = new List<LinkDto>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(new LinkDto(Url.Link("GetDog", new { dogId }), "self", "GET"));
            }
            else
            {
                links.Add(new LinkDto(Url.Link("GetDog", new { dogId, fields }), "self", "GET"));
            }

            links.Add(new LinkDto(Url.Link("GetDogsForOwner", new { ownerId }), "get_dogs_for_owner", "GET"));
            links.Add(new LinkDto(Url.Link("CreateDogForOwner", new { ownerId }), "create_dog_for_owner", "POST"));
            links.Add(new LinkDto(Url.Link("CreateDogForOwnerWithDateOfBirth", new { ownerId }), "create_dog_for_owner_with_date_of_birth", "POST"));
            links.Add(new LinkDto(Url.Link("UpdateDogForOwner", new { ownerId, dogId }), "update_dog_for_owner", "PUT"));
            links.Add(new LinkDto(Url.Link("PartiallyUpdateDogForOwner", new { ownerId, dogId }), "partially_update_dog_for_owner", "PATCH"));
            links.Add(new LinkDto(Url.Link("DeleteDogForOwner", new { ownerId, dogId }), "delete_dog_for_owner", "DELETE"));

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForAuthors(DogsResourceParameters parameters, bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>();
            links.Add(new LinkDto(CreateDogsResourceUri(parameters, ResourceUriType.Current), "self", "GET"));

            if (hasNext)
            {
                links.Add(new LinkDto(CreateDogsResourceUri(parameters, ResourceUriType.NextPage), "next_page", "GET"));
            }

            if (hasPrevious)
            {
                links.Add(new LinkDto(CreateDogsResourceUri(parameters, ResourceUriType.PreviousPage), "previous_page", "GET"));
            }

            return links;
        }

        private DogsLinkedCollectionResource GetLinkedCollectionResource(DogsResourceParameters parameters, PagedList<Dog> dogsFromRepo)
        {
            var links = CreateLinksForAuthors(parameters, dogsFromRepo.HasNext, dogsFromRepo.HasPrevious);
            var shapedDogs = _mapper.Map<IEnumerable<DogDto>>(dogsFromRepo).ShapeData(parameters.Fields);

            var shapedDogsWithLinks = shapedDogs.Select(dog =>
            {
                var dogAsDictionary = dog as IDictionary<string, object>;
                var dogsLinks = CreateLinksForDog((int)dogAsDictionary["Id"], (int)dogAsDictionary["OwnerId"], null);
                dogAsDictionary.Add("links", dogsLinks);
                return dogAsDictionary;
            });

            return new DogsLinkedCollectionResource(shapedDogsWithLinks, links);
        }
    }
}
