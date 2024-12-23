using Microsoft.Data.SqlClient;
using eBookStore.Models.ViewModels;
using eBookStore.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace eBookStore.Repository;
public class PersonalLibraryRepository
{
	private string? connectionString;

	public PersonalLibraryRepository(string _connectionString)
	{
		connectionString = _connectionString;
	}
    

	public List<PersonalLibraryModel> GetPersonalLibrary(int userId)
	{
		List<PersonalLibraryModel> personalLibraryList = new();
		using (SqlConnection connection = new(connectionString))
		{
			connection.Open();
			StringBuilder sb = new();
			sb.Append("SELECT * FROM PersonalLibrary WHERE userId = @userId");
			string? sql = sb.ToString();
			using (SqlCommand command = new(sql, connection))
			{
				command.Parameters.AddWithValue("@userId", userId);
				using (SqlDataReader dataReader = command.ExecuteReader())
				{
					while (dataReader.Read())
					{
						PersonalLibraryModel personalLibrary = new()
						{
							userId = Convert.ToInt32(dataReader["userId"]),
							bookId = Convert.ToInt32(dataReader["bookId"]),
							createAt = Convert.ToDateTime(dataReader["createAt"])
						};
						personalLibraryList.Add(personalLibrary);
					}
				}
			}
		}
		return personalLibraryList;
	}
}