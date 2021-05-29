using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;

namespace Consumer.Infrastructures.Extensions
{
    public static class ElasticsearchExtensions
    {
        public static void AddElasticsearch(this IServiceCollection services, IConfiguration configuration)
        {
            var url = configuration["Elasticsearch:Url"];
            var defaultIndex = configuration["Elasticsearch:Index"];
  
            var settings = new ConnectionSettings(new Uri(url))
                .DefaultIndex(defaultIndex);
  
            AddDefaultMappings(settings);
  
            var client = new ElasticClient(settings);
  
            services.AddSingleton<IElasticClient>(client);
  
            CreateIndex(client, defaultIndex);
        }
        
        private static void AddDefaultMappings(ConnectionSettings settings)
        {
            settings.
            DefaultMappingFor<Logic.Indexes.Movie>(m => m
                .PropertyName(p => p.PosterUrl, "posterUrl")
            );
        }
  
        private static void CreateIndex(IElasticClient client, string indexName)
        {
            var createIndexResponse = client.Indices.Create(indexName,
                index => index.Map<Logic.Indexes.Movie>(x => x.AutoMap())
            );
        }
    }
}