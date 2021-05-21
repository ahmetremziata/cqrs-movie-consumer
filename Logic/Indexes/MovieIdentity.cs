using System;
using System.Collections.Generic;

namespace Logic.Indexes
{
    public class MovieIdentity
    {
        public DateTime? VisionEntryDate { get; set; }
        public List<Actor> Directors { get; set; }
        public List<Actor> Scenarists { get; set; }
        public List<Actor> Producers { get; set; }
        public List<Actor> PhotographyDirectors { get; set; }
        public List<Actor> Composers { get; set; }
        public List<Actor> BookAuthors { get; set; }
        public List<Country> Countries { get; set; }
        public List<Type> Types { get; set; }
    }
}