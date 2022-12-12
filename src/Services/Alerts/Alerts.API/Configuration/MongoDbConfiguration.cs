namespace Microsoft.MecSolutionAccelerator.Services.Alerts.Configuration
{
    public class MongoDbConfiguration
    {
        public string DatabaseName { get; set; }
        public string Hostname { get; set; }
        public int PortNumber { get; set; }
        public string UrlPrefix { get; set; }
    }
}
