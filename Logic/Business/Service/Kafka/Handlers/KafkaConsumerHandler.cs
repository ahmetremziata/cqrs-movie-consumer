using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Logic.Business.Service.Interfaces;
using Logic.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace Logic.Business.Service.Kafka.Handlers
{
    public class KafkaConsumerHandler : IHostedService
    {
        private readonly IConfiguration _configuration;
        private readonly IElasticService _elasticService;
        
        public KafkaConsumerHandler(IConfiguration configuration, IElasticService elasticService)
        {
            _configuration = configuration;
            _elasticService = elasticService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var conf = new ConsumerConfig
            {
                GroupId = _configuration["MovieActivatedGroupName"],
                BootstrapServers = _configuration["KafkaBootstrapServers"],
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            
            using (var builder = new ConsumerBuilder<Ignore, 
                string>(conf).Build())
            {
                builder.Subscribe(_configuration["MovieActivatedTopicName"]);
                var cancelToken = new CancellationTokenSource();
                while (true)
                {
                    var consumer = builder.Consume(cancelToken.Token);
                    try
                    {
                        _elasticService.InsertMovie(consumer.Message.Value);
                    }
                    catch (Exception exception)
                    {
                        //builder.Close();
                    }
                }
            }
            
            return Task.CompletedTask;
        }
        
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}