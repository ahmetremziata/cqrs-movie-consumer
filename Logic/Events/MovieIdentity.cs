using System;
using System.Collections.Generic;

namespace Logic.Events
{
    public class MovieIdentity
    {
        public DateTime? VisionEntryDate { get; set; }
        public List<ActorEvent> Directors { get; set; }
        public List<ActorEvent> Scenarists { get; set; }
        public List<ActorEvent> Producers { get; set; }
        public List<ActorEvent> PhotographyDirectors { get; set; }
        public List<ActorEvent> Composers { get; set; }
        public List<ActorEvent> BookAuthors { get; set; }
        public List<Country> Countries { get; set; }
        public List<Type> Types { get; set; }
    }
}