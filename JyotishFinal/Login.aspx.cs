using System;
using System.Web.UI;

public partial class Login : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["UserId"] != null)
            Response.Redirect("~/Default.aspx");
    }

    protected void btnLogin_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid) return;

        string email    = txtEmail.Text.Trim();
        string password = txtPassword.Text;

        int userId = DatabaseHelper.LoginUser(email, password);

        if (userId > 0)
        {
            Session["UserId"]   = userId;
            Session["UserName"] = DatabaseHelper.GetUserFullName(userId);
            Session["RoleId"] = DatabaseHelper.GetUserRole(userId);
            ShowMsg("✓ Login successful! Redirecting...", true);
            int role = (int)Session["RoleId"];
            Response.AddHeader("Refresh", role == 1 ? "1;url=AdminDashboard.aspx" : "1;url=UserDashboard.aspx");
        }
        else if (userId == 0)
        {
            ShowMsg("Email ya password galat hai. Dobara try karo.", false);
        }
        else
        {
            ShowMsg("Server error. Thodi der baad try karo.", false);
        }
    }

    private void ShowMsg(string text, bool ok)
    {
        pnlMsg.Visible = true;
        divMsg.InnerHtml = ok ? "&#10003; " + text : "&#10007; " + text;
        divMsg.Attributes["class"] = ok ? "msg msg-ok" : "msg msg-err";
    }
}
