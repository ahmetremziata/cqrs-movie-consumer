# cqrs-movie-consumer
A consumer example for cqrs and event sourcing

## Dependencies
- Kafka (For event sourcing)
- ElasticSearch (Nosql for query)

## Usage
First of all you should standup below dependencies;
- kafka in localhost:9092 
- elasticsearch in localhost:9200

Then run application and test consumer for receiving data to elastic. If everything is ok, you can query these instructions from dev tools

```
GET /movies/_settings

GET /movies/_search
{
  "query": {
    "match_all": {}
  }
}

```

