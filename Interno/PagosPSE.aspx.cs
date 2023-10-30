using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web;
using System.IO;
using System.Net;

public partial class Interno_PagosPSE : System.Web.UI.Page
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
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

            HiddenFieldCliente.Value = usuario.CODIGO_ECCARGO;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Button1_Click(object sender, EventArgs e)
    {
        decimal valor_a_pagar = Convert.ToDecimal(HiddenFieldValorAPagar.Value);
        String[] lista_facturas_st = HiddenFieldListaFacturas.Value.Split(';');
        String[] lista_valores_st = HiddenFieldListaValores.Value.Split(';');
        String[] lista_divisa_l_st = HiddenFieldDivisaLocal.Value.Split(';');
        String[] lista_divisa_o_st = HiddenFieldDivisaOtra.Value.Split(';');
        String[] lista_valores_ica = HiddenFieldReteICA.Value.Split(';');
        String[] lista_valores_ret_iva = HiddenFieldReteIVA.Value.Split(';');
        String[] lista_sucursales = HiddenFieldSucursalFactura.Value.Split(';');

        List<decimal> lista_facturas = new List<decimal>();
        List<decimal> lista_valores = new List<decimal>();
        List<decimal> lista_divisa_l = new List<decimal>();
        List<decimal> lista_divisa_o = new List<decimal>();
        List<decimal> lista_valores_ic = new List<decimal>();
        List<decimal> lista_valores_rti = new List<decimal>();
        List<int> lista_sucoper = new List<int>();

        for (int i = 0; i < lista_divisa_l_st.Length - 1; i++)
        {
            lista_facturas.Add(Convert.ToDecimal(lista_facturas_st[i]));
            lista_valores.Add(Convert.ToDecimal(lista_valores_st[i]));
            lista_divisa_l.Add(Convert.ToDecimal(lista_divisa_l_st[i]));
            lista_divisa_o.Add(Convert.ToDecimal(lista_divisa_o_st[i]));
            lista_valores_ic.Add(Convert.ToDecimal(lista_valores_ica[i]));
            lista_valores_rti.Add(Convert.ToDecimal(lista_valores_ret_iva[i]));
            lista_sucoper.Add(Convert.ToInt32(lista_sucursales[i]));
        }

        ManejaPagos manejaPagos = new ManejaPagos();
        int referencia = manejaPagos.GuardarRegistros(lista_facturas, lista_valores, lista_divisa_l, lista_divisa_o, lista_valores_ic, lista_valores_rti, ((USUARIOSWEB)Session["usuario"]).IDUSUARIOWEB, valor_a_pagar, lista_sucoper);
        manejaPagos.DesconectarBD();

        if (referencia > 0)
        {
            string hora = DateTime.Now.Hour + ":" + DateTime.Now.Minute;

            USUARIOSWEB user = (USUARIOSWEB)Session["usuario"];
            TERCEROS tercero = (TERCEROS)Session["cliente"];

            NameValueCollection data = new NameValueCollection
            {
                { "usuario", "txd0s8g72zc2581k" },
                { "factura", referencia.ToString() },
                { "valor", HiddenFieldValorAPagar.Value },
                { "descripcionFactura", HiddenFieldListaFacturas.Value },
                { "tokenSeguridad", CalculateMD5Hash("02ca9baf4b604ba0b18ddcf49983cd55" + hora) },
                { "documentoComprador",tercero.DOCUMENTO },
                { "tipoDocumento", "NIT"},
                { "nombreComprador", tercero.RAZONSOCIAL },
                { "apellidoComprador", "" },
                { "correoComprador", user.CORREO }
            };            
            RedirectAndPOST(this.Page, "https://gateway2.tucompra.com.co/tc/app/inputs/compra.jsp", data);
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public string CalculateMD5Hash(string input)
    {
        // step 1, calculate MD5 hash from input
        MD5 md5 = MD5.Create();
        byte[] inputBytes = Encoding.ASCII.GetBytes(input);
        byte[] hash = md5.ComputeHash(inputBytes);

        // step 2, convert byte array to hex string
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < hash.Length; i++)
        {
            sb.Append(hash[i].ToString("X2"));
        }

        return sb.ToString();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Button2_Click(object sender, EventArgs e)
    {
        Panel1.Visible = false;
        Panel2.Visible = true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void BtnActualizarValores_Click(object sender, EventArgs e)
    {
        ManejaPagos manejaPagos = new ManejaPagos();
        List<decimal> lista_valores = manejaPagos.CalcularSaldosCliente(HiddenFieldCliente.Value);
        manejaPagos.DesconectarBD();

        if (lista_valores != null)
        {
            Label5.Text = lista_valores[ManejaPagos.posicion_trm].ToString("C2");
            Label6.Text = lista_valores[ManejaPagos.posicion_corrientes].ToString("C2");
            Label7.Text = lista_valores[ManejaPagos.posicion_vencidas].ToString("C2");
            Label8.Text = lista_valores[ManejaPagos.posicion_total].ToString("C2");
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
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void CheckBox2_CheckedChanged(object sender, EventArgs e)
    {
        CheckBox ch = (CheckBox)sender;

        List<Tuple<int, int>> list = new List<Tuple<int, int>>();
        if (ViewState["SelectedRecords"] != null)
        {
            list = (List<Tuple<int, int>>)ViewState["SelectedRecords"];
        }

        if (ch.Checked)
        {
            foreach (GridViewRow fila in GridView1.Rows)
            {
                ch = (CheckBox)fila.Cells[0].Controls[1];
                ch.Checked = true;

                var selectedFactKey = int.Parse(GridView1.DataKeys[fila.RowIndex].Values["fac_numero"].ToString());
                var selectedSucuKey = int.Parse(GridView1.DataKeys[fila.RowIndex].Values["fac_sucursal"].ToString());

                Tuple<int, int> tuple = new Tuple<int, int>(selectedFactKey, selectedSucuKey);
                if (!list.Contains(tuple))
                {
                    list.Add(tuple);
                }
            }

            ViewState["SelectedRecords"] = list;
            RecorrerGridView();
            Panel1.Visible = true;
        }
        else
        {
            foreach (GridViewRow fila in GridView1.Rows)
            {
                ch = (CheckBox)fila.Cells[0].Controls[1];
                ch.Checked = false;

                var selectedFactKey = int.Parse(GridView1.DataKeys[fila.RowIndex].Values["fac_numero"].ToString());
                var selectedSucuKey = int.Parse(GridView1.DataKeys[fila.RowIndex].Values["fac_sucursal"].ToString());

                Tuple<int, int> tuple = new Tuple<int, int>(selectedFactKey, selectedSucuKey);
                if (list.Contains(tuple))
                {
                    list.Remove(tuple);
                }
            }

            ViewState["SelectedRecords"] = list;
            Panel1.Visible = false;
        }

        GridView1.Focus();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void CheckBox3_CheckedChanged(object sender, EventArgs e)
    {
        CheckBox ch = (CheckBox)sender;
        GridViewRow row;
        bool aux = false;

        List<Tuple<int, int>> list = new List<Tuple<int, int>>();
        if (ViewState["SelectedRecords"] != null)
        {
            list = (List<Tuple<int, int>>)ViewState["SelectedRecords"];
        }

        for (int i = 0; i <= GridView1.Rows.Count - 1; i++)
        {
            row = GridView1.Rows[i];
            ch = (CheckBox)row.Cells[0].Controls[1];

            var selectedFactKey = int.Parse(GridView1.DataKeys[row.RowIndex].Values["fac_numero"].ToString());
            var selectedSucuKey = int.Parse(GridView1.DataKeys[row.RowIndex].Values["fac_sucursal"].ToString());

            Tuple<int, int> tuple = new Tuple<int, int>(selectedFactKey, selectedSucuKey);
            if (ch.Checked)
            {
                if (!list.Contains(tuple))
                {
                    list.Add(tuple);
                }
            }
            else
            {
                if (list.Contains(tuple))
                {
                    list.Remove(tuple);
                }
            }

            aux = aux || ch.Checked;
        }

        ViewState["SelectedRecords"] = list;

        if (aux || list != null)
        {
            RecorrerGridView();
            Panel1.Visible = true;
        }
        else
        {
            Panel1.Visible = false;
        }

        GridView1.Focus();
    }

    /// <summary>
    /// 
    /// </summary>
    public void RecorrerGridView()
    {
        Label2.Text = String.Empty;
        Label4.Text = String.Empty;
        Label10.Text = String.Empty;
        HiddenFieldValorAPagar.Value = String.Empty;
        HiddenFieldListaFacturas.Value = String.Empty;
        HiddenFieldListaValores.Value = String.Empty;
        HiddenFieldDivisaLocal.Value = String.Empty;
        HiddenFieldDivisaOtra.Value = String.Empty;
        HiddenFieldReteICA.Value = String.Empty;
        HiddenFieldReteIVA.Value = String.Empty;
        HiddenFieldSucursalFactura.Value = String.Empty;

        List<Tuple<int, int>> lista_facturas = ViewState["SelectedRecords"] as List<Tuple<int, int>>;

        ManejaPagos manejaPagos = new ManejaPagos();
        decimal valorAPagar = manejaPagos.CalcularValorTotalAPagar(lista_facturas, HiddenFieldCliente.Value.ToString());
        decimal valorICA = 0;
        decimal valorRetIVA = 0;

        if (lista_facturas != null)
        {
            for (int i = 0; i < lista_facturas.Count; i++)
            {
                HiddenFieldListaFacturas.Value += lista_facturas[i].Item1 + ";";
                HiddenFieldListaValores.Value += manejaPagos.SaldoFacturaCOP(lista_facturas[i].Item1, lista_facturas[i].Item2, HiddenFieldCliente.Value.ToString()) + ";";
                HiddenFieldDivisaLocal.Value += "1;";
                HiddenFieldDivisaOtra.Value += manejaPagos.DivisaUSD(lista_facturas[i].Item1, lista_facturas[i].Item2, HiddenFieldCliente.Value.ToString()) + ";";
                HiddenFieldSucursalFactura.Value += lista_facturas[i].Item2 + ";";

                Label2.Text += lista_facturas[i].Item1.ToString();
                if (i < (lista_facturas.Count - 1))
                {
                    Label2.Text += ", ";
                }

                valorICA = 0;
                HiddenFieldReteICA.Value += "0;";

                valorRetIVA = 0;
                HiddenFieldReteIVA.Value += "0;";
            }
        }

       manejaPagos.DesconectarBD();

        HiddenFieldValorAPagar.Value = (valorAPagar - valorICA - valorRetIVA).ToString();
       Label4.Text = valorAPagar.ToString("C2");
        Label10.Text = (valorAPagar - valorICA - valorRetIVA).ToString("C2");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void GridView1_DataBound(object sender, EventArgs e)
    {
        var dbContext = new EC_GROUPEntities();

        decimal valor_otra_divisa = 0;
        decimal valor_divisa_cop = 0;

        decimal valor_saldo_otra_divisa = 0;
        decimal valor_saldo_cop = 0;

        string codigo_ec = HiddenFieldCliente.Value.ToString();

        //Este for "Checkea" todos los chechbox del GridView1.
        for (int i = 0; i < GridView1.Rows.Count; i++)
        {
            int numFactura = Convert.ToInt32(GridView1.Rows[i].Cells[3].Text);            

            var list_facturas = dbContext.v_forward_facturas.Where(facturaT => facturaT.fac_numero == numFactura);
            var list_cte = dbContext.forward_ctacte.Where(ctacteT => ctacteT.cta_numero == numFactura && ctacteT.cta_debehaber == "D" && ctacteT.cta_comprobante == "FC" && ctacteT.cta_cbteaplica == "FC" && ctacteT.cta_cliente == codigo_ec);
            v_forward_facturas factura = new v_forward_facturas();
            forward_ctacte ctacte = new forward_ctacte();

            if (list_facturas.AsEnumerable().Count() > 0)
            {
                factura = list_facturas.AsEnumerable().First();
            }

            if (list_cte.AsEnumerable().Count() > 0)
            {
                ctacte = list_cte.AsEnumerable().First();
            }

            ManejaPagos manejaPagos = new ManejaPagos();
            decimal valor_otra_divisa_temp = 0;
            decimal valor_divisa_cop_temp = 0;
            decimal valor_saldo_otra_divisa_temp = 0;
            decimal valor_saldo_cop_temp = 0;

            if (factura.div_codigo.Equals(ManejaPagos.divisa_colombia))
            {
                valor_otra_divisa_temp = 0;
                valor_divisa_cop_temp = Convert.ToDecimal(factura.fac_total);

                valor_saldo_otra_divisa_temp = 0;
                valor_saldo_cop_temp = Convert.ToDecimal(ctacte.cta_saldo);
            }
            else
            {
                valor_otra_divisa_temp = Convert.ToDecimal(factura.fac_total);
                valor_divisa_cop_temp = manejaPagos.ValorFacturaCOP(factura);

                valor_saldo_otra_divisa_temp = Convert.ToDecimal(ctacte.cta_saldo); 
                valor_saldo_cop_temp = manejaPagos.SaldoFacturaCOP(ctacte);
            }

            ((Label)GridView1.Rows[i].FindControl("LabelGW2")).Text = valor_otra_divisa_temp.ToString("C2");
            ((Label)GridView1.Rows[i].FindControl("LabelGW4")).Text = valor_divisa_cop_temp.ToString("C2");
            ((Label)GridView1.Rows[i].FindControl("LabelGW6")).Text = valor_saldo_otra_divisa_temp.ToString("C2");
            ((Label)GridView1.Rows[i].FindControl("LabelGW8")).Text = valor_saldo_cop_temp.ToString("C2");
            valor_otra_divisa += valor_otra_divisa_temp;
            valor_divisa_cop += valor_divisa_cop_temp;
            valor_saldo_otra_divisa += valor_saldo_otra_divisa_temp;
            valor_saldo_cop += valor_saldo_cop_temp;

            manejaPagos.DesconectarBD();
        }

        if (GridView1.Rows.Count > 0)
        {
            GridView1.FooterRow.Cells[4].Text = "Total Facturas";
            GridView1.FooterRow.Cells[5].HorizontalAlign = HorizontalAlign.Right;

            GridView1.FooterRow.Cells[6].Text = valor_otra_divisa.ToString("C2");
            GridView1.FooterRow.Cells[7].Text = valor_divisa_cop.ToString("C2");

            GridView1.FooterRow.Cells[8].Text = valor_saldo_otra_divisa.ToString("C2");
            GridView1.FooterRow.Cells[9].Text = valor_saldo_cop.ToString("C2");
        }

        dbContext.Dispose();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void Page_Unload(object sender, System.EventArgs e)
    {

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        //Este if cambia el color de la fila
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int cant = Convert.ToInt32(e.Row.Cells[5].Text);

            if (cant > 0)
            {
                //#F2D1D1
                e.Row.BackColor = System.Drawing.Color.FromArgb(242, 209, 209);
            }
        }

        List<Tuple<int, int>> list = ViewState["SelectedRecords"] as List<Tuple<int, int>>;
        if (e.Row.RowType == DataControlRowType.DataRow && list != null)
        {
            var selectedFactKey = int.Parse(GridView1.DataKeys[e.Row.RowIndex].Values["fac_numero"].ToString());
            var selectedSucuKey = int.Parse(GridView1.DataKeys[e.Row.RowIndex].Values["fac_sucursal"].ToString());

            Tuple<int, int> tuple = new Tuple<int, int>(selectedFactKey, selectedSucuKey);
            if (list.Contains(tuple))
            {
                CheckBox chk = (CheckBox)e.Row.FindControl("CheckBox3");
                chk.Checked = true;
            }
        }
    }

    /// <summary> 
    /// Paginates the data. 
    /// </summary> 
    /// <param name="sender">The sender.</param> 
    /// <param name="e">The <seecref="System.Web.UI.WebControls.GridViewPageEventArgs"/>
    /// instance containing the event data.</param>
    protected void PaginateTheData(object sender, GridViewPageEventArgs e)
    {
        List<Tuple<int, int>> list = new List<Tuple<int, int>>();
        if (ViewState["SelectedRecords"] != null)
        {
            list = (List<Tuple<int, int>>)ViewState["SelectedRecords"];
        }

        foreach (GridViewRow row in GridView1.Rows)
        {
            CheckBox chk = (CheckBox)row.FindControl("CheckBox3");

            var selectedFactKey = int.Parse(GridView1.DataKeys[row.RowIndex].Values["fac_numero"].ToString());
            var selectedSucuKey = int.Parse(GridView1.DataKeys[row.RowIndex].Values["fac_sucursal"].ToString());

            Tuple<int, int> tuple = new Tuple<int, int>(selectedFactKey, selectedSucuKey);
            if (chk.Checked)
            {
                if (!list.Contains(tuple))
                {
                    list.Add(tuple);
                }
            }
            else
            {
                if (list.Contains(tuple))
                {
                    list.Remove(tuple);
                }
            }
        }
        ViewState["SelectedRecords"] = list;
        GridView1.PageIndex = e.NewPageIndex;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
    {
        string nombreArchivo = string.Empty;
        string nombreNuevaRes = string.Empty;
        
        ImageButton imageButton = (ImageButton)sender;
        int rowIndex = 0;

        if (GridView1.PageIndex >= 1)
        {
            int val = GridView1.PageIndex;
            int val2 = val * GridView1.PageSize;
            rowIndex = Convert.ToInt32(imageButton.ToolTip) - val2;
        }
        else
        {
            rowIndex = Convert.ToInt32(((ImageButton)sender).ToolTip);
        }       
		
		

        int factura = Convert.ToInt32(GridView1.DataKeys[rowIndex]["fac_numero"]);
        int sucursal = Convert.ToInt32(GridView1.DataKeys[rowIndex]["fac_sucursal"]);

        switch (sucursal)
        {
            case 1:
                nombreArchivo = "FV" + factura + ".pdf";
                nombreNuevaRes = "FV" + factura + ".pdf";
                break;
            case 2:
                nombreArchivo = "VR" + factura + ".pdf";
                nombreNuevaRes = "MDE" + factura + ".pdf";
                break;
            case 5:
                nombreArchivo = "BQ" + factura + ".pdf";
                nombreNuevaRes = "BAQ" + factura + ".pdf";
                break;
            case 9:
                nombreArchivo = "BGA" + factura + ".pdf";
                nombreNuevaRes = "BUC" + factura + ".pdf";
                break;
            case 10:
                nombreArchivo = "CLO" + factura + ".pdf";
                nombreNuevaRes = "CLI" + factura + ".pdf";
                break;
        }

        List<string> folders = Directory.EnumerateFiles(Server.MapPath("../Facturas"), "*.*", SearchOption.AllDirectories).ToList();
        var filteredFolderOldResolution = folders.Where(s => s.Contains(nombreArchivo)).FirstOrDefault();
        var filteredFolderNewResolution = folders.Where(s => s.Contains(nombreNuevaRes)).FirstOrDefault();

        if (filteredFolderNewResolution != null)
        {
            WebClient User = new WebClient();
            Byte[] FileBuffer = User.DownloadData(filteredFolderNewResolution);

            if (FileBuffer != null)
            {
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-length", FileBuffer.Length.ToString());
                Response.BinaryWrite(FileBuffer);
            }
        }
        else if (filteredFolderOldResolution != null)
        {
            WebClient User = new WebClient();
            Byte[] FileBuffer = User.DownloadData(filteredFolderOldResolution);

            if (FileBuffer != null)
            {
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-length", FileBuffer.Length.ToString());
                Response.BinaryWrite(FileBuffer);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void CheckBox5_CheckedChanged(object sender, EventArgs e)
    {
        RecorrerGridView();
    }
}