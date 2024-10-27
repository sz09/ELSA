using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Web.Http;

namespace IdentityServer.Clients
{
    /// <summary>
    /// POST methods
    /// </summary>
    public abstract partial class IntegrationClient
    {
        protected Task<HttpResponseMessage> PostAsync<T>(string url, T content)
        {
            var httpClient = _httpClientFactory.CreateClient();
            return httpClient.PostAsJsonAsync(url, content);
        }
        protected async Task<TRespone> PostAsync<TRequest, TRespone>(string url, TRequest content, Func<Exception> exeptionFunc)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var httpResponse = await httpClient.PostAsJsonAsync(requestUri: url, value: content);
            if (!httpResponse.IsSuccessStatusCode)
            {
                throw exeptionFunc();
            }

            return await httpResponse.Content.ReadFromJsonAsync<TRespone>();
        }

        class Error
        {
            public string Code { get; set; }
            public string Description { get; set; }
        }

        protected async Task<TRespone> PostAsync<TRequest, TRespone>(string url, TRequest content)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var httpResponse = await httpClient.PostAsJsonAsync(requestUri: url, value: content);
            if (!httpResponse.IsSuccessStatusCode)
            {
                var errors = await httpResponse.Content.ReadAsAsync<HttpError>();
                var errorValues = errors.SelectMany(d =>
                {
                    var errorStr = d.Value.ToString();
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<Error[]>(errorStr);
                }).ToArray();

                throw new InvalidOperationException(string.Join(", ", errorValues.Select(d => $"{d.Code} {d.Description}")));
            }

            return await httpResponse.Content.ReadFromJsonAsync<TRespone>();
        }

        protected async Task<string> PostAsync<TRequest>(string url, TRequest content, Func<Exception> exeptionFunc)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var httpResponse = await httpClient.PostAsJsonAsync(requestUri: url, value: content);
            if (!httpResponse.IsSuccessStatusCode)
            {
                throw exeptionFunc();
            }

            return await httpResponse.Content.ReadAsStringAsync();
        }

        private JsonContent CreateJsonContent<TRequest>(TRequest content)
        {
            return JsonContent.Create(content, mediaType: new MediaTypeHeaderValue("application/json"));
        }
    }
}
