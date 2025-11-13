using System;
using System.Collections.Generic;
using System.Security.Policy;

namespace LibraryManagement.Web.Entity
{
    public class Genre
    {
        

        public int GenreId { get; set; }
        public string Name { get; set; } = string.Empty;
        public HashSet<Book> Books { get; set; }

    }
}
