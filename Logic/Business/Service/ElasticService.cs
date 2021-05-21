using AutoMapper;
using Logic.Business.Service.Interfaces;
using Logic.Events;
using Logic.Indexes;
using Newtonsoft.Json;

namespace Logic.Business.Service
{
    public class ElasticService : IElasticService
    {
        private readonly IMapper _mapper;

        public ElasticService(IMapper mapper)
        {
            _mapper = mapper;   
        }

        public void InsertMovie(string payload)
        {
            MovieActivatedEvent movieActivatedEvent = JsonConvert.DeserializeObject<MovieActivatedEvent>(payload);
            //TODO: Fix with automapper
            //Movie movie = _mapper.Map<Movie>(movieActivatedEvent);

            Movie movie = new Movie
            {
                Description = movieActivatedEvent.Description,
                Name = movieActivatedEvent.Name,
                OriginalName = movieActivatedEvent.OriginalName,
                PosterUrl = movieActivatedEvent.PosterUrl,
                TotalMinute = movieActivatedEvent.TotalMinute,
                MovieId = movieActivatedEvent.MovieId,
                ConstructionYear = movieActivatedEvent.ConstructionYear
            };
        }
    }
}