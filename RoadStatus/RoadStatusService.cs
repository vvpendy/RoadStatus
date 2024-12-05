using System;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace RoadStatus{
    public class RoadStatusService
    {
        private readonly string BaseUrl = "https://api.tfl.gov.uk/Road/";
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public RoadStatusService(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public async Task<RoadStatusResponse> GetRoadStatusAsync(string roadId)
        {
                string requestUrl = $"{BaseUrl}{roadId}?app_id={_configuration["AppId"]}&app_key={_configuration["AppKey"]}";
                HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    JArray roadData = JArray.Parse(jsonResponse);
                    var road = roadData[0];
                    return new RoadStatusResponse{
                        DisplayName=road["displayName"]?.ToString(),
                        StatusSeverity=road["statusSeverity"]?.ToString(),
                        StatusSeverityDescription=road["statusSeverityDescription"]?.ToString(),
                        ExitCode=0
                    };
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new RoadStatusResponse{
                        ErroCode=$"{roadId} is not a valid road",
                        ExitCode=1
                    };
                }
                else
                {
                    return new RoadStatusResponse{
                        ErroCode=$"Unexpected error: {response.StatusCode}",
                        ExitCode=1
                    };
               
                }
        }
    }
}