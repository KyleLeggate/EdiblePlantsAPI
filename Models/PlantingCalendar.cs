using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdiblePlantsAPI.Models
{
	public class PlantingCalendar
	{
		public string plant { get; set; }
		public int indoorStart { get; set; }
		public int indoorEnd { get; set; }
		public int outdoorStart { get; set; }
		public int outdoorEnd { get; set; }
		public int harvestStart { get; set; }
		public int harvestEnd { get; set; }
	}
}
