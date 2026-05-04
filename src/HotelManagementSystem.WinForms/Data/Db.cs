using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace HotelManagementSystem.WinForms.Data;

public static class Db
{
    private static string ConnectionString =>
        ConfigurationManager.ConnectionStrings["HotelDb"]?.ConnectionString
        ?? throw new InvalidOperationException("Missing connection string 'HotelDb' in App.config");

    public static DataTable ExecuteSelect(string sql, params SqlParameter[] parameters)
    {
        using var conn = new SqlConnection(ConnectionString);
        using var cmd = new SqlCommand(sql, conn);
        if (parameters is { Length: > 0 })
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
        if (parameters is { Length: > 0 })
            cmd.Parameters.AddRange(parameters);

        return cmd.ExecuteNonQuery();
    }
}
