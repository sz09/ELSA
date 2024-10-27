namespace ELSA.IdentityServer.Clients
{
    /// <summary>
    /// PUT methods
    /// </summary>
    public abstract partial class IntegrationClient
    {
        protected readonly IHttpClientFactory _httpClientFactory;

        protected IntegrationClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        protected async Task<TRespone> PutAsync<TRequest, TRespone>(string url, TRequest content, Func<Exception> exeptionFunc)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var httpResponse = await httpClient.PutAsJsonAsync(url, content);
            if (!httpResponse.IsSuccessStatusCode)
            {
                throw exeptionFunc();
            }

            return await httpResponse.Content.ReadFromJsonAsync<TRespone>();
        }

        protected async Task PutAsync<TRequest>(string url, TRequest content, Func<Exception> exeptionFunc)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var httpResponse = await httpClient.PutAsJsonAsync(url, content);
            if (!httpResponse.IsSuccessStatusCode)
            {
                throw exeptionFunc();
            }
        }
    }
}