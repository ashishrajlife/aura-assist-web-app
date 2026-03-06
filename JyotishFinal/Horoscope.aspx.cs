using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;

public partial class Horoscope : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            int rashiId;
            if (int.TryParse(Request.QueryString["rashiId"], out rashiId))
                LoadHoroscope(rashiId);
            else
                Response.Redirect("Default.aspx");
        }
    }

    private void LoadHoroscope(int rashiId)
    {
        string cs = ConfigurationManager.ConnectionStrings["AstrologyDB"].ConnectionString;
        using (SqlConnection con = new SqlConnection(cs))
        {
            SqlCommand cmd = new SqlCommand(@"
            SELECT 
                h.PredictionText, h.LuckyNumber, h.LuckyColor,
                r.RashiEnglishName, r.RashiName, r.Symbol
            FROM DailyHoroscope h
            INNER JOIN Rashis r ON h.RashiId = r.RashiId
            WHERE h.RashiId = @RashiId
            AND CAST(h.PredictionDate AS DATE) = CAST(GETDATE() AS DATE)", con);

            cmd.Parameters.AddWithValue("@RashiId", rashiId);
            con.Open();

            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                pnlHoroscope.Visible = true;
                pnlNotFound.Visible = false;

                lblSymbol.Text = dr["Symbol"].ToString();
                lblRashiName.Text = dr["RashiEnglishName"].ToString() + " — " + dr["RashiName"].ToString();
                lblDate.Text = DateTime.Now.ToString("dd MMMM yyyy");
                lblPrediction.Text = dr["PredictionText"].ToString();
                lblLuckyNumber.Text = dr["LuckyNumber"].ToString();
                lblLuckyColor.Text = dr["LuckyColor"].ToString();

                // Log activity
                if (Session["UserId"] != null)
                    LogActivity((int)Session["UserId"], rashiId);
            }
            else
            {
                pnlNotFound.Visible = true;
                pnlHoroscope.Visible = false;
            }
            dr.Close();
        }
    }

    private void LogActivity(int userId, int rashiId)
    {
        try
        {
            string cs = ConfigurationManager.ConnectionStrings["AstrologyDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO UserActivityLogs (UserId, ActivityType) VALUES (@UserId, @Type)", con);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@Type", "ViewHoroscope_" + rashiId);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
        catch { }
    }
}