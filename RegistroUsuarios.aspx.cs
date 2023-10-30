using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class RegistroUsuarios : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if(!IsPostBack)
        {
            string registrar = (string)Session["registrar"];
            TERCEROS cliente = (TERCEROS)Session["cliente"];
            string clicodigo = (string)Session["clicodigo"];

            if (cliente == null || registrar == "" || clicodigo == "")
                Response.Redirect("Index.aspx");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Button1_Click1(object sender, EventArgs e)
    {
        Label1.Text = "";
        div_actualizar.Visible = false;
        div_success.Visible = false;

        if ( TextBox3.Text.Trim().Equals(""))
        {
            Label1.Text = "Los campos no pueden estar vacíos.";
            div_actualizar.Visible = true;
        }       
        else
        {
            ManejaUsuarios manejaUsuarios = new ManejaUsuarios();
            if (manejaUsuarios.ExisteCorreo(TextBox3.Text.Trim()))
            {
                Label1.Text = "Esta dirección de correo electrónico ya se encuentra registrada en nuestro sistema.";
                manejaUsuarios.DesconectarBD();
                div_actualizar.Visible = true;

            }
            else if (manejaUsuarios.IngresarUsuarioWeb(((TERCEROS)Session["cliente"]).IDTERCERO, Session["clicodigo"].ToString(), TextBox3.Text.Trim()))
            {
                manejaUsuarios.DesconectarBD();
                div_success.Visible = true;
            }
            else
            {
                Label1.Text = "Intente nuevamente.";
                manejaUsuarios.DesconectarBD();
                div_actualizar.Visible = true;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void LinkButton1_Click(object sender, EventArgs e)
    {
        Session["registrar"] = null;

        //Elementos del panel 2
        div_actualizar.Visible = false;
        div_success.Visible = false;
        
        LinkButton1.Visible = false;
        Label1.Text = "";
       

        Response.Redirect("Index.aspx");
    }
}