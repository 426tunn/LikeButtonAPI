using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LikeButtonAPI.Entities
{
    public class RateLimitingConfiguration
    {
            public int PerMinuteLimit { get; set; } = 10;
            public int PerHourLimit { get; set; } = 100;
            public int PerDayLimit { get; set; } = 1000;
    }
}