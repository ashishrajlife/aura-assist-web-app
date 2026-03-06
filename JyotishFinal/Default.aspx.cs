using System;
using System.Data;
using System.Web.UI;

public partial class Default : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            DataTable dt = DatabaseHelper.GetAllRashis();
            rptRashis.DataSource = dt;
            rptRashis.DataBind();
        }
    }
   

}
