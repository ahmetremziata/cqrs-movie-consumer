using AutoMapper;
using Logic.Events;
using Logic.Indexes;

namespace Logic.Mappings
{
    public class MovieMapping : Profile
    {
        public MovieMapping()
        {
            CreateMap<MovieActivatedEvent, Movie>();
        }
    }
}