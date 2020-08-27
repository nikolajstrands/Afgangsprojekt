using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using UserDataAPI.Models;
using UserDataAPI.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace UserDataAPI.Controllers
{
    [ApiController]
    [Route("api/me/albums")]
    public class UserController : ControllerBase
    {
        private readonly IUserDataRepo _repo;
        private int _userId;
        
        public UserController(IUserDataRepo repo)
        {
            _repo = repo;

        }

        // GET api/me/albums
        [Authorize(Policy = "ReadAndWriteAccess")]
        [HttpGet]
        public ActionResult<IEnumerable<Guid>> GetUserAlbums()
        {
            try
            {
                // Forsøg at sætte brugeren
                if(!SetUser())
                {
                    return Forbid();
                }
                
                // Find bruger i db
                var user = _repo.GetById(_userId);

                // Opret, hvis brugeren ikke findes
                if (user == null)
                {
                    user = new User()
                    {
                        Id = _userId,
                        AlbumIDs = new List<Guid>()
                    };  
                    _repo.Add(user);             
                }
            
                return Ok(user.AlbumIDs);
     
            }
            catch (Exception ex)
            { 
                // Hvis fx db fejler, returneres status 500           
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // PUT api/me/albums
        [Authorize(Policy = "ReadAndWriteAccess")]
        [HttpPut]
        public ActionResult PutUserAlbums([FromBody] IEnumerable<Guid> albumIDs)
        {
            try
            {
                // Forsøg at sætte brugeren
                if(!SetUser())
                {
                    return Forbid();
                }

                // Tjek at medsendt data er formateret korretk
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                
                // Find bruger i db
                var user = _repo.GetById(_userId);

                // Opret, hvis brugeren ikke findes
                if (user == null)
                {
                    user = new User()
                    {
                        Id = _userId,
                        AlbumIDs = new List<Guid>()
                    };
                    _repo.Add(user);
                }

                // Tilføj nye albums til bruger
                foreach(Guid newId in albumIDs)
                {
                    if (!user.AlbumIDs.Contains(newId))
                    {
                        user.AlbumIDs.Add(newId);
                    }                
                }

                // Gem ændringer til db
                _repo.Save(user);

                return Created("", null);                
            
            }
            catch (Exception ex)
            { 
                // Hvis fx db fejler, returneres status 500           
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        // DELETE api/me/albums
        [Authorize(Policy = "ReadAndWriteAccess")]
        [HttpDelete]
        public ActionResult DeleteUserAlbums([FromBody] IEnumerable<Guid> albumIDs)
        {
            try
            {
                // Forsøg at sætte brugeren
                if(!SetUser())
                {
                    return Forbid();
                }

                // Tjek at medsendt data er formateret korretk
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                
                // Find bruger i db
                var user = _repo.GetById(_userId);

                // Hvis brugeren ikke findes returneres 404
                if (user == null)
                {
                    return NotFound();
                }

                // Fjern de tilsendte album-id'er
                foreach(Guid oldId in albumIDs)
                {
                    user.AlbumIDs.Remove(oldId);
                }

                // Gem ændringer til db
                _repo.Save(user);

                return Ok();
            }
            catch (Exception ex)
            { 
                // Hvis fx db fejler, returneres status 500           
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        // Metode der sætter brugerId ud fra access token
        private bool SetUser()
        {
            // Find claim af typen NameIdentifier
            var userIdFromClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdFromClaim == null || userIdFromClaim == "")
            {
                // Findes den ikke, returner false
                return false;
            }

            var result = 0;
            if(!Int32.TryParse(userIdFromClaim, out result))
            {
                // Kan den ikke parses til et heltal, returner false
                return false;
            }
            else
            {
                // Ellers, sæt brugeren og returner true
                _userId = result;
                return true;
            }
        }
    }
}
