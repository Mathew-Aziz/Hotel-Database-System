using System.Data;
using Microsoft.Data.SqlClient;
using System.Configuration;

namespace HotelManagementSystem.WinForms.Data;

public static class Db
{
    // Change Data Source to match your SQL Server (localhost, .\SQLEXPRESS, etc.)
    private static string ConnectionString = 
    "Data Source=localhost;Initial Catalog=HotelDB;Integrated Security=True;TrustServerCertificate=True;";

    public static DataTable ExecuteSelect(string sql, params SqlParameter[] parameters)
    {
        using var conn = new SqlConnection(ConnectionString);
        using var cmd = new SqlCommand(sql, conn);
        if (parameters != null && parameters.Length > 0)
            cmd.Parameters.AddRange(parameters);

        using var da = new SqlDataAdapter(cmd);
        var dt = new DataTable();
        da.Fill(dt);
        return dt;
    }

    public static int ExecuteNonQuery(string sql, params SqlParameter[] parameters)
    {
        using var conn = new SqlConnection(ConnectionString);
        conn.Open();
        using var cmd = new SqlCommand(sql, conn);
        if (parameters != null && parameters.Length > 0)
            cmd.Parameters.AddRange(parameters);
        return cmd.ExecuteNonQuery();
    }

    public static object ExecuteScalar(string sql, params SqlParameter[] parameters)
    {
        using var conn = new SqlConnection(ConnectionString);
        conn.Open();
        using var cmd = new SqlCommand(sql, conn);
        if (parameters != null && parameters.Length > 0)
            cmd.Parameters.AddRange(parameters);
        return cmd.ExecuteScalar();
    }
}