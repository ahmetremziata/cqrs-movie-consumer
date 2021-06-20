using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Logic.Business.Service.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Logic.Business.Service.Kafka.Handlers
{
    public class MovieConsumerHandler : IHostedService
    {
        private readonly IConfiguration _configuration;
        private readonly IElasticService _elasticService;
        
        public MovieConsumerHandler(IConfiguration configuration, IElasticService elasticService)
        {
            _configuration = configuration;
            _elasticService = elasticService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Thread activatedThread = new Thread(StartActivatedConsumer)
            {
                Name = "ActivatedThread"
            };
            
            Thread deactivatedThread = new Thread(StartDeactivatedConsumer)
            {
                Name = "DeactivatedThread"
            };
            
            activatedThread.Start();  
            deactivatedThread.Start();
            
            return Task.CompletedTask;
        }

        private void StartActivatedConsumer()
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
                        builder.Close();
                    }
                }
            }
        }

        private void StartDeactivatedConsumer()
        {
            var conf = new ConsumerConfig
            {
                GroupId = _configuration["MovieDeactivatedGroupName"],
                BootstrapServers = _configuration["KafkaBootstrapServers"],
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            
            using (var builder = new ConsumerBuilder<Ignore, 
                string>(conf).Build())
            {
                builder.Subscribe(_configuration["MovieDeactivatedTopicName"]);
                var cancelToken = new CancellationTokenSource();
                while (true)
                {
                    var consumer = builder.Consume(cancelToken.Token);
                    try
                    {
                        _elasticService.DeleteMovie(consumer.Message.Value);
                    }
                    catch (Exception exception)
                    {
                        builder.Close();
                    }
                }
            }
        }
        
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}