namespace ControlPlane.API.Settings
{
    public class ClientCredentialsSettings
    {
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

        public string Authority
        {
            get
            {
                return "https://login.microsoftonline.com/" + TenantId;
            }
        }
    }
}