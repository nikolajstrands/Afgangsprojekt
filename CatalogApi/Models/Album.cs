using System;
using System.Collections.Generic;

namespace CatalogApi.Models
{
    public class Album
    {
        public Guid Id {get; set;}

        public string Artist {get; set;}

        public string Title {get; set;}

        public DateTime Released {get; set;}

        public string Label {get; set;}

        public string CoverImageUrl {get; set;} 

        public List<Track> Tracks { get; set; }

    }


}