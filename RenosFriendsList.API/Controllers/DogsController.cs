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
        public IActionResult GetDogs([FromQuery]DogsResourceParameters parameters)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<DogDto, Dog>(parameters.OrderBy))
            {
                return BadRequest();
            }

            if (!_propertyCheckerService.TypeHasProperties<DogDto>(parameters.Fields))
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

            return Ok(_mapper.Map<IEnumerable<DogDto>>(dogsFromRepo).ShapeData(parameters.Fields));
        }

        [HttpHead]
        [HttpGet("{dogId}")]
        public ActionResult<DogDto> GetDog(int dogId, string fields)
        {
            if (!_propertyCheckerService.TypeHasProperties<DogDto>(fields))
            {
                return BadRequest();
            }

            var dogFromRepo = _dogRepository.GetDog(dogId);
            if (dogFromRepo == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<DogDto>(dogFromRepo).ShapeData(fields));
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
    }
}
