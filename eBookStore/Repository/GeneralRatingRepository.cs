using eBookStore.Models.ViewModels;
using Microsoft.Data.SqlClient;
using eBookStore.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace eBookStore.Repository;
public class GeneralRatingRepository
{
	private string? connectionString;

	public GeneralRatingRepository(string _connectionString)
	{
		connectionString = _connectionString;
	}


    //this function will add the GenralRating to the database
    public void AddGeneralRating(GeneralRatingModel generalRatingModel)
    {
        using SqlConnection connection = new SqlConnection(connectionString);
        connection.Open();
        StringBuilder sb = new StringBuilder();
        sb.Append("INSERT INTO GeneralRating (starRating, userId, createdAt) ");
        sb.Append("VALUES (@starRating, @userId, @createdAt)");
        String sql = sb.ToString();
        using SqlCommand command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@starRating", generalRatingModel.starRating);
        command.Parameters.AddWithValue("@userId", generalRatingModel.userId);
        command.Parameters.AddWithValue("@createdAt", generalRatingModel.createdAt);
        command.ExecuteNonQuery();
    }

    //this func to get all the general ratings from the database
    public List<GeneralRatingModel> GetAllGeneralRating()
    {
        List<GeneralRatingModel> generalRatingModels = new List<GeneralRatingModel>();
        using SqlConnection connection = new SqlConnection(connectionString);
        connection.Open();
        StringBuilder sb = new StringBuilder();
        sb.Append("SELECT id, starRating, userId, createdAt FROM GeneralRating");
        String sql = sb.ToString();
        using SqlCommand command = new SqlCommand(sql, connection);
        using SqlDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            GeneralRatingModel generalRatingModel = new GeneralRatingModel();
            generalRatingModel.id = reader.GetInt32(0);
            generalRatingModel.starRating = reader.GetInt32(1);
            generalRatingModel.userId = reader.GetInt32(2);
            generalRatingModel.createdAt = reader.GetDateTime(3);
            generalRatingModels.Add(generalRatingModel);
        }
        return generalRatingModels;
    }

    //this func to get rating as average from the database
    public float GetAverageRating()
    {
        using SqlConnection connection = new SqlConnection(connectionString);
        connection.Open();
        StringBuilder sb = new StringBuilder();
        sb.Append("SELECT AVG(CAST(starRating AS FLOAT)) FROM GeneralRating");
        String sql = sb.ToString();
        using SqlCommand command = new SqlCommand(sql, connection);
        using SqlDataReader reader = command.ExecuteReader();
        if (reader.Read())
        {
            return (float)reader.GetDouble(0);
        }
        return 0;
    }
   

    //this func check if user already rated the website
    public bool CheckIfUserRated(int userId)
    {
        using SqlConnection connection = new SqlConnection(connectionString);
        connection.Open();
        StringBuilder sb = new StringBuilder();
        sb.Append("SELECT * FROM GeneralRating WHERE userId = @userId");
        String sql = sb.ToString();
        using SqlCommand command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@userId", userId);
        using SqlDataReader reader = command.ExecuteReader();
        if (reader.Read())
        {
            return true;
        }
        return false;
    }

}