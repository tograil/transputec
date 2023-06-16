namespace CrisesControl.Auth.Config;

public class ServerCredentialsOptions
{
    public const string ServerCredentials = "ServerCredentials";
    public string Profile { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public int TokenExpiryInDays { get; set; }
    public string EncryptionKeyThumbprint { get; set; }
    public string SigningKeyThumbprint { get; set; }

}