using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;

/// <summary>
/// Descripción breve de ManejaExcel
/// </summary>
public class ManejaExcel
{
    public ManejaExcel()
    {
        //
        // TODO: Agregar aquí la lógica del constructor
        //
    }

    public MemoryStream GenerarArchivoPagos(int codigoReferencia)
    {
        //Create the data set and table
        DataTable dt = new DataTable("New_DataTable");

        //Set the locale for each
        dt.Locale = System.Threading.Thread.CurrentThread.CurrentCulture;

        //Open a DB connection (in this example with OleDB)
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["EC_GROUPConnectionString"].ConnectionString);
        con.Open();

        //Create a query and fill the data table with the data from the DB
        string sql = "SELECT FC.CLI_DESCRIPCION AS [RAZON SOCIAL], FC.CLI_CUIT AS NIT, RCPUW.NUMERODOCUMENTO AS FACTURA, " +
                     "cast(round((RCPUW.COTDIVISALOCAL / RCPUW.COTDIVISAOTRA), 2) as numeric(36, 2)) AS TRM, RCPUW.VALORPAGO, " +
                     "RCPUW.ICA, RCPUW.RETEIVA, RCPUW.FECHAPAGO, RCPUW.TIPORELACIONDOCUMENTO, RCPUW.DETALLE FROM forward_clientes AS FC INNER JOIN " +
                     "(SELECT RCP.*, UW.CODIGO_ECCARGO FROM USUARIOSWEB AS UW INNER JOIN (SELECT RDP.*, TRD.TIPORELACIONDOCUMENTO FROM TIPORELACIONDOCUMENTOS AS TRD INNER JOIN " +
                     "(select RC.NUMERODOCUMENTO, RC.ICA, RC.RETEIVA, RC.VALORPAGO, RP.IDUSUARIOWEB, RP.FECHAPAGO, RC.COTDIVISALOCAL, RC.COTDIVISAOTRA, RC.DETALLE, RC.IDTIPORELACIONDOCUMENTO, RP.IDREFERENCIAPAGO " +
                     "From REFERENCIASPAGOS AS RP inner join RELACIONDOCUMENTOS AS RC ON RP.IDREFERENCIAPAGO = RC.IDREFERENCIAPAGO AND RP.IDESTADOREFERENCIA = 2) AS RDP ON TRD.IDTIPORELACIONDOCUMENTO = RDP.IDTIPORELACIONDOCUMENTO) AS RCP " +
                     "ON UW.IDUSUARIOWEB = RCP.IDUSUARIOWEB) AS RCPUW ON RCPUW.CODIGO_ECCARGO = FC.CLI_CODIGO WHERE RCPUW.IDREFERENCIAPAGO = " + codigoReferencia + ";";

        SqlCommand cmd = new SqlCommand(sql, con);
        SqlDataAdapter adptr = new SqlDataAdapter();

        adptr.SelectCommand = cmd;
        adptr.Fill(dt);
        con.Close();

        MemoryStream outputStream = new MemoryStream();
        using (ExcelPackage p = new ExcelPackage(outputStream))
        {
            //Here setting some document properties
            p.Workbook.Properties.Author = "EC GROUP";
            p.Workbook.Properties.Title = "PAGOS REALIZADOS MEDIANTE WEB";

            //Create a sheet
            p.Workbook.Worksheets.Add("PAGOS");
            ExcelWorksheet ws = p.Workbook.Worksheets[1];
            ws.Name = "PAGOS"; //Setting Sheet's name
            ws.Cells.Style.Font.Size = 11; //Default font size for whole sheet
            ws.Cells.Style.Font.Name = "Calibri"; //Default Font name for whole sheet

            //Merging cells and create a center heading for out table
            ws.Cells[1, 1].Value = "PAGOS";
            ws.Cells[1, 1, 1, dt.Columns.Count].Merge = true;
            ws.Cells[1, 1, 1, dt.Columns.Count].Style.Font.Bold = true;
            ws.Cells[1, 1, 1, dt.Columns.Count].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            int colIndex = 1;
            int rowIndex = 2;

            foreach (DataColumn dc in dt.Columns) //Creating Headings
            {
                var cell = ws.Cells[rowIndex, colIndex];

                //Setting the background color of header cells to Gray
                var fill = cell.Style.Fill;
                fill.PatternType = ExcelFillStyle.Solid;
                fill.BackgroundColor.SetColor(Color.Gray);

                //Setting Top/left,right/bottom borders.
                var border = cell.Style.Border;
                border.Bottom.Style =
                    border.Top.Style =
                    border.Left.Style =
                    border.Right.Style = ExcelBorderStyle.Thin;

                //Setting Value in cell
                cell.Value = dc.ColumnName.ToUpper();
                colIndex++;
            }

            foreach (DataRow dr in dt.Rows) // Adding Data into rows
            {
                colIndex = 1;
                rowIndex++;

                foreach (DataColumn dc in dt.Columns)
                {
                    var cell = ws.Cells[rowIndex, colIndex];
                    //Setting Value in cell
                    cell.Value = dr[dc.ColumnName];

                    //Setting borders of cell
                    var border = cell.Style.Border;
                    border.Left.Style =
                        border.Right.Style = ExcelBorderStyle.Thin;
                    colIndex++;
                }
            }

            p.Save();
        }

        outputStream.Position = 0;
        return outputStream;
    }
}