using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Configs
{
    public class EnvironmentConfig
    {
        public string DatabaseHost => Environment.GetEnvironmentVariable("DATABASE_HOST") ?? "localhost";
        public int DatabasePort => int.TryParse(Environment.GetEnvironmentVariable("DATABASE_PORT"), out var port) ? port : 5432;
        public string DatabaseName => Environment.GetEnvironmentVariable("DATABASE_NAME") ?? "defaultdb";
        public string DatabaseUser => Environment.GetEnvironmentVariable("DATABASE_USERNAME") ?? "postgres";
        public string DatabasePassword => Environment.GetEnvironmentVariable("DATABASE_PASSWORD") ?? "password";
        public string DatabaseProvider => Environment.GetEnvironmentVariable("DATABASE_PROVIDER") ?? "postgres";
        public string RabbitMqHost => Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "rabbit-mq";
        public int RabbitMqPort  => int.TryParse(Environment.GetEnvironmentVariable("RABBITMQ_PORT"), out var port) ? port : 5672;
        public string RabbitMqUser => Environment.GetEnvironmentVariable("RABBITMQ_USERNAME") ?? "username";
        public string RabbitMqPassword => Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD") ?? "password";

        public string RedisHost => Environment.GetEnvironmentVariable("REDIS_HOST") ?? "redis";
        public string RedisPassword => Environment.GetEnvironmentVariable("REDIS_PASSWORD") ?? "default";
        public int RedisPort => int.TryParse(Environment.GetEnvironmentVariable("REDIS_PORT"), out var port) ? port : 6379;
        
        public string GoogleApiKey => Environment.GetEnvironmentVariable("GOOGLE_API_KEY") ?? "your-google-api-key";
        public string FoursquareApiKey => Environment.GetEnvironmentVariable("FOURSQUARE_API_KEY") ?? "your-foursquare-api-key";
        public string ElasticSearchHost => Environment.GetEnvironmentVariable("ELASTICSEARCH_HOSTS") ??  "elastic-search-host";
        public string ElasticSearchUsername => Environment.GetEnvironmentVariable("ELASTICSEARCH_USERNAME") ?? "elastic-search-username";
        public string ElasticSearchPassword => Environment.GetEnvironmentVariable("ELASTICSEARCH_PASSWORD") ?? "elastic-search-password";
        public string ElasticSearchDefaultIndex =>  Environment.GetEnvironmentVariable("ELASTICSEARCH_INDEX");
    }
}