using System;
using System.Web.UI;

public partial class Site : MasterPage
{
    protected void Page_Load(object sender, EventArgs e) { }

    protected void btnLogout_Click(object sender, EventArgs e)
    {
        Session.Clear();
        Session.Abandon();
        Response.Redirect("~/Default.aspx");
    }
}
