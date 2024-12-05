// See https://aka.ms/new-console-template for more information
using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RoadStatus
{
    class Program
    {
        static async Task Main(string[] args)
        {
             var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory) // Set base path to the app directory
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true) // Add JSON configuration file
                .Build();

            if (args.Length != 1)
            {
                Console.WriteLine("Usage: RoadStatus <road_id>");
                Environment.Exit(1);
            }

            string roadId = args[0];
            try
            {
                var roadStatusService = new RoadStatusService(config,new HttpClient());
                var roadStatusResponse =await roadStatusService.GetRoadStatusAsync(roadId);
                if(roadStatusResponse.ExitCode==0){
                    Console.WriteLine($"The status of the {roadStatusResponse.DisplayName} is as follows:");
                    Console.WriteLine($"    Road Status is {roadStatusResponse.StatusSeverity}");
                    Console.WriteLine($"    Road Status Description is {roadStatusResponse.StatusSeverityDescription}");
                }else{
                    Console.WriteLine(roadStatusResponse.ErroCode);
                }

                Environment.Exit(roadStatusResponse.ExitCode);
               
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                Environment.Exit(1);
            }
        }
    }
}