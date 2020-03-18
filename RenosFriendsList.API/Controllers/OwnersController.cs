﻿using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RenosFriendsList.API.Entities;
using RenosFriendsList.API.Models;
using RenosFriendsList.API.ResourceParameters;
using RenosFriendsList.API.Services;

namespace RenosFriendsList.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OwnersController : ControllerBase
    {
        private readonly IOwnerRepository _ownerRepository;
        private readonly IMapper _mapper;

        public OwnersController(IOwnerRepository ownerRepository,
                                IMapper mapper)
        {
            _ownerRepository = ownerRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [HttpHead]
        public ActionResult<IEnumerable<Owner>> GetOwners([FromQuery] OwnersResourceParameters parameters)
        {
            var ownersFromRepo = _ownerRepository.GetOwners(parameters);
            return Ok(_mapper.Map<IEnumerable<OwnerDto>>(ownersFromRepo));
        }

        [HttpGet("{ownerId}")]
        public ActionResult<Owner> GetOwner(int ownerId)
        {
            var ownerFromRepo = _ownerRepository.GetOwner(ownerId);

            if (ownerFromRepo == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<OwnerDto>(ownerFromRepo));
        }
    }
}