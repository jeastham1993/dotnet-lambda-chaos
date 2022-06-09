using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using DotnetChaosLambda;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace DotnetChaosLambda.FunctionTest
{
    public class Function
    {
        private HttpClient _httpClient;
        private LambdaChaos _chaos;

        public Function()
        {
            this._httpClient = new HttpClient();
            this._chaos = new LambdaChaos();
        }
        
        public async Task<APIGatewayProxyResponse> LambdaHandler(APIGatewayProxyRequest evt, ILambdaContext context)
        {
            await this._chaos.UnleashChaos();
            
            var responseHeaders = new Dictionary<string, string>()
            {
                {"Content-Type", "application/json"},
                {"X-Custom-Header", "application/json"}
            };

            var result = await this.getPageContent("https://checkip.amazonaws.com");
            var output = $"{{\"message\": \"hello world\", \"location\": \"{result}\"}}";

            return new APIGatewayProxyResponse()
            {
                Headers = responseHeaders,
                Body = output,
                StatusCode = 200
            };
        }

        private async Task<string> getPageContent(string url) => await this._httpClient.GetStringAsync(new Uri(url));
    }
}