namespace RulesEngine.Configuration
{
    public class MqttConfig
    {
        public static string SectionName => "Mqtt";

        public string ConnectionString { get; set; }

        public int Port { get; set; }
    }
}
