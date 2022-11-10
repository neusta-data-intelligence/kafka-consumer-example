using Confluent.Kafka;

namespace KafkaConsumer;

static class Program
{
    public static void Main(string[] args)
    {
        var groupId = GetEnvironmentVariableOrThrow("KAFKA_GROUP_ID");
        var bootstrapServers = GetEnvironmentVariableOrThrow("KAFKA_BOOTSTRAP_SERVERS");
        var saslUserName = GetEnvironmentVariableOrThrow("KAFKA_SASL_USERNAME");
        var saslPassword = GetEnvironmentVariableOrThrow("KAFKA_SASL_PASSWORD");
        var topicName = GetEnvironmentVariableOrThrow("KAFKA_TOPIC_NAME");


        var conf = new ConsumerConfig
        {
            GroupId = groupId,
            BootstrapServers = bootstrapServers,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslMechanism = SaslMechanism.Plain,
            SaslUsername = saslUserName,
            SaslPassword = saslPassword
        };

        using (var c = new ConsumerBuilder<Ignore, string>(conf).Build())
        {
            c.Subscribe(topicName);

            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true; // prevent the process from terminating.
                cts.Cancel();
            };

            try
            {
                while (true)
                {
                    try
                    {
                        var cr = c.Consume(cts.Token);
                        Console.WriteLine($"Consumed message '{cr.Value}' at: '{cr.TopicPartitionOffset}'.");
                    }
                    catch (ConsumeException e)
                    {
                        Console.WriteLine($"Error occured: {e.Error.Reason}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Ensure the consumer leaves the group cleanly and final offsets are committed.
                c.Close();
            }
        }
    }

    private static string GetEnvironmentVariableOrThrow(string name) =>
        Environment.GetEnvironmentVariable(name) 
        ?? throw new ArgumentException($"{name} missing");
}