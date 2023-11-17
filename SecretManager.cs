using Microsoft.Extensions.Configuration;

namespace AI_NPCs
{

    public class SecretManager
    {
        private readonly IConfiguration _configuration;

        public SecretManager()
        {
            var builder = new ConfigurationBuilder().AddUserSecrets<SecretManager>();
            _configuration = builder.Build();
        }

        public string GetSecretValue(string key)
        {
            return _configuration[key];
        }
    }
}
