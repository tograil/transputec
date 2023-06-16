namespace CC.Authority.Implementation.Config
{
    public class CrisesControlServerConfig
    {
        public const string Name = "CrisesControlServer";
        public string ApiEndpoint { get; set; }
        public string ApiSecret { get; set; }
        public string AppPath { get; set; }
    }
}