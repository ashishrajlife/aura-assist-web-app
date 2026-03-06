using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class AdminDashboard : Page
{
    string cs = ConfigurationManager.ConnectionStrings["AstrologyDB"].ConnectionString;

    protected void Page_Load(object sender, EventArgs e)
    {
        // Only Admin (RoleId=1) allowed
        if (Session["UserId"] == null || Session["RoleId"] == null || (int)Session["RoleId"] != 1)
        {
            Response.Redirect("Login.aspx");
            return;
        }
        if (!IsPostBack)
        {
            LoadStats();
            LoadRashiDropdown();
            LoadUsers();
            LoadAllHoroscopes();
            LoadSearchRashiDropdown();
            txtDate.Text = DateTime.Today.ToString("yyyy-MM-dd");

            // Check and shuffle if needed
            CheckAndShuffleHoroscopes();
        }
    }

    private void CheckAndShuffleHoroscopes()
    {
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

    private void LoadStats()
    {
        using (SqlConnection con = new SqlConnection(cs))
        {
            con.Open();
            lblTotalUsers.Text = new SqlCommand("SELECT COUNT(*) FROM Users WHERE RoleId=2", con).ExecuteScalar().ToString();

            SqlCommand today = new SqlCommand(
                "SELECT ISNULL(TotalLogins,0), ISNULL(TotalRegistrations,0) FROM DailyStats WHERE StatDate=CAST(GETDATE() AS DATE)", con);
            SqlDataReader dr = today.ExecuteReader();
            if (dr.Read()) { lblTodayLogins.Text = dr[0].ToString(); lblTodayReg.Text = dr[1].ToString(); }
            else { lblTodayLogins.Text = "0"; lblTodayReg.Text = "0"; }
            dr.Close();

            lblHoroscopeCount.Text = new SqlCommand(
                "SELECT COUNT(*) FROM UserActivityLogs WHERE ActivityType LIKE 'ViewHoroscope%' AND CAST(ActivityDate AS DATE)=CAST(GETDATE() AS DATE)", con)
                .ExecuteScalar().ToString();
        }
    }

    private void LoadRashiDropdown()
    {
        using (SqlConnection con = new SqlConnection(cs))
        {
            SqlCommand cmd = new SqlCommand("SELECT RashiId, RashiEnglishName FROM Rashis ORDER BY RashiId", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            ddlRashi.DataSource = dt;
            ddlRashi.DataTextField = "RashiEnglishName";
            ddlRashi.DataValueField = "RashiId";
            ddlRashi.DataBind();
        }
    }

    private void LoadUsers()
    {
        using (SqlConnection con = new SqlConnection(cs))
        {
            SqlCommand cmd = new SqlCommand(
                "SELECT UserId, FullName, Email, CreatedDate, LastLoginDate, IsActive FROM Users WHERE RoleId=2 ORDER BY CreatedDate DESC", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            gvUsers.DataSource = dt;
            gvUsers.DataBind();
        }
    }

   private void LoadAllHoroscopes(string searchDate = "", int searchRashiId = 0)
{
    using (SqlConnection con = new SqlConnection(cs))
    {
        string query = @"
            SELECT h.*, r.RashiEnglishName,
                CASE 
                    WHEN h.IsShuffled = 1 THEN '🔄 Shuffled'
                    ELSE '✏️ Original'
                END AS ShuffledStatus
            FROM DailyHoroscope h
            INNER JOIN Rashis r ON h.RashiId = r.RashiId
            WHERE 1=1";
        
        if (!string.IsNullOrEmpty(searchDate))
            query += " AND CAST(h.PredictionDate AS DATE) = CAST(@SearchDate AS DATE)";
        
        if (searchRashiId > 0)
            query += " AND h.RashiId = @RashiId";
        
        query += " ORDER BY h.PredictionDate DESC, h.RashiId";
        
        SqlCommand cmd = new SqlCommand(query, con);
        
        if (!string.IsNullOrEmpty(searchDate))
            cmd.Parameters.AddWithValue("@SearchDate", DateTime.Parse(searchDate));
        
        if (searchRashiId > 0)
            cmd.Parameters.AddWithValue("@RashiId", searchRashiId);
        
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        DataTable dt = new DataTable();
        da.Fill(dt);
        
        gvHoroscopes.DataSource = dt;
        gvHoroscopes.DataBind();
    }
}

    // Helper method for Shuffled/Original text
    protected string GetShuffledText(object isShuffled)
    {
        if (isShuffled == DBNull.Value || isShuffled == null)
            return "✏️ Original";  // NULL ko Original consider karo

        bool shuffled = Convert.ToBoolean(isShuffled);
        return shuffled ? "🔄 Shuffled" : "✏️ Original";
    }

    // Search dropdown load karo
    private void LoadSearchRashiDropdown()
    {
        using (SqlConnection con = new SqlConnection(cs))
        {
            SqlCommand cmd = new SqlCommand("SELECT RashiId, RashiEnglishName FROM Rashis ORDER BY RashiId", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            ddlSearchRashi.DataSource = dt;
            ddlSearchRashi.DataTextField = "RashiEnglishName";
            ddlSearchRashi.DataValueField = "RashiId";
            ddlSearchRashi.DataBind();

            ddlSearchRashi.Items.Insert(0, new ListItem("-- All Rashis --", ""));
        }
    }

    protected void ddlEditRashi_DataBinding(object sender, EventArgs e)
    {
        DropDownList ddl = (DropDownList)sender;
        ddl.Items.Clear();

        using (SqlConnection con = new SqlConnection(cs))
        {
            SqlCommand cmd = new SqlCommand("SELECT RashiId, RashiEnglishName FROM Rashis ORDER BY RashiId", con);
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                ddl.Items.Add(new ListItem(reader["RashiEnglishName"].ToString(), reader["RashiId"].ToString()));
            }
            reader.Close();
        }
    }

    // Search button click
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        int rashiId = ddlSearchRashi.SelectedValue != "" ? int.Parse(ddlSearchRashi.SelectedValue) : 0;
        LoadAllHoroscopes(txtSearchDate.Text, rashiId);
    }

    // Reset button click
    protected void btnReset_Click(object sender, EventArgs e)
    {
        txtSearchDate.Text = "";
        ddlSearchRashi.SelectedIndex = 0;
        LoadAllHoroscopes();
    }

    // Row Editing
    protected void gvHoroscopes_RowEditing(object sender, GridViewEditEventArgs e)
    {
        gvHoroscopes.EditIndex = e.NewEditIndex;
        LoadAllHoroscopes();
    }

    // Row Canceling Edit
    protected void gvHoroscopes_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        gvHoroscopes.EditIndex = -1;
        LoadAllHoroscopes();
    }

    // Row Updating
    protected void gvHoroscopes_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        int horoscopeId = Convert.ToInt32(gvHoroscopes.DataKeys[e.RowIndex].Value);

        GridViewRow row = gvHoroscopes.Rows[e.RowIndex];

        DropDownList ddlRashi = (DropDownList)row.FindControl("ddlEditRashi");
        TextBox txtDate = (TextBox)row.FindControl("txtEditDate");
        TextBox txtPrediction = (TextBox)row.FindControl("txtEditPrediction");
        TextBox txtNumber = (TextBox)row.FindControl("txtEditNumber");
        TextBox txtColor = (TextBox)row.FindControl("txtEditColor");

        using (SqlConnection con = new SqlConnection(cs))
        {
            SqlCommand cmd = new SqlCommand(@"
                UPDATE DailyHoroscope 
                SET RashiId = @RashiId,
                    PredictionDate = @Date,
                    PredictionText = @Text,
                    LuckyNumber = @Number,
                    LuckyColor = @Color,
                    IsShuffled = 0  -- Manual update to original ban gaya
                WHERE HoroscopeId = @Id", con);

            cmd.Parameters.AddWithValue("@Id", horoscopeId);
            cmd.Parameters.AddWithValue("@RashiId", int.Parse(ddlRashi.SelectedValue));
            cmd.Parameters.AddWithValue("@Date", DateTime.Parse(txtDate.Text));
            cmd.Parameters.AddWithValue("@Text", txtPrediction.Text.Trim());
            cmd.Parameters.AddWithValue("@Number", txtNumber.Text.Trim());
            cmd.Parameters.AddWithValue("@Color", txtColor.Text.Trim());

            con.Open();
            cmd.ExecuteNonQuery();
        }

        gvHoroscopes.EditIndex = -1;
        LoadAllHoroscopes();
        ShowMsg("Horoscope updated successfully!", true);
    }

    // Row Deleting
    protected void gvHoroscopes_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        int horoscopeId = Convert.ToInt32(gvHoroscopes.DataKeys[e.RowIndex].Value);

        using (SqlConnection con = new SqlConnection(cs))
        {
            SqlCommand cmd = new SqlCommand("DELETE FROM DailyHoroscope WHERE HoroscopeId = @Id", con);
            cmd.Parameters.AddWithValue("@Id", horoscopeId);

            con.Open();
            cmd.ExecuteNonQuery();
        }

        LoadAllHoroscopes();
        ShowMsg("Horoscope deleted successfully!", true);
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand(@"
                    IF EXISTS (SELECT 1 FROM DailyHoroscope WHERE RashiId=@RashiId AND PredictionDate=@Date)
                        UPDATE DailyHoroscope 
                        SET PredictionText=@Text, LuckyNumber=@Num, LuckyColor=@Color,
                            IsShuffled = 0
                        WHERE RashiId=@RashiId AND PredictionDate=@Date
                    ELSE
                        INSERT INTO DailyHoroscope (RashiId, PredictionDate, PredictionText, LuckyNumber, LuckyColor, IsShuffled)
                        VALUES (@RashiId, @Date, @Text, @Num, @Color, 0)", con);

                cmd.Parameters.AddWithValue("@RashiId", int.Parse(ddlRashi.SelectedValue));
                cmd.Parameters.AddWithValue("@Date", DateTime.Parse(txtDate.Text));
                cmd.Parameters.AddWithValue("@Text", txtPrediction.Text.Trim());
                cmd.Parameters.AddWithValue("@Num", txtLuckyNumber.Text.Trim());
                cmd.Parameters.AddWithValue("@Color", txtLuckyColor.Text.Trim());

                con.Open();
                cmd.ExecuteNonQuery();

                ShowMsg("Horoscope save ho gaya!", true);
                LoadAllHoroscopes(); // Refresh the grid
            }
        }
        catch (Exception ex)
        {
            ShowMsg("Error: " + ex.Message, false);
        }
    }

    private void ShowMsg(string msg, bool ok)
    {
        pnlMsg.Visible = true;
        divMsg.InnerHtml = msg;
        divMsg.Style["background"] = ok ? "rgba(80,180,120,0.1)" : "rgba(220,100,100,0.1)";
        divMsg.Style["border-color"] = ok ? "rgba(80,180,120,0.3)" : "rgba(220,100,100,0.3)";
        divMsg.Style["color"] = ok ? "#7ecba0" : "#e07070";
    }
}