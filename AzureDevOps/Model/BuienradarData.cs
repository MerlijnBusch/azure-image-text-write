using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDevOps.Model
{
    public class Buienradar
    {
        public string? copyright { get; set; }
        public string? terms { get; set; }
    }

    public class StationMeasurement
    {
        public int? stationid { get; set; }
        public string? stationname { get; set; }
        public double? lat { get; set; }
        public double? lon { get; set; }
        public DateTime? timestamp { get; set; }
        public string? weatherdescription { get; set; }
        public string? iconurl { get; set; }
        public string? graphUrl { get; set; }
        public string? winddirection { get; set; }
        public double? temperature { get; set; }
        public double? groundtemperature { get; set; }
        public double? feeltemperature { get; set; }
        public double? windgusts { get; set; }
        public double? windspeed { get; set; }
        public int? windspeedBft { get; set; }
        public double? humidity { get; set; }
        public double? precipitation { get; set; }
        public double? sunpower { get; set; }
        public double? rainFallLast24Hour { get; set; }
        public double? rainFallLastHour { get; set; }
        public double? winddirectiondegrees { get; set; }
    }

    public class Actual
    {
        public string? actualradarurl { get; set; }
        public DateTime? sunrise { get; set; }
        public DateTime? sunset { get; set; }
        public List<StationMeasurement>? stationmeasurements { get; set; }
    }

    public class BuienradarData
    {
        public Buienradar? buienradar { get; set; }
        public Actual? actual { get; set; }
    }
}
