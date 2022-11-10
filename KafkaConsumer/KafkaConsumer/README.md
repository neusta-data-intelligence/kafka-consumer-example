# Simple Kafka Consumer Example
Consumes with [Sasl](https://en.wikipedia.org/wiki/Simple_Authentication_and_Security_Layer) mechanism Plaintext and security protocol SaslSsl.

## Usage
Build and run via docker.
- Copy file `env-template` to `env` and fill in the blanks
- Run inside **Solution directory**
```shell
$ docker build -f KafkaConsumer/Dockerfile  -t simple-kafka-consumer-manual . \
&& docker run --env-file  KafkaConsumer/env simple-kafka-consumer-manual
```