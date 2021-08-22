using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            var movie = GetMovie(movieActivatedEvent);

            _elasticClient.IndexDocument(movie);
        }
        
        public void DeleteMovie(string payload)
        {
            MovieDeactivatedEvent movieDeactivatedEvent = JsonConvert.DeserializeObject<MovieDeactivatedEvent>(payload);
            _elasticClient.DeleteByQuery<Movie>(q => q
                .Query(rq => rq
                    .Match(m => m
                        .Field(f => f.MovieId)
                        .Query(movieDeactivatedEvent.MovieId.ToString()))
                )
                .Index("movies")
            );
        }

        public void UpdateMovie(string movieUpdatedEvent)
        {
            MovieActivatedEvent movieActivatedEvent = JsonConvert.DeserializeObject<MovieActivatedEvent>(movieUpdatedEvent);

            var searchResponse =  _elasticClient
                .Search<Indexes.Movie>(s => s.Query(q =>
                    q.Raw(GetMovieByIdQueryUrl(movieActivatedEvent.MovieId))));

            if (!searchResponse.Hits.Any())
            {
                return;
            }

            string id = searchResponse.Hits.ToList()[0].Id;
            var updatedMovie = GetMovie(movieActivatedEvent);
            
            _elasticClient.Update(DocumentPath<Movie>
                    .Id(id),
                u => u
                    .Index("movies")
                    .DocAsUpsert(true)
                    .Doc(updatedMovie));
        }
        
        private Movie GetMovie(MovieActivatedEvent movieActivatedEvent)
        {
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
            Indexes.MovieIdentity movieIdentity = new Indexes.MovieIdentity
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
            return movie;
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
        
        private string GetMovieByIdQueryUrl(int movieId)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(@"{""bool"": {""should"": [");
            stringBuilder.Append(@"{""match"": {""movieId"":" + movieId + @"}}");
            stringBuilder.Append(@"]}}");
            return stringBuilder.ToString();
        }
    }
}