using System.Collections.Generic;
using System.Text.RegularExpressions;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RenosFriendsList.API.Entities;
using RenosFriendsList.API.Helpers;
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

        public DogsController(IDogRepository dogRepository,
            IMapper mapper,
            IPropertyMappingService propertyMappingService)
        {
            _dogRepository = dogRepository;
            _mapper = mapper;
            _propertyMappingService = propertyMappingService;
        }

        [HttpHead]
        [HttpGet(Name = "GetDogs")]
        public ActionResult<IEnumerable<DogDto>> GetDogs([FromQuery]DogsResourceParameters parameters)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<DogDto, Dog>(parameters.OrderBy))
            {
                return BadRequest();
            }

            var dogsFromRepo = _dogRepository.GetAllDogs(parameters);

            var previousPageLink = dogsFromRepo.HasPrevious
                ? CreateDogsResourceUri(parameters, ResourceUriType.PreviousPage)
                : null;

            var nextPageLink = dogsFromRepo.HasNext
                ? CreateDogsResourceUri(parameters, ResourceUriType.NextPage)
                : null;

            Response.AddPagination(dogsFromRepo.TotalCount, dogsFromRepo.PageSize, dogsFromRepo.CurrentPage,
                dogsFromRepo.TotalPages, previousPageLink, nextPageLink);

            return Ok(_mapper.Map<IEnumerable<DogDto>>(dogsFromRepo));
        }

        [HttpHead]
        [HttpGet("{dogId}")]
        public ActionResult<DogDto> GetDog(int dogId)
        {
            var dogFromRepo = _dogRepository.GetDog(dogId);
            if (dogFromRepo == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<DogDto>(dogFromRepo));
        }

        [HttpOptions]
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
                        orderBy = parameters.OrderBy,
                        pageNumber = parameters.PageNumber + 1,
                        pageSize = parameters.PageSize,
                        name = parameters.Name,
                        renoLikesIt = parameters.RenoLikesIt,
                        bodyType = parameters.BodyType,
                        gender = parameters.Gender
                    });
                default:
                    return Url.Link("GetDogs", new
                    {
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
    }
}
