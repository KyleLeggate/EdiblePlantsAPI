using EdiblePlantsAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EdiblePlantsAPI.Controllers
{
	//URL: "api/UserData"
	[Route("api/[controller]")]
	[ApiController]
	public class UserDataController : ControllerBase
	{
		private string connectionString = "Data Source=edibleplants.database.windows.net;Initial Catalog=EdiblePlants;User ID=kyleleggate;Password=Manager10;";

		// GET: api/UserData/[userID]
		// READ
		[HttpGet("{userID}")]
		public async Task<ActionResult<IEnumerable<UserData>>> GetUserPlants(string userID)
		{
			//Setup the connection and command 
			SqlConnection cnn = new SqlConnection(connectionString);
			SqlCommand cmd = new SqlCommand("SELECT * FROM UserData WHERE userID = '" + userID + "';", cnn);

			cnn.Open();
			SqlDataReader data = cmd.ExecuteReader(); //Executes the command and returns data to the datareader

			if (data.HasRows == false) //Check if any record exists
			{
				data.Close();
				cmd.Dispose();
				cnn.Close();
				return NotFound("No records were found");
			}

			//Create a UserData object and add data from the datareader 
			List<UserData> plants = new List<UserData>();
			UserData tmp;

			while (data.Read()) //add each record from the datareader
			{
				tmp = new UserData();
				tmp.entryID = data.GetInt32(0);
				tmp.userID = data.GetString(1);
				tmp.plant = data.GetString(2);

				plants.Add(tmp);
			}

			data.Close();
			cmd.Dispose();

			//For each of these plants, get their data as well [COULD BE IMPROVED WITH BETTER INITIAL SQL STATEMENT, TRY UNION?]
			for(int i = 0; i < plants.Count; i++)
			{
				cmd = new SqlCommand("SELECT * FROM PlantData WHERE plant = '" + plants[i].plant + "';", cnn);
				
				data = cmd.ExecuteReader(); //Executes the command and returns data to the datareader

				if (data.HasRows == false) //Check if any record exists
				{
					data.Close();
					cmd.Dispose();
					cnn.Close();
					return NotFound("No records were found");
				}

				data.Read();

				plants[i].temp = data.GetString(1);
				plants[i].water = data.GetString(2);

				data.Close();
				cmd.Dispose();
			}

			cnn.Close();

			return plants;
		}

		// POST: api/UserData?userID=*&plant=*
		[HttpPost]
		public async Task<ActionResult<UserData>> AddPlant([FromQuery] string userID,
															[FromQuery] string plant)
		{
			//Setup the connection and command 
			SqlConnection cnn = new SqlConnection(connectionString);
			//First SQL statement checks if the plant already exists
			SqlCommand cmd = new SqlCommand("SELECT * FROM UserData WHERE userID = '" + userID + "' AND plant = '" + plant + "';", cnn);

			cnn.Open();
			SqlDataReader data = cmd.ExecuteReader(); //Executes the command

			if (data.HasRows == true) //This means that the plant has already been added
			{
				data.Close();
				cmd.Dispose();
				cnn.Close();
				return Ok("0");
			}
			data.Close();
			cmd.Dispose();

			//Change the sql command
			cmd = new SqlCommand("INSERT INTO UserData(userID, plant) VALUES('" + userID + "', '" + plant + "');", cnn);

			cmd.ExecuteNonQuery(); //Executes the command
			cmd.Dispose();
			cnn.Close();

			return Ok("1");
		}

		// DELETE: api/UserData?userID=***&plant=***
		[HttpDelete]
		public async Task<ActionResult<UserData>> DeletePlant([FromQuery] string userID, [FromQuery] string plant)
		{
			//Setup the connection and command
			SqlConnection cnn = new SqlConnection(connectionString);
			//First SQL statement checks if the plant exists
			SqlCommand cmd = new SqlCommand("SELECT * FROM UserData WHERE userID = '" + userID + "' AND plant = '" + plant + "';", cnn);

			cnn.Open();
			SqlDataReader data = cmd.ExecuteReader(); //Executes the command and returns data to the datareader

			//Check if any record exists
			if (data.HasRows == false)
			{
				data.Close();
				cmd.Dispose();
				cnn.Close();
				return NotFound("0");
			}

			data.Close();
			cmd.Dispose();

			//Change the command to delete the record
			cmd = new SqlCommand("DELETE FROM UserData WHERE userID = '" + userID + "' AND plant = '" + plant + "';", cnn);
			cmd.ExecuteNonQuery(); //Executes the command

			cmd.Dispose();
			cnn.Close();

			return Ok("1");
		}
	}
}
