using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace AzureCertAuthClientConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Get data!");

            var payload = await GetApiDataUsingHttpClientHandler();

            Console.WriteLine("Success!");
        }

        private static async Task<string> GetApiDataUsingHttpClientHandler()
        {
            var cert = new X509Certificate2("client.pfx", "1234");
            var handler = new HttpClientHandler();
            handler.ClientCertificates.Add(cert);
            var client = new HttpClient(handler);

            var request = new HttpRequestMessage()
            {
                // You need to configure your server auth for this
                RequestUri = new Uri("https://apiconnectorclaims20211118132813.azurewebsites.net/api/Claims"),
                Method = HttpMethod.Get,
            };
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseContent);
                return responseContent;
            }

            throw new ApplicationException($"Status code: {response.StatusCode}, Error: {response.ReasonPhrase}");
        }
    }
}
