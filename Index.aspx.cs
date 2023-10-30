using AjaxControlToolkit.HtmlEditor.ToolbarButtons;
using System;
using System.Web;
using System.Web.UI.WebControls;

public partial class Index : System.Web.UI.Page
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

            if(admin != null) Response.Redirect("Admin/Inicio.aspx");
            else if (usuario == null || cliente == null)
            {
                return;
            }
            else
            {
                if (usuario.IDTIPOUSUARIOWEB == _cliente)
                    Response.Redirect("Code.aspx");
            }
        }
    }

    private string GenerateCode()
    {
        // Generate a random 6-digit code
        Random random = new Random();
        int code = random.Next(100000, 999999);
        return code.ToString();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void LogAcceso_Authenticate(object sender, AuthenticateEventArgs e)
    {
        div_error.Visible = false;
        Label2.Text = "";
        






        string userConGuion = ManejaUsuarios.LimpiarNIT(Login1.UserName.Trim());
        string userSinGuion = String.Empty;

        // Verificar si tiene dígito de verificación
        if (Login1.UserName.IndexOf('-') != -1)
            userSinGuion = ManejaUsuarios.LimpiarNIT(Login1.UserName.Remove(Login1.UserName.IndexOf('-')));
        else
            userSinGuion = ManejaUsuarios.LimpiarNIT(Login1.UserName.Trim());
        //

        string nitEC = Login1.UserName.Trim();
        

        ManejaUsuarios manejaUsuarios = new ManejaUsuarios();

        forward_clientes clienteEC = manejaUsuarios.BuscarClienteBD_ECCargo(nitEC);

        TERCEROS clienteConGuion = manejaUsuarios.BuscarClienteBDHosting(userConGuion);
        TERCEROS clienteSinGuion = manejaUsuarios.BuscarClienteBDHosting(userSinGuion);
        fed_usuarios admin = manejaUsuarios.BuscarAdmin(Login1.UserName.Trim());

        if (admin != null)
        {
            var admin_password = manejaUsuarios.BuscarAdminPassword(Login1.UserName.Trim());

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

                    string codigoVerificacion = GenerateCode();


                    USUARIOSWEB usuario_password = manejaUsuarios.BuscarUsuarioYPasswordBDHosting(userConGuion ?? String.Empty);

                    if (usuario_password == null) usuario_password = manejaUsuarios.BuscarUsuarioYPasswordBDHosting(userSinGuion ?? String.Empty);

                    if (usuario_password != null && usuario_password.NUMEROINTENTOS < ManejaUsuarios.numero_intentos)
                    {
                        Session["admin"] = null;
                        Session["usuario"] = usuario_password;
                        Session["cliente"] = usuario_password.TERCEROS;
                        manejaUsuarios.MandarCorreoCode(usuario, codigoVerificacion);
                        Session["CodigoVerificacion"] = codigoVerificacion;
                        Session["CodigoVerificacionTiempo"] = DateTime.Now;
                        manejaUsuarios.ReiniciarIntentos(usuario_password);
                        manejaUsuarios.DesconectarBD();
                        e.Authenticated = true;
                    }
                    else if (usuario.NUMEROINTENTOS >= ManejaUsuarios.numero_intentos)
                    {
                        manejaUsuarios.ActualizarIntentosFallidos(usuario);
                        manejaUsuarios.MandarCorreoCode(usuario, codigoVerificacion);
                        Session["CodigoVerificacion"] = codigoVerificacion;
                        Session["CodigoVerificacionTiempo"] = DateTime.Now;
                        manejaUsuarios.DesconectarBD();
                        //div_error.Visible = true;
                        Label2.Text = codigoVerificacion;
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
            Label2.Text = "El cliente ingresado no existe en nuestra base de datos.";
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
        Panel1.Visible = false;
        div_error.Visible = false;
        Response.Redirect("RecuperarContrasena.aspx");
    }
}