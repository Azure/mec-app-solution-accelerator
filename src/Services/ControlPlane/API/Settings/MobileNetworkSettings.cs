namespace ControlPlane.API.Settings
{
    public class MobileNetworkSettings
    {
        public string SubscriptionId { get; set; }
        public string ResourceGroup { get; set; }
        public string Name { get; set; }
        public string AttachedDataNetwork { get; set; }
        public string Slice { get; set; }
    }
}