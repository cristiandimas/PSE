using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Interno_MasterPageInterna : System.Web.UI.MasterPage
{
    private static int _admin = 2;
    private static int _cliente = 1;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            USUARIOSWEB user = (USUARIOSWEB)Session["usuario"];
            fed_usuarios fed_user = (fed_usuarios)Session["admin"];

            if (user == null || fed_user != null)
            {
                Response.Redirect("../Index.aspx");
            }

            if (user.IDTIPOUSUARIOWEB != _cliente)
            {
                Response.Redirect("../Index.aspx");
            }
        }
    }
}
