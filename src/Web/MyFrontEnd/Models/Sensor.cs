using System;
namespace MyFrontEnd.Models
{
	public class Sensor : Source
	{
		public string SensorType { get; set; }

		public Sensor(string id, string name, string type, double latitude, double longitude, string sensorType) : base( name, type, latitude, longitude)
		{
			SensorType = sensorType;
		}
	}
}

