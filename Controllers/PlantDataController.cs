using EdiblePlantsAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace EdiblePlantsAPI.Controllers
{
	//URL: "api/PlantData"
	[Route("api/[controller]")]
	[ApiController]
	public class PlantDataController : ControllerBase
	{
		private string connectionString = "Data Source=edibleplants.database.windows.net;Initial Catalog=EdiblePlants;User ID=kyleleggate; Password=Manager10;";

		// GET: api/PlantData/[plant]
		[HttpGet("{plant}")]
		public async Task<ActionResult<PlantData>> GetPlantData(string plant)
		{
			//Setup the connection and command 
			SqlConnection cnn = new SqlConnection(connectionString);
			SqlCommand cmd = new SqlCommand("SELECT * FROM PlantData WHERE plant = '" + plant + "';", cnn);

			cnn.Open();
			//Executes the command and returns data to the datareader
			SqlDataReader data = cmd.ExecuteReader(); 

			if (data.HasRows == false) //Check if any record exists
			{
				data.Close();
				cmd.Dispose();
				cnn.Close();
				return NotFound("No records were found");
			}

			//Create a PlantData object and retrieve data from the datareader 
			PlantData plantData = new PlantData();

			data.Read();
			plantData.plant = data.GetString(0);
			plantData.temp = data.GetString(1);
			plantData.water = data.GetString(2);

			data.Close();
			cmd.Dispose();
			cnn.Close();

			return plantData;
		}
	}
}
