using System;
using System.Xml.Linq;

namespace Microsoft.MecSolutionAccelerator.AlertsUI.Models
{
	public class CameraModel : SourceModel
	{
		public int Range { get; set; }
		public int Direction { get; set; }

		public CameraModel(string name, string type, double latitude, double longitude, int range, int direction) : base(name, type, latitude, longitude)
		{
			Range = range;
			Direction = direction;
		}
	}
}

