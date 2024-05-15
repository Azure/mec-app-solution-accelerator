namespace Alerts.API.Configuration
{
    public class MqttConfig
    {
        public static string SectionName => "Mqtt";

        public string ConnectionString { get; set; } = string.Empty;

        public int Port { get; set; }
    }
}
