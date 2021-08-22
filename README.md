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

## ElasticSearch
To get data from elastic search first you run elastic on local. These link show you how you can run elastic on locak
https://opensource.com/article/19/7/installing-elasticsearch-macos
Note: You can also load kibana on your local to operate elastic processes
For details; https://codingexplained.com/dev-ops/mac/installing-kibana-for-elasticsearch-on-os-x

After you bind elasticsearch and kafka, you run application and test it.

