using System;
using System.Collections.Generic;

namespace CatalogApi.Models
{
    public class QueryParameters
    {
        public List<Guid> Ids { get; set; } = new List<Guid>();

        public string Query { get; set;} = "";

    }
}