using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

// NOTE: No namespace — WebSite projects mein App_Code classes global hoti hain
public static class DatabaseHelper
{
    private static string ConnStr
    {
        get { return ConfigurationManager.ConnectionStrings["AstrologyDB"].ConnectionString; }
    }

    // Register user — calls RegisterUser stored procedure
    // Returns: 0=success, 1=email exists, -1=error
    public static int RegisterUser(string fullName, string email, string password)
    {
        try
        {
            using (SqlConnection con = new SqlConnection(ConnStr))
            {
                con.Open();

                // Email already registered?
                SqlCommand check = new SqlCommand("SELECT COUNT(1) FROM Users WHERE Email=@Email", con);
                check.Parameters.AddWithValue("@Email", email);
                int exists = (int)check.ExecuteScalar();
                if (exists > 0) return 1;

                // Call SP
                SqlCommand cmd = new SqlCommand("RegisterUser", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FullName", fullName);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@PasswordHash", password);
                cmd.ExecuteNonQuery();
                return 0;
            }
        }
        catch { return -1; }
    }

    // Login user — calls LoginUser stored procedure
    // Returns: UserId if success, 0=invalid credentials, -1=error
    public static int LoginUser(string email, string password)
    {
        try
        {
            using (SqlConnection con = new SqlConnection(ConnStr))
            {
                con.Open();

                // Call SP first (updates LastLoginDate + logs activity)
                SqlCommand sp = new SqlCommand("LoginUser", con);
                sp.CommandType = CommandType.StoredProcedure;
                sp.Parameters.AddWithValue("@Email", email);
                sp.Parameters.AddWithValue("@PasswordHash", password);
                sp.ExecuteNonQuery();

                // Get UserId
                SqlCommand get = new SqlCommand(
                    "SELECT UserId FROM Users WHERE Email=@Email AND PasswordHash=@Hash AND IsActive=1", con);
                get.Parameters.AddWithValue("@Email", email);
                get.Parameters.AddWithValue("@Hash", password);
                object result = get.ExecuteScalar();

                return result != null ? Convert.ToInt32(result) : 0;
            }
        }
        catch { return -1; }
    }

    // Get user full name by UserId
    public static string GetUserFullName(int userId)
    {
        try
        {
            using (SqlConnection con = new SqlConnection(ConnStr))
            {
                SqlCommand cmd = new SqlCommand("SELECT FullName FROM Users WHERE UserId=@Id", con);
                cmd.Parameters.AddWithValue("@Id", userId);
                con.Open();
                object result = cmd.ExecuteScalar();
                return result != null ? result.ToString() : "";
            }
        }
        catch { return ""; }
    }

    // Get all rashis for the grid
    public static DataTable GetAllRashis()
    {
        try
        {
            using (SqlConnection con = new SqlConnection(ConnStr))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Rashis ORDER BY RashiId", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }
        catch { return new DataTable(); }
    }

    public static int GetUserRole(int userId)
    {
        try
        {
            string cs = ConfigurationManager.ConnectionStrings["AstrologyDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("SELECT RoleId FROM Users WHERE UserId=@Id", con);
                cmd.Parameters.AddWithValue("@Id", userId);
                con.Open();
                object r = cmd.ExecuteScalar();
                return r != null ? (int)r : 2;
            }
        }
        catch { return 2; }
    }
}
