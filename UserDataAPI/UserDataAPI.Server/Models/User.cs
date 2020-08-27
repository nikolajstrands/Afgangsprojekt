using System;
using System.Collections.Generic;

namespace UserDataAPI.Models
{
    public class User
    {
        public int Id {get; set;}

        public List<Guid> AlbumIDs {get; set;}

    }
}