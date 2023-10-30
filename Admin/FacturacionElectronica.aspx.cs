using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Admin_FacturacionElectronica : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ImageButtonValidacion_Click(object sender, ImageClickEventArgs e)
    {
        string nombreArchivo = "";

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

        int factura = Convert.ToInt32(GridView1.DataKeys[rowIndex]["fed_num_factura"]);
        int sucursal = Convert.ToInt32(GridView1.DataKeys[rowIndex]["fed_sucursal_id"]);

        switch (sucursal)
        {
            case 1:
                nombreArchivo = "dianresponse_BOG" + factura + ".xml";
                break;
            case 2:
                nombreArchivo = "dianresponse_MDE" + factura + ".xml";
                break;
            case 5:
                nombreArchivo = "dianresponse_BAQ" + factura + ".xml";
                break;
            case 9:
                nombreArchivo = "dianresponse_BUC" + factura + ".xml";
                break;
            case 10:
                nombreArchivo = "dianresponse_CLI" + factura + ".xml";
                break;
        }

        List<string> folders = Directory.GetFiles(Server.MapPath("../DocumentosGenerados"), nombreArchivo, SearchOption.AllDirectories).ToList();

        if (folders.Count > 0)
        {
            WebClient User = new WebClient();
            Byte[] FileBuffer = User.DownloadData(folders[0]);

            if (FileBuffer != null)
            {
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.ContentType = System.Net.Mime.MediaTypeNames.Text.Xml;
                HttpContext.Current.Response.AddHeader("Content-Disposition:", "attachment;filename=" + HttpUtility.UrlEncode(nombreArchivo));
                HttpContext.Current.Response.BinaryWrite(FileBuffer);
                HttpContext.Current.Response.End();
            }
        }
    }

    protected void ImageButtonValidacionXML_Click(object sender, ImageClickEventArgs e)
    {
        string nombreArchivo = "";

        ImageButton imageButton = (ImageButton)sender;
        int rowIndex = 0;

        if (GridView2.PageIndex >= 1)
        {
            int val = GridView2.PageIndex;
            int val2 = val * GridView2.PageSize;
            rowIndex = Convert.ToInt32(imageButton.ToolTip) - val2;
        }
        else
        {
            rowIndex = Convert.ToInt32(((ImageButton)sender).ToolTip);
        }

        int factura = Convert.ToInt32(GridView2.DataKeys[rowIndex]["num_factura"]);
        int sucursal = Convert.ToInt32(GridView2.DataKeys[rowIndex]["sucursal"]);

        switch (sucursal)
        {
            case 1:
                nombreArchivo = "BOG" + factura + ".xml";
                break;
            case 2:
                nombreArchivo = "MDE" + factura + ".xml";
                break;
            case 5:
                nombreArchivo = "BAQ" + factura + ".xml";
                break;
            case 9:
                nombreArchivo = "BUC" + factura + ".xml";
                break;
            case 10:
                nombreArchivo = "CLI" + factura + ".xml";
                break;
        }

        List<string> folders = Directory.GetFiles(Server.MapPath("../DocumentosGenerados"), nombreArchivo, SearchOption.AllDirectories).ToList();

        if (folders.Count > 0)
        {
            WebClient User = new WebClient();
            Byte[] FileBuffer = User.DownloadData(folders[0]);

            if (FileBuffer != null)
            {
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.ContentType = System.Net.Mime.MediaTypeNames.Text.Xml;
                HttpContext.Current.Response.AddHeader("Content-Disposition:", "attachment;filename=" + HttpUtility.UrlEncode(nombreArchivo));
                HttpContext.Current.Response.BinaryWrite(FileBuffer);
                HttpContext.Current.Response.End();
            }
        }
    }

    protected void ImageButtonValidacionPDF_Click(object sender, ImageClickEventArgs e)
    {
        string nombreArchivo = "";

        ImageButton imageButton = (ImageButton)sender;
        int rowIndex = 0;

        if (GridView2.PageIndex >= 1)
        {
            int val = GridView2.PageIndex;
            int val2 = val * GridView2.PageSize;
            rowIndex = Convert.ToInt32(imageButton.ToolTip) - val2;
        }
        else
        {
            rowIndex = Convert.ToInt32(((ImageButton)sender).ToolTip);
        }

        int factura = Convert.ToInt32(GridView2.DataKeys[rowIndex]["num_factura"]);
        int sucursal = Convert.ToInt32(GridView2.DataKeys[rowIndex]["sucursal"]);

        switch (sucursal)
        {
            case 1:
                nombreArchivo = "BOG" + factura + ".pdf";
                break;
            case 2:
                nombreArchivo = "MDE" + factura + ".pdf";
                break;
            case 5:
                nombreArchivo = "BAQ" + factura + ".pdf";
                break;
            case 9:
                nombreArchivo = "BUC" + factura + ".pdf";
                break;
            case 10:
                nombreArchivo = "CLI" + factura + ".pdf";
                break;
        }

        var folders = Directory.GetFiles(Server.MapPath("../DocumentosGenerados"), nombreArchivo, SearchOption.AllDirectories).FirstOrDefault();

        if (folders != null)
        {
            WebClient User = new WebClient();
            Byte[] FileBuffer = User.DownloadData(folders);

            if (FileBuffer != null)
            {
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-length", FileBuffer.Length.ToString());
                Response.BinaryWrite(FileBuffer);
            }
        }
    }

    protected void ImageButtonValidacionDIAN_Click(object sender, ImageClickEventArgs e)
    {
        ImageButton imageButton = (ImageButton)sender;
        int rowIndex = 0;

        if (GridView2.PageIndex >= 1)
        {
            int val = GridView2.PageIndex;
            int val2 = val * GridView2.PageSize;
            rowIndex = Convert.ToInt32(imageButton.ToolTip) - val2;
        }
        else
        {
            rowIndex = Convert.ToInt32(((ImageButton)sender).ToolTip);
        }

        int factura = Convert.ToInt32(GridView2.DataKeys[rowIndex]["num_factura"]);
        int sucursal = Convert.ToInt32(GridView2.DataKeys[rowIndex]["sucursal"]);

        ManejaPagos manejaPagos = new ManejaPagos();
        string UUID = manejaPagos.ObtenerCUFE(factura, sucursal);
        manejaPagos.DesconectarBD();

        Response.Redirect("https://catalogo-vpfe.dian.gov.co/document/searchqr?documentkey=" + UUID);
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        div_error_facturas_enviadas.Visible = false;
        if (TextBox1.Text.Equals(string.Empty)) div_error_facturas_enviadas.Visible = true;
        else
        {
            GridView1.DataSourceID = string.Empty;
            GridView1.DataSource = SqlDataSourceFacturasProcesadasFiltradas;
            GridView1.DataBind();
        }
    }

    protected void Button2_Click(object sender, EventArgs e)
    {
        div_error_facturas_enviadas.Visible = false;
        TextBox1.Text = String.Empty;
        DropDownList1.SelectedIndex = 0;
        GridView1.DataSourceID = string.Empty;
        GridView1.DataSource = SqlDataSourceFacturasProcesadas;
        GridView1.DataBind();
    }

    protected void Button3_Click(object sender, EventArgs e)
    {
        div_error_facturas_procesadas.Visible = false;
        if (TextBox2.Text.Equals(string.Empty)) div_error_facturas_procesadas.Visible = true;
        else
        {
            GridView2.DataSourceID = string.Empty;
            GridView2.DataSource = SqlDataSourceFacturasAprobadasDIANFiltradas;
            GridView2.DataBind();
        }
    }

    protected void Button4_Click(object sender, EventArgs e)
    {
        div_error_facturas_procesadas.Visible = false;
        TextBox2.Text = String.Empty;
        DropDownList2.SelectedIndex = 0;
        GridView2.DataSourceID = string.Empty;
        GridView2.DataSource = SqlDataSourceFacturasAprobadasDIAN;
        GridView2.DataBind();
    }

    protected void Button5_Click(object sender, EventArgs e)
    {
        div_error_facturas_errores_db.Visible = false;
        if (TextBox3.Text.Equals(string.Empty)) div_error_facturas_errores_db.Visible = true;
        else
        {
            GridView3.DataSourceID = string.Empty;
            GridView3.DataSource = SqlDataSourceFacturasErroresInternosFiltrados;
            GridView3.DataBind();
        }
    }

    protected void Button6_Click(object sender, EventArgs e)
    {
        div_error_facturas_errores_db.Visible = false;
        TextBox3.Text = String.Empty;
        DropDownList3.SelectedIndex = 0;
        GridView3.DataSourceID = string.Empty;
        GridView3.DataSource = SqlDataSourceFacturasErroresInternos;
        GridView3.DataBind();
    }

    protected void Button7_Click(object sender, EventArgs e)
    {
        div_error_facturas_errores_dian.Visible = false;
        if (TextBox4.Text.Equals(string.Empty)) div_error_facturas_errores_dian.Visible = true;
        else
        {
            GridView4.DataSourceID = string.Empty;
            GridView4.DataSource = SqlDataSourceFacturasErroresDIANFiltrados;
            GridView4.DataBind();
        }
    }

    protected void Button8_Click(object sender, EventArgs e)
    {
        div_error_facturas_errores_dian.Visible = false;
        TextBox4.Text = String.Empty;
        DropDownList4.SelectedIndex = 0;
        GridView4.DataSourceID = string.Empty;
        GridView4.DataSource = SqlDataSourceFacturasErroresDIAN;
        GridView4.DataBind();
    }
}