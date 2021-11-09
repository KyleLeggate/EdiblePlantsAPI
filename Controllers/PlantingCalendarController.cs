using EdiblePlantsAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EdiblePlantsAPI.Controllers
{
	//URL: "api/PlantingCalendar"
	[Route("api/[controller]")]
	[ApiController]
	public class PlantingCalendarController : ControllerBase
	{
		private string connectionString = "Data Source=edibleplants.database.windows.net;Initial Catalog=EdiblePlants;User ID=kyleleggate; Password=Manager10;";

		// GET: api/PlantingCalendar/[plant]
		// READ BY PLANT
		[HttpGet("{plant}")]
		public async Task<ActionResult<PlantingCalendar>> GetMonthsByPlant(string plant)
		{
			//Setup the connection and command
			SqlConnection cnn = new SqlConnection(connectionString);
			SqlCommand cmd = new SqlCommand("SELECT * FROM PlantingCalendar WHERE plant = '" + plant + "';", cnn);

			cnn.Open();
			SqlDataReader data = cmd.ExecuteReader(); //Executes the command and returns data to the datareader

			if (data.HasRows == false) //Check if any record exists
			{
				data.Close();
				cmd.Dispose();
				cnn.Close();
				return NotFound("No records were found");
			}

			//Create a PlantingCalendar object and add data from the datareader
			PlantingCalendar dates = new PlantingCalendar();

			data.Read();
			dates.plant = data.GetString(0);
			dates.indoorStart = data.GetInt32(1);
			dates.indoorEnd = data.GetInt32(2);
			dates.outdoorStart = data.GetInt32(3);
			dates.outdoorEnd = data.GetInt32(4);
			dates.harvestStart = data.GetInt32(5);
			dates.harvestEnd = data.GetInt32(6);

			data.Close();
			cmd.Dispose();
			cnn.Close();

			return dates;
		}

		// GET: api/PlantingCalendar/month/[month index]
		// READ BY MONTH
		[HttpGet("month/{month}")]
		public async Task<ActionResult<IEnumerable<string>>> GetPlantsByMonth(int month)
		{
			//Setup the connection and command 
			SqlConnection cnn = new SqlConnection(connectionString);

			//The SQL command combines a query for all the plants that have an indoorStart and indoorEnd which the input fits into, and
			//	a query for all the plants with no indoor months, but do have proper a outdoorStart and outdoorEnd
			SqlCommand cmd = new SqlCommand("SELECT * FROM PlantingCalendar WHERE indoorStart <= " + month + " AND indoorEnd >= " + month + 
											" UNION " +
											"SELECT * FROM PlantingCalendar WHERE indoorStart = 0 AND outdoorStart <= " + month + " AND outdoorEnd >=" + month + ";", cnn);

			cnn.Open();
			SqlDataReader data = cmd.ExecuteReader(); //Executes the command and returns data to the datareader

			if (data.HasRows == false) //Check if any record exists
			{
				data.Close();
				cmd.Dispose();
				cnn.Close();
				return NotFound("0");
			}

			//Create a list to store the plants from the datareader 
			List<string> plants = new List<string>();

			while(data.Read())
			{
				plants.Add(data.GetString(0));
			}

			data.Close();
			cmd.Dispose();
			cnn.Close();

			//Randomly choose 5 plants, if there are more than five plants total
			if(plants.Count > 5)
			{
				Random r = new Random();
				int index;
				while(plants.Count > 5)
				{
					index = r.Next(plants.Count);
					plants.RemoveAt(index);
				}
			}

			return plants;
		}
	}
}
