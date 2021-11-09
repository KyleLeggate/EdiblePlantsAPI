using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdiblePlantsAPI.Models
{
	public class UserData
	{
		public int entryID { get; set; }
		public string userID { get; set; }
		public string plant { get; set; }
		public string temp { get; set; }
		public string water { get; set; }
	}
}
