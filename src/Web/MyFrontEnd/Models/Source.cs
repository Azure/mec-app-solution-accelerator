using System;
namespace MyFrontEnd.Models
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


		public Source(string id, string name, string type, double latitude, double longitude)
		{
			Id = id;
			Name = name;
			Type = type;
			lat = latitude;
			lon = longitude;
		}
	}
}

