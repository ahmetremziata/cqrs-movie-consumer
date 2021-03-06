using System;
using System.Collections.Generic;

namespace Logic.Events
{
    public class MovieActivatedEvent
    {
        public int MovieId { get; set; }
        public string Name { get; set; }
        public string OriginalName { get; set; }
        public string Description { get; set; }
        public int ConstructionYear { get; set; }
        public int TotalMinute { get; set; }
        public string PosterUrl { get; set; }
        public MovieIdentity Identity { get; set; }
        public List<ActorEvent> Actors { get; set; }
    }
}