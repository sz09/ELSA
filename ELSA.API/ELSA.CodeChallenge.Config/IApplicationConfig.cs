namespace ELSA.Config
{
    public interface IApplicationConfig
	{
		MongoDbConfig MongoDbConfig { get; }
        MongoDbConfig IdentityServer { get; }
        IdentityServerConfig JwtSettings { get; }
        public BlobContainerConfig Blob { get; }
    }
}

