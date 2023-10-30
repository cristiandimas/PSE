using System;
using System.Web.UI;

public partial class Interno_MasterPageInterna : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            USUARIOSWEB usuario = (USUARIOSWEB)Session["usuario"];
            TERCEROS cliente = (TERCEROS)Session["cliente"];
            fed_usuarios user = (fed_usuarios)Session["admin"];
            if (user == null || usuario != null || cliente != null) Response.Redirect("../Index.aspx");
        }
    }
}
