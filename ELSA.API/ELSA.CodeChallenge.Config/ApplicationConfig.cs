namespace ELSA.Config;
public class ApplicationConfig: IApplicationConfig
{
    public MongoDbConfig MongoDbConfig { get; set; }
    public IdentityServerConfig JwtSettings { get; set; }

    public MongoDbConfig IdentityServer { get; set; }

    public BlobContainerConfig Blob { get; set; }
}

public class MongoDbConfig
{
    public string ConnectionString { get; set; }
    public string DatabaseNamespace { get; set; }
}

public class IdentityServerConfig
{
    public string ApiName { get; set; }
    public string Authority { get; set; }
}

public class BlobContainerConfig
{
    public string AzureBlobConnectionString { get; set; }
    public string AzureBlobContainer { get; set; }

}
