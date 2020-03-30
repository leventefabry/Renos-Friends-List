using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RenosFriendsList.API.Models.Dog;
using RenosFriendsList.API.ResourceParameters;
using RenosFriendsList.API.Services;

namespace RenosFriendsList.API.Controllers
{
    [ApiController]
    [Route("api/dogs")]
    public class DogsController : ApiControllerBase
    {
        private readonly IDogRepository _dogRepository;
        private readonly IMapper _mapper;

        public DogsController(IDogRepository dogRepository,
            IMapper mapper)
        {
            _dogRepository = dogRepository;
            _mapper = mapper;
        }

        [HttpHead]
        [HttpGet]
        public ActionResult<IEnumerable<DogDto>> GetDogs([FromQuery]DogsResourceParameters parameters)
        {
            var dogsFromRepo = _dogRepository.GetAllDogs(parameters);
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
    }
}
