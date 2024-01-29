using System;
namespace Microsoft.MecSolutionAccelerator.AlertsUI.Models
{
	public class SensorModel : SourceModel
	{
		public string SensorType { get; set; }

		public SensorModel(string name, string type, double latitude, double longitude, string sensorType) : base(name, type, latitude, longitude)
		{
			SensorType = sensorType;
		}
	}
}

