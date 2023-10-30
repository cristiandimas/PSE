using System;
using System.Web;

public partial class RegistraPagos : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            /// Numumero factura
            int codigoReferencia = Convert.ToInt32(HttpUtility.UrlDecode(HttpContext.Current.Request.Form["codigoFactura"]));
            string valor = HttpUtility.UrlDecode(HttpContext.Current.Request.Form["valorFactura"]).Replace('.', ',');
            decimal valorReferencia = Decimal.Round(Convert.ToDecimal(valor), 2);
            int transaccionAprobada = Convert.ToInt32(HttpUtility.UrlDecode(HttpContext.Current.Request.Form["transaccionAprobada"]));
            //string codigoAutorizacion = HttpUtility.UrlDecode(HttpContext.Current.Request.Form["codigoAutorizacion"]);
            //string firmaTuCompra = HttpUtility.UrlDecode(HttpContext.Current.Request.Form["firmaTuCompra"]);
            string numeroTransaccion = HttpUtility.UrlDecode(HttpContext.Current.Request.Form["numeroTransaccion"]);
            string metodoPago = HttpUtility.UrlDecode(HttpContext.Current.Request.Form["metodoPago"]);

            if (transaccionAprobada == 1)
            {
                ManejaPagos mp = new ManejaPagos();
                if (mp.RegistrarPagos(codigoReferencia, valorReferencia, numeroTransaccion, metodoPago))
                {
                    mp.MandarCorreoAdministrativo(codigoReferencia);
                    mp.DesconectarBD();
                    HttpContext.Current.Response.StatusCode = 200;
                }
                else
                {
                    mp.DesconectarBD();
                    HttpContext.Current.Response.StatusCode = 500;
                }
            }
            else
            {
                HttpContext.Current.Response.StatusCode = 200;
            }
        }
        catch
        {
            HttpContext.Current.Response.StatusCode = 500;
        }
    }
}