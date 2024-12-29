using eBookStore.Models.ViewModels;
using Microsoft.Data.SqlClient;
using eBookStore.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace eBookStore.Repository;
public class GeneralFeedbackModelRepository
{
	private string? connectionString;

	public GeneralFeedbackModelRepository(string _connectionString)
	{
		connectionString = _connectionString;
	}

    //this function will add the Genralfeedback to the database
    public void AddGeneralFeedback(GeneralFeedbackModel generalFeedbackModel)
    {
        using SqlConnection connection = new SqlConnection(connectionString);
        connection.Open();
        StringBuilder sb = new StringBuilder();
        sb.Append("INSERT INTO GeneralFeedback (userId, comment, createdAt) ");
        sb.Append("VALUES (@userId, @comment, @createdAt)");
        String sql = sb.ToString();
        using SqlCommand command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@userId", generalFeedbackModel.userId);
        command.Parameters.AddWithValue("@comment", generalFeedbackModel.comment);
        command.Parameters.AddWithValue("@createdAt", generalFeedbackModel.createdAt);
        command.ExecuteNonQuery();
    }


    //this function will get all the general feedbacks from the database
    public List<GeneralFeedbackModel> GetAllGeneralFeedback()
    {
        List<GeneralFeedbackModel> generalFeedbackModels = new List<GeneralFeedbackModel>();
        using SqlConnection connection = new SqlConnection(connectionString);
        connection.Open();
        StringBuilder sb = new StringBuilder();
        sb.Append("SELECT * FROM GeneralFeedback");
        String sql = sb.ToString();
        using SqlCommand command = new SqlCommand(sql, connection);
        using SqlDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            GeneralFeedbackModel generalFeedbackModel = new GeneralFeedbackModel();
            generalFeedbackModel.id = reader.GetInt32(0);
            generalFeedbackModel.userId = reader.GetInt32(1);
            generalFeedbackModel.comment = reader.GetString(2);
            generalFeedbackModel.createdAt = reader.GetDateTime(3);
            generalFeedbackModels.Add(generalFeedbackModel);
        }
        return generalFeedbackModels;
    }
}