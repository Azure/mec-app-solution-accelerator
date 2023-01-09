using System;
namespace MyFrontEnd.Models
{
	public class Source
	{
		// public string Id { get; set; }
		public string Name { get; set; }
		public string Type { get; set; }
		public double lat { get; set; }
		public double lon { get; set; }

		public Source(string name, string type, double latitude, double longitude)
		{
			Name = name;
			Type = type;
			lat = latitude;
			lon = longitude;
		}
	}
}

