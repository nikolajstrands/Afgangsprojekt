using System;
using System.Collections.Generic;

namespace IngestApp.Models
{
    public class AlbumRequestDTO
    {
        public string Artist {get; set;}

        public string Title {get; set;}

        public string Released {get; set;}

        public string Label {get; set;}

        public string CoverImageUrl {get; set;} 

        public List<TrackRequestDTO> Tracks {get; set;}

    }

}