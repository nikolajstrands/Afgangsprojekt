using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using CatalogApi.Models;
using CatalogApi.Data;

namespace CatalogApi.Controllers
{
    [ApiController]
    // Controlleren håndterer forespørgsler der starter med api/albums
    [Route("api/albums")]
    public class AlbumController : ControllerBase
    {
        
        private readonly ICatalogRepo _repo;
        private readonly ILogger<AlbumController> _logger;

        public AlbumController(ILogger<AlbumController> logger, ICatalogRepo repo)
        {
            _logger = logger;
            _repo = repo;
        }

        // Hent albums ud fra søgestreng eller liste af GUID'er
        [HttpGet]
        public ActionResult<List<Album>> GetByQuery([FromQuery] QueryParameters queryParameters)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                if (queryParameters.Ids.Count == 0)
                {
                    if (String.IsNullOrEmpty(queryParameters.Query))
                    {
                        var albums = _repo.GetAll();
                        return Ok(albums);
                    }
                    else
                    {                        
                        var albums = _repo.GetByQuery(queryParameters.Query);
                        return Ok(albums);  
                    }
                }
                else 
                {
                    var albums = _repo.GetByIds(queryParameters.Ids);
                    return Ok(albums);
                }  
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }                
        }

        // Hent album ud fra GUID
        [HttpGet("{id}")]
        public ActionResult<Album> GetById(Guid id)
        {
            try
            {
                 var album = _repo.GetById(id);

                if (album == null)
                {
                    return NotFound();
                }
                
                return Ok(album); 

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            } 
        }
        
        // Metode der opretter et nyt album
        [Authorize(Policy = "WriteAccess")]
        [HttpPost]
        public ActionResult<Album> PostResource([FromBody] AlbumRequestDTO album)
        {
            // Tjek om forespørgselsobjektet er validt
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Opret nyt album-objekt
                var newAlbum = new Album
                {
                    Artist = album.Artist,
                    Title = album.Title,
                    Released = album.Released,
                    Label = album.Label,
                    CoverImageUrl = album.CoverImageUrl,
                    Tracks = new List<Track>()
                };

                // Opret et track-objekt for hvert track
                foreach(TrackRequestDTO track in album.Tracks)
                {
                    var newTrack = new Track
                    {
                        Length = track.Length,
                        Title = track.Title,
                        Number = track.Number
                    };

                    // Tilføj track til nyt album
                    newAlbum.Tracks.Add(newTrack);
                }

                // Tilføj album til repository (dvs. databasen)
                _repo.Add(newAlbum);

                // Returner 201 Created med det oprettede objekt
                return CreatedAtAction(nameof(GetById), new { id = newAlbum.Id }, newAlbum);
            }
            catch (Exception ex)
            {
                // Hvis noget går galt, returner 500
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }         
        }

        // Slet album ud fra GUID
        [Authorize(Policy = "WriteAccess")]
        [HttpDelete, Route("{id}")]
        public ActionResult DeleteById(Guid id)
        {
            try
            {
                var album = _repo.GetById(id);

                if(album == null)
                {
                    return NotFound();
                }

                _repo.Delete(album);
                return Ok();
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }          
        }
    }
}
