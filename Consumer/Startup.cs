using Consumer.Infrastructures.Extensions;
using Consumer.Utils;
using Logic.Business.Service.Kafka.Handlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AutoMapper;
using Logic.Business.Service;
using Logic.Business.Service.Interfaces;
using Logic.Events;
using Logic.Mappings;

namespace Consumer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddMvcOptions(o =>
            { 
                o.EnableEndpointRouting = false;
            });
            services.AddSingleton<IHostedService, KafkaConsumerHandler>();
            services.AddElasticsearch(Configuration);
            services.AddScoped<IElasticService, ElasticService>();
            services.AddAutoMapper(typeof(MovieMapping).GetType().Assembly);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionHandler>();
            app.UseMvc();
        }
    }
}