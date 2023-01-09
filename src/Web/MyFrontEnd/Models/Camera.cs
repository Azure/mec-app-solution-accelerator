using System;
using System.Xml.Linq;

namespace MyFrontEnd.Models
{
	public class Camera : Source
	{
		public int Range { get; set; }
		public int Direction { get; set; }

		public Camera(string id, string name, string type, double latitude, double longitude, int range, int direction) : base( name, type, latitude, longitude)
		{
			Range = range;
			Direction = direction;
		}
	}
}

