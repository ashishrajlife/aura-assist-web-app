using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;

public partial class UserDashboard : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["UserId"] == null)
        {
            Response.Redirect("Login.aspx");
            return;
        }
        if (!IsPostBack)
        {
            CheckAndShuffleHoroscopes();
            LoadData();
        }
    }

    private void CheckAndShuffleHoroscopes()
    {
        string cs = ConfigurationManager.ConnectionStrings["AstrologyDB"].ConnectionString;
        using (SqlConnection con = new SqlConnection(cs))
        {
            // Check if today's horoscope exists
            SqlCommand checkCmd = new SqlCommand(
                "SELECT COUNT(*) FROM DailyHoroscope WHERE CAST(PredictionDate AS DATE) = CAST(GETDATE() AS DATE)",
                con);
            con.Open();
            int count = (int)checkCmd.ExecuteScalar();

            // Agar today ka nahi hai to shuffle karo
            if (count == 0)
            {
                SqlCommand shuffleCmd = new SqlCommand("sp_ShuffleDailyHoroscopes", con);
                shuffleCmd.CommandType = CommandType.StoredProcedure;
                shuffleCmd.ExecuteNonQuery();
            }
        }
    }

    private void LoadData()
    {
        int userId = (int)Session["UserId"];
        string cs = ConfigurationManager.ConnectionStrings["AstrologyDB"].ConnectionString;

        using (SqlConnection con = new SqlConnection(cs))
        {
            con.Open();

            // User info
            SqlCommand cmd = new SqlCommand(
                "SELECT FullName, CreatedDate, LastLoginDate FROM Users WHERE UserId=@Id", con);
            cmd.Parameters.AddWithValue("@Id", userId);
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                lblName.Text = dr["FullName"].ToString();
                lblJoinDate.Text = Convert.ToDateTime(dr["CreatedDate"]).ToString("MMM yyyy");
                lblLastLogin.Text = dr["LastLoginDate"] != DBNull.Value
                    ? Convert.ToDateTime(dr["LastLoginDate"]).ToString("dd MMM yyyy")
                    : "First time!";
            }
            dr.Close();

            // Rashis
            SqlCommand r = new SqlCommand("SELECT * FROM Rashis ORDER BY RashiId", con);
            SqlDataAdapter da = new SqlDataAdapter(r);
            DataTable dt = new DataTable();
            da.Fill(dt);
            rptRashis.DataSource = dt;
            rptRashis.DataBind();
        }
    }
}