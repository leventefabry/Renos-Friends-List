using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RenosFriendsList.API.Entities;
using RenosFriendsList.API.Helpers;
using RenosFriendsList.API.Models;
using RenosFriendsList.API.Services;

namespace RenosFriendsList.API.Controllers
{
    [ApiController]
    [Route("api/ownercollections")]
    public class OwnerCollectionsController : ControllerBase
    {
        private readonly IOwnerRepository _ownerRepository;
        private readonly IMapper _mapper;

        public OwnerCollectionsController(IOwnerRepository ownerRepository,
            IMapper mapper)
        {
            _ownerRepository = ownerRepository;
            _mapper = mapper;
        }

        [HttpGet("({ids})", Name = "GetOwnerCollection")]
        public ActionResult<IEnumerable<OwnerDto>> GetOwnerCollection(
            [FromRoute][ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<int> ids)
        {
            if (ids == null)
            {
                return BadRequest();
            }

            var ownerIds = ids.ToList();
            var ownerEntities = _ownerRepository.GetOwners(ownerIds);
            if (ownerIds.Count != ownerEntities.Count())
            {
                return NotFound();
            }

            var ownersToReturn = _mapper.Map<IEnumerable<OwnerDto>>(ownerEntities);
            return Ok(ownersToReturn);
        }

        [HttpPost]
        public ActionResult<IEnumerable<OwnerDto>> CreateOwnerCollection(IEnumerable<OwnerForCreationDto> ownerCollection)
        {
            var ownerEntities = _mapper.Map<IEnumerable<Owner>>(ownerCollection);
            foreach (var owner in ownerEntities)
            {
                _ownerRepository.AddOwner(owner);
            }

            var ownerCollectionToReturn = _mapper.Map<IEnumerable<OwnerDto>>(ownerEntities);
            var idsAsString = string.Join(",", ownerCollectionToReturn.Select(o => o.Id));
            return CreatedAtRoute("GetOwnerCollection", new {ids = idsAsString}, ownerCollectionToReturn);
        }
    }
}
