using AjaxControlToolkit.HtmlEditor.ToolbarButtons;
using System;
using System.Web;
using System.Web.UI.WebControls;

public partial class Code : System.Web.UI.Page
{
    private static int _admin = 2;
    private static int _cliente = 1;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (HttpContext.Current.User.Identity.IsAuthenticated)
        {
            USUARIOSWEB usuario = (USUARIOSWEB)Session["usuario"];
            TERCEROS cliente = (TERCEROS)Session["cliente"];
            fed_usuarios admin = (fed_usuarios)Session["admin"];

            if (admin != null) Response.Redirect("Admin/Inicio.aspx");
            else if (usuario == null || cliente == null)
            {
                return;
            }
            //else
            //{
            //    if (usuario.IDTIPOUSUARIOWEB == _cliente)
            //        Response.Redirect("Interno/Inicio.aspx");
            //}
        }
    }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void LogAcceso_Authenticate2(object sender, AuthenticateEventArgs e)
    {
        div_error.Visible = false;
        Label2.Text = "";
        string codeSave = Session["CodigoVerificacion"] as string;
        string userConGuion = ManejaUsuarios.LimpiarNIT(Login2.UserName.Trim());
        string userSinGuion = String.Empty;

        // Verificar si tiene dígito de verificación
        if (Login2.UserName.IndexOf('-') != -1)
            userSinGuion = ManejaUsuarios.LimpiarNIT(Login2.UserName.Remove(Login2.UserName.IndexOf('-')));
        else
            userSinGuion = ManejaUsuarios.LimpiarNIT(Login2.UserName.Trim());
        //

        string nitEC = Login2.UserName.Trim();
        string code = Login2.Password.Trim();

        ManejaUsuarios manejaUsuarios = new ManejaUsuarios();

        forward_clientes clienteEC = manejaUsuarios.BuscarClienteBD_ECCargo(nitEC);

        TERCEROS clienteConGuion = manejaUsuarios.BuscarClienteBDHosting(userConGuion);
        TERCEROS clienteSinGuion = manejaUsuarios.BuscarClienteBDHosting(userSinGuion);
        fed_usuarios admin = manejaUsuarios.BuscarAdmin(Login2.UserName.Trim());

        if (admin != null)
        {
            var admin_password = manejaUsuarios.BuscarAdminPassword(Login2.UserName.Trim());

            if (admin_password != null && admin_password.fed_usuario_intentos < ManejaUsuarios.numero_intentos)
            {
                Session["admin"] = admin_password;
                Session["usuario"] = null;
                Session["cliente"] = null;
                manejaUsuarios.ReiniciarIntentosAdmin(admin_password);
                manejaUsuarios.DesconectarBD();
                e.Authenticated = true;
            }
            else if (admin.fed_usuario_intentos >= ManejaUsuarios.numero_intentos)
            {
                manejaUsuarios.DesconectarBD();
                div_error.Visible = true;
                Label2.Text = "Su usuario ha sido bloqueado al exceder el límite de inicios de sesión fallidos.";
            }
            else
            {
                manejaUsuarios.ActualizarIntentosFallidosAdmin(admin);
                manejaUsuarios.DesconectarBD();
                div_error.Visible = true;
                Label2.Text = "Los datos ingresados son incorrectos.";
            }
        }
        else if (clienteEC != null)
        {
            if (clienteConGuion != null || clienteSinGuion != null)
            {
                USUARIOSWEB usuario = manejaUsuarios.BuscarUsuarioBDHosting(userConGuion ?? String.Empty);

                if(usuario == null) usuario = manejaUsuarios.BuscarUsuarioBDHosting(userSinGuion ?? String.Empty);

                if (usuario != null)
                {
                    

                    USUARIOSWEB usuario_password = manejaUsuarios.BuscarUsuarioYPasswordBDHosting(userConGuion ?? String.Empty);

                    if (usuario_password == null) usuario_password = manejaUsuarios.BuscarUsuarioYPasswordBDHosting(userSinGuion ?? String.Empty);

                    if (code == codeSave)
                    {
                        Session["admin"] = null;
                        Session["usuario"] = usuario_password;
                        Session["cliente"] = usuario_password.TERCEROS;
                        manejaUsuarios.ReiniciarIntentos(usuario_password);
                        manejaUsuarios.DesconectarBD();
                        e.Authenticated = true;
                        Response.Redirect("Interno/Inicio.aspx");

                    }
                    else if (usuario.NUMEROINTENTOS >= ManejaUsuarios.numero_intentos)
                    {
                        
                        manejaUsuarios.DesconectarBD();
                        div_error.Visible = true;
                        Label2.Text = "Su usuario ha sido bloqueado al exceder el límite de inicios de sesión fallidos.";
                    }
                    else
                    {
                        manejaUsuarios.ActualizarIntentosFallidos(usuario);
                        manejaUsuarios.DesconectarBD();
                        div_error.Visible = true;
                        Label2.Text = "Los datos ingresados son incorrectos.";
                    }
                }
                else
                {
                    Session["cliente"] = clienteConGuion ?? clienteSinGuion;
                    Session["registrar"] = "S";
                    Session["clicodigo"] = clienteEC.cli_codigo.ToString();
                    manejaUsuarios.DesconectarBD();
                    Response.Redirect("RegistroUsuarios.aspx");
                }
            }
            else
            {
                clienteConGuion = manejaUsuarios.IngresarClienteNoExistente(clienteEC);

                if (clienteConGuion != null)
                {
                    Session["cliente"] = clienteConGuion;
                    Session["registrar"] = "S";
                    Session["clicodigo"] = clienteEC.cli_codigo.ToString();
                    manejaUsuarios.DesconectarBD();
                    Response.Redirect("RegistroUsuarios.aspx");
                }
            }
        }
        else
        {
            //El cliente no existe en ningún lado.
            manejaUsuarios.DesconectarBD();
            div_error.Visible = true;
            Label2.Text = Session["CodigoVerificacion"] as string;
        }
    }    

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void LinkButton3_Click(object sender, EventArgs e)
    {
        //Elementos del panel 1
        Panel2.Visible = false;
        div_error.Visible = false;
        Response.Redirect("RecuperarContrasena.aspx");
    }
}