using System;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;

public partial class Interno_PagoAnticipos : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack && !Page.IsCallback)
        {
            if (Session["usuario"] == null || Session["cliente"] == null)
            {
                Session.Abandon();
                Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));
                Response.Redirect("../Index.aspx");
            }

            HiddenFieldCliente.Value = ((USUARIOSWEB)Session["usuario"]).CODIGO_ECCARGO;

            TERCEROS cliente = (TERCEROS)Session["cliente"];
            if (cliente != null)
            {
                if (String.IsNullOrEmpty(cliente.TIPOCONTRIBUYENTE))
                {
                    Session.Abandon();
                    Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));
                    Response.Redirect("../Index.aspx");
                }
            }
            else
            {
                Session.Abandon();
                Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));
                Response.Redirect("../Index.aspx");
            }
        }
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        int valorPagar = 0;
        div_error.Visible = false;
        Label1.Text = "";

        if (TextBox1.Text.Equals("") || TextBox2.Text.Equals("") || TextBox3.Text.Equals(String.Empty))
        {
            Label1.Text = "Los campos no pueden estar vacíos.";
            div_error.Visible = true;
        }
        else if (!int.TryParse(TextBox2.Text, out valorPagar))
        {
            Label1.Text = "El valor a pagar no es un número válido.";
            div_error.Visible = true;
        }
        else
        {
            string hora = DateTime.Now.Hour + ":" + DateTime.Now.Minute;

            USUARIOSWEB user = ((USUARIOSWEB)Session["usuario"]);

            ManejaPagos manejaPagos = new ManejaPagos();
            int referencia = manejaPagos.GuardarRegistroAnticipos(user.IDUSUARIOWEB, valorPagar, TextBox1.Text, TextBox3.Text);
            manejaPagos.DesconectarBD();

            NameValueCollection data = new NameValueCollection
            {
                { "usuario", "txd0s8g72zc2581k" },
                { "factura", referencia.ToString() },
                { "valor", valorPagar.ToString() },
                { "descripcionFactura", TextBox1.Text },
                { "tokenSeguridad", CalculateMD5Hash("02ca9baf4b604ba0b18ddcf49983cd55" + hora) },
                { "documentoComprador", user.TERCEROS.DOCUMENTO },
                { "tipoDocumento", "NIT"},
                { "nombreComprador", user.TERCEROS.RAZONSOCIAL },
                { "apellidoComprador", "" },
                { "correoComprador", user.CORREO }
            };
            RedirectAndPOST(this.Page, "https://gateway2.tucompra.com.co/tc/app/inputs/compra.jsp", data);
        }
    }

    /// <summary>
    /// POST data and Redirect to the specified url using the specified page.
    /// </summary>
    /// <param name="page">The page which will be the referrer page.</param>
    /// <param name="destinationUrl">The destination Url to which
    /// the post and redirection is occuring.</param>
    /// <param name="data">The data should be posted.</param>
    public static void RedirectAndPOST(Page page, string destinationUrl, NameValueCollection data)
    {
        //Prepare the Posting form
        string strForm = PreparePOSTForm(destinationUrl, data);
        //Add a literal control the specified page holding 
        //the Post Form, this is to submit the Posting form with the request.
        page.Controls.Add(new LiteralControl(strForm));
    }

    /// <summary>
    /// This method prepares an Html form which holds all data
    /// in hidden field in the addetion to form submitting script.
    /// </summary>
    /// <param name="url">The destination Url to which the post and redirection
    /// will occur, the Url can be in the same App or ouside the App.</param>
    /// <param name="data">A collection of data that
    /// will be posted to the destination Url.</param>
    /// <returns>Returns a string representation of the Posting form.</returns>
    private static String PreparePOSTForm(string url, NameValueCollection data)
    {
        //Set a name for the form
        string formID = "PostForm";
        //Build the form using the specified data to be posted.
        StringBuilder strForm = new StringBuilder();
        strForm.Append("<form id=\"" + formID + "\" name=\"" +
                       formID + "\" action=\"" + url +
                       "\" method=\"POST\">");

        foreach (string key in data)
        {
            strForm.Append("<input type=\"hidden\" name=\"" + key +
                           "\" value=\"" + data[key] + "\">");
        }

        strForm.Append("</form>");
        //Build the JavaScript which will do the Posting operation.
        StringBuilder strScript = new StringBuilder();
        strScript.Append("<script language=\"javascript\">");
        strScript.Append("var v" + formID + " = document." +
                         formID + ";");
        strScript.Append("v" + formID + ".submit();");
        strScript.Append("$('#overlay').modal('show');");
        strScript.Append("</script>");
        //Return the form and the script concatenated.
        //(The order is important, Form then JavaScript)
        return strForm.ToString() + strScript.ToString();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public string CalculateMD5Hash(string input)

    {
        // step 1, calculate MD5 hash from input
        MD5 md5 = System.Security.Cryptography.MD5.Create();
        byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
        byte[] hash = md5.ComputeHash(inputBytes);

        // step 2, convert byte array to hex string
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < hash.Length; i++)
        {
            sb.Append(hash[i].ToString("X2"));
        }

        return sb.ToString();
    }
}