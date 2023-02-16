using System;
namespace Alerts.UI.Models
{
	public class Source
	{
		public Source(string name, string type, double lat, double lon)
		{
			Name = name ?? throw new ArgumentNullException(nameof(name));
			Type = type ?? throw new ArgumentNullException(nameof(type));
			Lat = lat;
			Lon = lon;
		}

		// public string Id { get; set; }
		public string Name { get; set; }
		public string Type { get; set; }
		public double Lat { get; set; }
		public double Lon { get; set; }
	}
}

