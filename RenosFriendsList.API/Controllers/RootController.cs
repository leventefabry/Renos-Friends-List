using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using RenosFriendsList.API.Models;

namespace RenosFriendsList.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class RootController : ControllerBase
    {
        [HttpGet(Name = "GetRoot")]
        public IActionResult GetRoot()
        {
            var links = new List<LinkDto>();

            links.Add(new LinkDto(Url.Link("GetRoot", new { }), "self", "GET"));
            links.Add(new LinkDto(Url.Link("GetOwners", new { }), "owners", "GET"));
            links.Add(new LinkDto(Url.Link("GetDogs", new { }), "dogs", "GET"));

            return Ok(links);
        }
    }
}
