using System;
using Microsoft.Extensions.Configuration;

namespace RoadStatus{

    public class AppConfiguration 
    {
        public required string AppId { get; set; }
        public required string AppKey { get; set; }
    }
}