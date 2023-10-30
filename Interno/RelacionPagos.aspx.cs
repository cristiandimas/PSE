using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Interno_RelacionPagos : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack && !Page.IsCallback)
        {
            TERCEROS cliente = (TERCEROS)Session["cliente"];
            USUARIOSWEB usuario = (USUARIOSWEB)Session["usuario"];

            if (cliente == null || usuario == null)
            {
                Session.Abandon();
                Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));
                Response.Redirect("../Index.aspx");
            }

            HiddenFieldCodigoCliente.Value = usuario.CODIGO_ECCARGO;
        }
    }

    protected void LinkButton_Click(object sender, EventArgs e)
    {
        Panel1.Visible = true;
        LinkButton b = (LinkButton)sender;
        int dato = Convert.ToInt32(b.CommandArgument);
        Label1.Text = dato.ToString();

        ManejaPagos manejaPagos = new ManejaPagos();
        Label2.Text = manejaPagos.ListarFacturasReferencia(dato);
        manejaPagos.DesconectarBD();
    }
}