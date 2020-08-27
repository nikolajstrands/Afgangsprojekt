using System;

namespace CatalogApi.Models
{
    public class Track
    {
        public Guid Id {get; set;}

        public string Title {get; set;}

        public int Number {get; set;}

        public string Length {get; set; }

    }
}