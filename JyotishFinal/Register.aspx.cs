using System;
using System.Web.UI;

public partial class Register : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["UserId"] != null)
            Response.Redirect("~/Default.aspx");
    }

    protected void btnRegister_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid) return;

        string fullName = txtFullName.Text.Trim();
        string email    = txtEmail.Text.Trim();
        string password = txtPassword.Text;

        int result = DatabaseHelper.RegisterUser(fullName, email, password);

        switch (result)
        {
            case 0:
                ShowMsg("Account ban gaya! Login page pe ja rahe hain...", true);
                txtFullName.Text = "";
                txtEmail.Text    = "";
                Response.AddHeader("Refresh", "2;url=Login.aspx");
                break;
            case 1:
                ShowMsg("Yeh email already registered hai. Login karo ya alag email use karo.", false);
                break;
            default:
                ShowMsg("Server error. Thodi der baad try karo.", false);
                break;
        }
    }

    private void ShowMsg(string text, bool ok)
    {
        pnlMsg.Visible = true;
        divMsg.InnerHtml = ok ? "&#10003; " + text : "&#10007; " + text;
        divMsg.Attributes["class"] = ok ? "msg msg-ok" : "msg msg-err";
    }
}
