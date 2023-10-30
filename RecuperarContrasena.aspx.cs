using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class RecuperarContrasena : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Button2_Click(object sender, EventArgs e)
    {
        div_recuperar.Visible = false;
        div_recuperar_success.Visible = false;

        ManejaUsuarios manejaUsuarios = new ManejaUsuarios();

        if (TextBox4.Text.Trim().Equals(""))
        {
            Label3.Text = "La dirección de correo electrónico no puede estar vacía.";
            div_recuperar.Visible = true;
        }
        else
        {
            int codigo = manejaUsuarios.ActualizarPassword(TextBox4.Text);
            manejaUsuarios.DesconectarBD();

            switch (codigo)
            {
                case 1:
                    {
                        Label1.Text = TextBox4.Text;
                        TextBox4.Text = String.Empty;
                        div_recuperar_success.Visible = true;
                        break;
                    }
                case 2:
                    {
                        Label3.Text = "Intente nuevamente.";
                        div_recuperar.Visible = true;
                        break;
                    }
                case 3:
                    {
                        Label3.Text = "La dirección de correo electrónico no existe en nuestra base de datos.";
                        div_recuperar.Visible = true;
                        break;
                    }
                default:
                    {
                        Label3.Text = "Intente nuevamente.";
                        div_recuperar.Visible = true;
                        break;
                    }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void LinkButton2_Click(object sender, EventArgs e)
    {
        div_recuperar.Visible = false;
        div_recuperar_success.Visible = false;
        Response.Redirect("Index.aspx");
    }
}