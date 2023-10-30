using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Interno_CuentaUsuario : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

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

        if (TextBox1.Text.Trim().Equals("") || TextBox2.Text.Trim().Equals("") || TextBox3.Text.Trim().Equals(""))
        {
            Label1.Text = "Los campos no pueden estar vacíos.";
            div_actualizar.Visible = true;
        }
        else if (!TextBox1.Text.Equals(TextBox2.Text))
        {
            Label1.Text = "Las contraseñas no pueden ser diferentes.";
            div_actualizar.Visible = true;
        }
        else if (!((USUARIOSWEB)Session["usuario"]).PASSWORD.Equals(ManejaUsuarios.CalculateMD5Hash(TextBox3.Text)))
        {
            Label1.Text = @"La contraseña actual no es correcta.";
            div_actualizar.Visible = true;
        }
        else if (!ManejaUsuarios.IsValidPassword(TextBox1.Text))
        {
            Label1.Text = @"La contraseña debe tener un tamaño entre 8 y 12 caracteres, además, debe contener al menos una letra en mayúscula, una en minúscula, un número y un caractér especial.";
            div_actualizar.Visible = true;
        }
        //else
        //{
        //    ManejaUsuarios manejaUsuarios = new ManejaUsuarios();
        //    //int resultado = manejaUsuarios.ActualizarPassword(TextBox1.Text, ((USUARIOSWEB)Session["usuario"]).CORREO);

        //    if (resultado == ManejaUsuarios.correo_password_send_success)
        //    {
        //        div_success.Visible = true;
        //        manejaUsuarios.DesconectarBD();
        //    }
        //    else
        //    {
        //        Label1.Text = "";
        //        div_actualizar.Visible = true;
        //        manejaUsuarios.DesconectarBD();
        //    }
        //}
    }
}