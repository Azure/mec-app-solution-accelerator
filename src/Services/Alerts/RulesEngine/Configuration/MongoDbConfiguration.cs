namespace Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Configuration
{
    public class MongoDbConfiguration
    {
        public string DatabaseName { get; set; }
        public string Hostname { get; set; }
        public int PortNumber { get; set; }
        public string UrlPrefix { get; set; }
    }
}
