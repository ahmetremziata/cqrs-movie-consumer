using System.Collections.Generic;
using Logic.Business.Service.Interfaces;
using Logic.Events;
using Logic.Indexes;
using Nest;
using Newtonsoft.Json;

namespace Logic.Business.Service
{
    public class ElasticService : IElasticService
    {
        private readonly IElasticClient _elasticClient;
        public ElasticService(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }
        
        public void InsertMovie(string payload)
        {
            MovieActivatedEvent movieActivatedEvent = JsonConvert.DeserializeObject<MovieActivatedEvent>(payload);

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

            var identityEvent = movieActivatedEvent.Identity;
            Logic.Indexes.MovieIdentity movieIdentity = new Logic.Indexes.MovieIdentity
            {
                VisionEntryDate = identityEvent.VisionEntryDate,
                BookAuthors = GetActors(identityEvent.BookAuthors),
                Producers = GetActors(identityEvent.Producers),
                Composers = GetActors(identityEvent.Composers),
                Scenarists = GetActors(identityEvent.Scenarists),
                PhotographyDirectors = GetActors(identityEvent.PhotographyDirectors),
                Directors = GetActors(identityEvent.Directors),
                Types = GetTypes(identityEvent.Types),
                Countries = GetCountries(identityEvent.Countries)
            };

            movie.Identity = movieIdentity;
            
            movie.Actors = GetActors(movieActivatedEvent.Actors);
            
            _elasticClient.IndexDocument(movie);
        }
        
        private List<Actor> GetActors(List<ActorEvent> actorEvents)
        {
            List<Actor> actors = new List<Actor>();
            
            foreach (var actor in actorEvents)
            {
                actors.Add(GetActor(actor));
            }

            return actors;
        }
        
        private List<Indexes.Type> GetTypes(List<Events.Type> typeEvents)
        {
            List<Indexes.Type> types = new List<Indexes.Type>();
            
            foreach (var type in typeEvents)
            {
                types.Add(GetType(type));
            }

            return types;
        }
        
        private List<Indexes.Country> GetCountries(List<Events.Country> countryEvents)
        {
            List<Indexes.Country> countries = new List<Indexes.Country>();
            
            foreach (var country in countryEvents)
            {
                countries.Add(GetCountry(country));
            }

            return countries;
        }

        private Indexes.Actor GetActor(ActorEvent actorEvent)
        {
            return new Actor()
            {
                AvatarUrl = actorEvent.AvatarUrl,
                Id = actorEvent.Id,
                CharacterName = actorEvent.CharacterName,
                Name = actorEvent.Name
            };
        }
        
        private Indexes.Type GetType(Events.Type typeEvent)
        {
            return new Indexes.Type()
            {
                Id = typeEvent.Id,
                Name = typeEvent.Name
            };
        }
        
        private Indexes.Country GetCountry(Events.Country countryEvent)
        {
            return new Indexes.Country()
            {
                Id = countryEvent.Id,
                Name = countryEvent.Name
            };
        }
    }
}