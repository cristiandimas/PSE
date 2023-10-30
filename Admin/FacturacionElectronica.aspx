<%@ Page Language="C#" AutoEventWireup="true" CodeFile="FacturacionElectronica.aspx.cs" Inherits="Admin_FacturacionElectronica" MasterPageFile="~/Admin/MasterPageInterna.master" %>

<asp:Content ID="Content1" runat="server" contentplaceholderid="ContentPlaceHolder1">

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="panel panel-default">
                <asp:Panel ID="pHeader" runat="server" CssClass="panel-heading">
                    <asp:Label ID="lblText" runat="server" />
                </asp:Panel>

                <asp:Panel ID="pBody" runat="server" CssClass="panel-body texter-center" HorizontalAlign="Center">
                    <table style="width: 100%;">
                        <tr>
                            <td style="width:200px;">Número Factura</td>
                            <td>
                                <asp:TextBox ID="TextBox1" runat="server" CssClass="form-control"></asp:TextBox>
                            </td>
                            <td style="width:110px;">
                                <asp:Button ID="Button1" runat="server" Text="Buscar" CssClass="btn btn-info" Width="100px" OnClick="Button1_Click" />
                            </td>
                        </tr>
                        <tr>
                            <td style="width:200px;">Sucursal</td>
                            <td>
                                <asp:DropDownList ID="DropDownList1" runat="server" CssClass="form-control" DataSourceID="SqlDataSourceSucursales" DataTextField="suc_descripcion" DataValueField="suc_numero">
                                </asp:DropDownList>
                            </td>
                            <td style="width:110px;">
                                <asp:Button ID="Button2" runat="server" Text="Quitar filtro" CssClass="btn btn-info" Width="100px" OnClick="Button2_Click" />
                            </td>
                        </tr>
                    </table>
                    <br />
                    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataSourceID="SqlDataSourceFacturasProcesadas"
                        CssClass="table table-bordered table-hover texter-center" AllowPaging="True" AllowSorting="True" Width="100%"
                        BorderWidth="0px" GridLines="None" AlternatingRowStyle-HorizontalAlign="Center" AlternatingRowStyle-VerticalAlign="Middle"
                        EditRowStyle-HorizontalAlign="Center" EditRowStyle-VerticalAlign="Middle"
                        EmptyDataRowStyle-HorizontalAlign="Center" EmptyDataRowStyle-VerticalAlign="Middle"
                        HeaderStyle-HorizontalAlign="Center" HeaderStyle-VerticalAlign="Middle" HorizontalAlign="Center"
                        RowStyle-HorizontalAlign="Center" RowStyle-VerticalAlign="Middle"
                        DataKeyNames="fed_num_factura,fed_sucursal_id" EmptyDataText="No hay datos disponibles.">
                        <Columns>
                            <asp:BoundField DataField="suc_descripcion" HeaderText="SUCURSAL" SortExpression="suc_descripcion" />
                            <asp:BoundField DataField="fed_num_factura" HeaderText="FACTURA" SortExpression="fed_num_factura" />
                            <asp:BoundField DataField="fed_factura_uuid" HeaderText="fed_factura_uuid" SortExpression="fed_factura_uuid" Visible="false" />
                            <asp:BoundField DataField="fed_fecha_registro" HeaderText="FECHA REGISTRO" SortExpression="fed_fecha_registro" />
                            <asp:BoundField DataField="fed_sucursal_id" HeaderText="fed_sucursal_id" SortExpression="fed_sucursal_id" Visible="false"></asp:BoundField>
                            <asp:TemplateField HeaderText="VALIDACION DIAN">
                                <ItemTemplate>
                                    <asp:ImageButton ID="ImageButtonValidacion" runat="server" ImageUrl="../img/descargar.png" OnClick="ImageButtonValidacion_Click" ToolTip='<%# Container.DataItemIndex %>' Width="24px" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <AlternatingRowStyle HorizontalAlign="Center" VerticalAlign="Middle"></AlternatingRowStyle>
                        <EditRowStyle HorizontalAlign="Center" VerticalAlign="Middle"></EditRowStyle>
                        <EmptyDataRowStyle HorizontalAlign="Center" VerticalAlign="Middle"></EmptyDataRowStyle>
                        <HeaderStyle BackColor="#45BAEB" Font-Bold="True" Font-Overline="False" ForeColor="White" />
                        <PagerStyle ForeColor="Black" />
                        <RowStyle HorizontalAlign="Center" VerticalAlign="Middle"></RowStyle>
                    </asp:GridView>
                    <br />
                    <div id="div_error_facturas_enviadas" runat="server" class="alert alert-danger" visible="false">
                        <strong>Error!</strong>
                        Debe ingresar todos los datos para realizar la búsqueda!
                    </div>
                    <asp:SqlDataSource ID="SqlDataSourceFacturasProcesadas" runat="server" ConnectionString="<%$ ConnectionStrings:EC_GROUPConnectionString %>" SelectCommand="SELECT fs.suc_descripcion, ffe.fed_num_factura, ffe.fed_factura_uuid, ffe.fed_fecha_registro, ffe.fed_sucursal_id FROM fed_facturas_enviadas AS ffe INNER JOIN forward_sucursal AS fs ON ffe.fed_sucursal_id = fs.suc_id ORDER BY ffe.fed_num_factura DESC"></asp:SqlDataSource>
                    <asp:SqlDataSource ID="SqlDataSourceFacturasProcesadasFiltradas" runat="server" ConnectionString="<%$ ConnectionStrings:EC_GROUPConnectionString %>" SelectCommand="SELECT fs.suc_descripcion, ffe.fed_num_factura, ffe.fed_factura_uuid, ffe.fed_fecha_registro, ffe.fed_sucursal_id FROM fed_facturas_enviadas AS ffe INNER JOIN forward_sucursal AS fs ON ffe.fed_sucursal_id = fs.suc_id
                        where ffe.fed_sucursal_id = @sucursal and ffe.fed_num_factura = @factura ORDER BY ffe.fed_num_factura DESC">
                        <SelectParameters>
                            <asp:ControlParameter ControlID="DropDownList1" Name="sucursal" PropertyName="SelectedValue" />
                            <asp:ControlParameter ControlID="TextBox1" Name="factura" PropertyName="Text" />
                        </SelectParameters>
                    </asp:SqlDataSource>
                </asp:Panel>

                <ajaxToolkit:CollapsiblePanelExtender ID="CollapsiblePanelExtender1" runat="server" TargetControlID="pBody" CollapseControlID="pHeader" ExpandControlID="pHeader"
                    Collapsed="true" TextLabelID="lblText" CollapsedText="Facturas Procesadas y Enviadas a la DIAN" ExpandedText="Facturas Procesadas y Enviadas a la DIAN"
                    CollapsedSize="0"></ajaxToolkit:CollapsiblePanelExtender>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="GridView1" />
        </Triggers>
    </asp:UpdatePanel>

    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
        <ContentTemplate>
            <div class="panel panel-default">
                <asp:Panel ID="pHeader2" runat="server" CssClass="panel-heading">
                    <asp:Label ID="lblText2" runat="server" />
                </asp:Panel>

                <asp:Panel ID="pBody2" runat="server" CssClass="panel-body">
                    <table style="width: 100%;">
                        <tr>
                            <td style="width:200px;">Número Factura</td>
                            <td>
                                <asp:TextBox ID="TextBox2" runat="server" CssClass="form-control"></asp:TextBox>
                            </td>
                            <td style="width:110px;">
                                <asp:Button ID="Button3" runat="server" Text="Buscar" CssClass="btn btn-info" Width="100px" OnClick="Button3_Click"/>
                            </td>
                        </tr>
                        <tr>
                            <td style="width:200px;">Sucursal</td>
                            <td>
                                <asp:DropDownList ID="DropDownList2" runat="server" CssClass="form-control" DataSourceID="SqlDataSourceSucursales" DataTextField="suc_descripcion" DataValueField="suc_numero">
                                </asp:DropDownList>
                            </td>
                            <td style="width:110px;">
                                <asp:Button ID="Button4" runat="server" Text="Quitar filtro" CssClass="btn btn-info" Width="100px" OnClick="Button4_Click"/>
                            </td>
                        </tr>
                    </table>
                    <br />
                    <asp:GridView ID="GridView2" runat="server" CssClass="table table-bordered table-hover texter-center" AutoGenerateColumns="False" DataSourceID="SqlDataSourceFacturasAprobadasDIAN" AllowPaging="True" AllowSorting="True" Font-Size="X-Small"
                        DataKeyNames="num_factura,sucursal" EmptyDataText="No hay datos disponibles.">
                        <Columns>
                            <asp:BoundField DataField="suc_descripcion" HeaderText="SUCURSAL" SortExpression="suc_descripcion"></asp:BoundField>
                            <asp:BoundField DataField="num_factura" HeaderText="FACTURA" SortExpression="num_factura"></asp:BoundField>
                            <asp:BoundField DataField="mensaje_respuesta" HeaderText="MENSAJE RESPUESTA" SortExpression="mensaje_respuesta"></asp:BoundField>
                            <asp:BoundField DataField="fecha_registro" HeaderText="FECHA" SortExpression="fecha_registro"></asp:BoundField>
                            <asp:CheckBoxField DataField="correo_enviado" HeaderText="CORREO ENVIADO" SortExpression="correo_enviado"></asp:CheckBoxField>
                            <asp:BoundField DataField="fed_sucursal_id" HeaderText="fed_sucursal_id" SortExpression="fed_sucursal_id" Visible="false"></asp:BoundField>
                            <asp:TemplateField HeaderText="XML">
                                <ItemTemplate>
                                    <asp:ImageButton ID="ImageButtonXML" runat="server" ImageUrl="../img/descargar.png" OnClick="ImageButtonValidacionXML_Click" ToolTip='<%# Container.DataItemIndex %>' Width="24px" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="PDF">
                                <ItemTemplate>
                                    <asp:ImageButton ID="ImageButtonPDF" runat="server" ImageUrl="../img/descargar.png" OnClick="ImageButtonValidacionPDF_Click" OnClientClick="target ='_blank';" ToolTip='<%# Container.DataItemIndex %>' Width="24px" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="DIAN">
                                <ItemTemplate>
                                    <asp:ImageButton ID="ImageButtonDIAN" runat="server" ImageUrl="../img/link-icon.png" OnClick="ImageButtonValidacionDIAN_Click" OnClientClick="target ='_blank';" ToolTip='<%# Container.DataItemIndex %>' Width="24px" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <HeaderStyle BackColor="#45BAEB" Font-Bold="True" Font-Overline="False" ForeColor="White" />
                        <AlternatingRowStyle HorizontalAlign="Center" VerticalAlign="Middle"></AlternatingRowStyle>
                        <EditRowStyle HorizontalAlign="Center" VerticalAlign="Middle"></EditRowStyle>
                        <EmptyDataRowStyle HorizontalAlign="Center" VerticalAlign="Middle"></EmptyDataRowStyle>
                        <HeaderStyle BackColor="#45BAEB" Font-Bold="True" Font-Overline="False" ForeColor="White" />
                        <PagerStyle ForeColor="Black" />
                        <RowStyle HorizontalAlign="Center" VerticalAlign="Middle"></RowStyle>
                    </asp:GridView>
                    <br />
                    <div id="div_error_facturas_procesadas" runat="server" class="alert alert-danger" visible="false">
                        <strong>Error!</strong>
                        Debe ingresar todos los datos para realizar la búsqueda!
                    </div>

                    <asp:SqlDataSource runat="server" ID="SqlDataSourceFacturasAprobadasDIAN" ConnectionString='<%$ ConnectionStrings:EC_GROUPConnectionString %>' SelectCommand="SELECT fs.suc_descripcion, ffp.num_factura, ffp.mensaje_respuesta, ffp.fecha_registro, ffp.correo_enviado, ffp.sucursal FROM fed_facturas_procesadas AS ffp INNER JOIN forward_sucursal AS fs ON ffp.sucursal = fs.suc_numero WHERE (ffp.registro_dian = 1) ORDER BY ffp.num_factura DESC"></asp:SqlDataSource>
                    <asp:SqlDataSource ID="SqlDataSourceFacturasAprobadasDIANFiltradas" runat="server" ConnectionString="<%$ ConnectionStrings:EC_GROUPConnectionString %>" SelectCommand="SELECT fs.suc_descripcion, ffp.num_factura, ffp.mensaje_respuesta, ffp.fecha_registro, ffp.correo_enviado, ffp.sucursal FROM fed_facturas_procesadas AS ffp INNER JOIN forward_sucursal AS fs 
ON ffp.sucursal = fs.suc_numero 
WHERE ffp.registro_dian = 1 and ffp.num_factura = @factura and ffp.sucursal = @sucursal ORDER BY ffp.num_factura DESC">
                        <SelectParameters>
                            <asp:ControlParameter ControlID="TextBox2" Name="factura" PropertyName="Text" />
                            <asp:ControlParameter ControlID="DropDownList2" Name="sucursal" PropertyName="SelectedValue" />
                        </SelectParameters>
                    </asp:SqlDataSource>
                </asp:Panel>

                <ajaxToolkit:CollapsiblePanelExtender ID="CollapsiblePanelExtender2" runat="server" TargetControlID="pBody2" CollapseControlID="pHeader2" ExpandControlID="pHeader2"
                    Collapsed="true" TextLabelID="lblText2" CollapsedText="Facturas Aprobadas por la DIAN" ExpandedText="Facturas Aprobadas por la DIAN"
                    CollapsedSize="0"></ajaxToolkit:CollapsiblePanelExtender>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="GridView2" />
        </Triggers>
    </asp:UpdatePanel>

    <asp:UpdatePanel ID="UpdatePanel3" runat="server">
        <ContentTemplate>
            <div class="panel panel-default">
                <asp:Panel ID="pHeader3" runat="server" CssClass="panel-heading">
                    <asp:Label ID="lblText3" runat="server" />
                </asp:Panel>

                <asp:Panel ID="pBody3" runat="server" CssClass="panel-body">
                    <table style="width: 100%;">
                        <tr>
                            <td style="width:200px;">Número Factura</td>
                            <td>
                                <asp:TextBox ID="TextBox3" runat="server" CssClass="form-control"></asp:TextBox>
                            </td>
                            <td style="width:110px;">
                                <asp:Button ID="Button5" runat="server" Text="Buscar" CssClass="btn btn-info" Width="100px" OnClick="Button5_Click"/>
                            </td>
                        </tr>
                        <tr>
                            <td style="width:200px;">Sucursal</td>
                            <td>
                                <asp:DropDownList ID="DropDownList3" runat="server" CssClass="form-control" DataSourceID="SqlDataSourceSucursales" DataTextField="suc_descripcion" DataValueField="suc_numero">
                                </asp:DropDownList>
                            </td>
                            <td style="width:110px;">
                                <asp:Button ID="Button6" runat="server" Text="Quitar filtro" CssClass="btn btn-info" Width="100px" OnClick="Button6_Click"/>
                            </td>
                        </tr>
                    </table>
                    <br />
                    <asp:GridView ID="GridView3" runat="server" AutoGenerateColumns="False" DataSourceID="SqlDataSourceFacturasErroresInternos"
                        CssClass="table table-bordered table-hover texter-center" AllowPaging="True" AllowSorting="True"
                        BorderWidth="0px" GridLines="None" AlternatingRowStyle-HorizontalAlign="Center" AlternatingRowStyle-VerticalAlign="Middle"
                        EditRowStyle-HorizontalAlign="Center" EditRowStyle-VerticalAlign="Middle"
                        EmptyDataRowStyle-HorizontalAlign="Center" EmptyDataRowStyle-VerticalAlign="Middle"
                        HeaderStyle-HorizontalAlign="Center" HeaderStyle-VerticalAlign="Middle" HorizontalAlign="Center"
                        RowStyle-HorizontalAlign="Center" RowStyle-VerticalAlign="Middle" EmptyDataText="No hay datos disponibles.">
                        <Columns>
                            <asp:BoundField DataField="suc_descripcion" HeaderText="SUCURSAL" SortExpression="suc_descripcion"></asp:BoundField>
                            <asp:BoundField DataField="fed_num_factura" HeaderText="FACTURA" SortExpression="fed_num_factura"></asp:BoundField>
                            <asp:BoundField DataField="fed_error" HeaderText="ERROR" SortExpression="fed_error"></asp:BoundField>
                            <asp:BoundField DataField="fed_fecha_registro" HeaderText="FECHA REGISTRO" SortExpression="fed_fecha_registro"></asp:BoundField>
                        </Columns>
                        <AlternatingRowStyle HorizontalAlign="Center" VerticalAlign="Middle"></AlternatingRowStyle>
                        <EditRowStyle HorizontalAlign="Center" VerticalAlign="Middle"></EditRowStyle>
                        <EmptyDataRowStyle HorizontalAlign="Center" VerticalAlign="Middle"></EmptyDataRowStyle>
                        <HeaderStyle BackColor="#45BAEB" Font-Bold="True" Font-Overline="False" ForeColor="White" />
                        <PagerStyle ForeColor="Black" />
                        <RowStyle HorizontalAlign="Center" VerticalAlign="Middle"></RowStyle>
                    </asp:GridView>
                    <br />
                    <div id="div_error_facturas_errores_db" runat="server" class="alert alert-danger" visible="false">
                        <strong>Error!</strong>
                        Debe ingresar todos los datos para realizar la búsqueda!
                    </div>
                    <asp:SqlDataSource runat="server" ID="SqlDataSourceFacturasErroresInternos" ConnectionString='<%$ ConnectionStrings:EC_GROUPConnectionString %>' SelectCommand="SELECT forward_sucursal.suc_descripcion, fed_facturas_no_enviadas.fed_num_factura, fed_facturas_no_enviadas.fed_error, fed_facturas_no_enviadas.fed_fecha_registro FROM fed_facturas_no_enviadas INNER JOIN forward_sucursal ON fed_facturas_no_enviadas.fed_sucursal_id = forward_sucursal.suc_numero ORDER BY fed_facturas_no_enviadas.fed_num_factura DESC"></asp:SqlDataSource>
                    <asp:SqlDataSource ID="SqlDataSourceFacturasErroresInternosFiltrados" runat="server" ConnectionString="<%$ ConnectionStrings:EC_GROUPConnectionString %>" SelectCommand="SELECT forward_sucursal.suc_descripcion, fed_facturas_no_enviadas.fed_num_factura, fed_facturas_no_enviadas.fed_error, fed_facturas_no_enviadas.fed_fecha_registro FROM fed_facturas_no_enviadas INNER JOIN forward_sucursal ON fed_facturas_no_enviadas.fed_sucursal_id = forward_sucursal.suc_numero
where fed_facturas_no_enviadas.fed_num_factura = @factura and forward_sucursal.suc_numero = @sucursal ORDER BY fed_facturas_no_enviadas.fed_num_factura DESC">
                        <SelectParameters>
                            <asp:ControlParameter ControlID="TextBox3" Name="factura" PropertyName="Text" />
                            <asp:ControlParameter ControlID="DropDownList3" Name="sucursal" PropertyName="SelectedValue" />
                        </SelectParameters>
                    </asp:SqlDataSource>
                </asp:Panel>

                <ajaxToolkit:CollapsiblePanelExtender ID="CollapsiblePanelExtender3" runat="server" TargetControlID="pBody3" CollapseControlID="pHeader3" ExpandControlID="pHeader3"
                    Collapsed="true" TextLabelID="lblText3" CollapsedText="Facturas Con Errores Internos" ExpandedText="Facturas Con Errores Internos"
                    CollapsedSize="0"></ajaxToolkit:CollapsiblePanelExtender>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdatePanel ID="UpdatePanel4" runat="server">
        <ContentTemplate>
            <div class="panel panel-default">
                <asp:Panel ID="pHeader4" runat="server" CssClass="panel-heading">
                    <asp:Label ID="lblText4" runat="server" />
                </asp:Panel>

                <asp:Panel ID="pBody4" runat="server" CssClass="panel-body">
                    <table style="width: 100%;">
                        <tr>
                            <td style="width:200px;">Número Factura</td>
                            <td>
                                <asp:TextBox ID="TextBox4" runat="server" CssClass="form-control"></asp:TextBox>
                            </td>
                            <td style="width:110px;">
                                <asp:Button ID="Button7" runat="server" Text="Buscar" CssClass="btn btn-info" Width="100px" OnClick="Button7_Click"/>
                            </td>
                        </tr>
                        <tr>
                            <td style="width:200px;">Sucursal</td>
                            <td>
                                <asp:DropDownList ID="DropDownList4" runat="server" CssClass="form-control" DataSourceID="SqlDataSourceSucursales" DataTextField="suc_descripcion" DataValueField="suc_numero">
                                </asp:DropDownList>
                            </td>
                            <td style="width:110px;">
                                <asp:Button ID="Button8" runat="server" Text="Quitar filtro" CssClass="btn btn-info" Width="100px" OnClick="Button8_Click"/>
                            </td>
                        </tr>
                    </table>
                    <br />
                    <asp:GridView ID="GridView4" runat="server" AutoGenerateColumns="False" DataSourceID="SqlDataSourceFacturasErroresDIAN"
                        CssClass="table table-bordered table-hover texter-center" AllowPaging="True" AllowSorting="True"
                        BorderWidth="0px" GridLines="None" AlternatingRowStyle-HorizontalAlign="Center" AlternatingRowStyle-VerticalAlign="Middle"
                        EditRowStyle-HorizontalAlign="Center" EditRowStyle-VerticalAlign="Middle"
                        EmptyDataRowStyle-HorizontalAlign="Center" EmptyDataRowStyle-VerticalAlign="Middle"
                        HeaderStyle-HorizontalAlign="Center" HeaderStyle-VerticalAlign="Middle" HorizontalAlign="Center"
                        RowStyle-HorizontalAlign="Center" RowStyle-VerticalAlign="Middle" EmptyDataText="No hay datos disponibles.">
                        <Columns>
                            <asp:BoundField DataField="suc_descripcion" HeaderText="SUCURSAL" SortExpression="suc_descripcion"></asp:BoundField>
                            <asp:BoundField DataField="num_factura" HeaderText="FACTURA" SortExpression="num_factura"></asp:BoundField>
                            <asp:BoundField DataField="mensaje_respuesta" HeaderText="MENSAJE RESPUESTA" SortExpression="mensaje_respuesta"></asp:BoundField>
                            <asp:CheckBoxField DataField="registro_dian" HeaderText="REGISTRO DIAN" SortExpression="registro_dian"></asp:CheckBoxField>
                            <asp:BoundField DataField="fecha_registro" HeaderText="FECHA REGISTRO" SortExpression="fecha_registro"></asp:BoundField>
                        </Columns>
                        <AlternatingRowStyle HorizontalAlign="Center" VerticalAlign="Middle"></AlternatingRowStyle>
                        <EditRowStyle HorizontalAlign="Center" VerticalAlign="Middle"></EditRowStyle>
                        <EmptyDataRowStyle HorizontalAlign="Center" VerticalAlign="Middle"></EmptyDataRowStyle>
                        <HeaderStyle BackColor="#45BAEB" Font-Bold="True" Font-Overline="False" ForeColor="White" />
                        <PagerStyle ForeColor="Black" />
                        <RowStyle HorizontalAlign="Center" VerticalAlign="Middle"></RowStyle>
                    </asp:GridView>
                    <br />
                    <div id="div_error_facturas_errores_dian" runat="server" class="alert alert-danger" visible="false">
                        <strong>Error!</strong>
                        Debe ingresar todos los datos para realizar la búsqueda!
                    </div>
                    <asp:SqlDataSource runat="server" ID="SqlDataSourceFacturasErroresDIAN" ConnectionString='<%$ ConnectionStrings:EC_GROUPConnectionString %>' SelectCommand="SELECT forward_sucursal.suc_descripcion, fed_facturas_procesadas.num_factura, fed_facturas_procesadas.mensaje_respuesta, fed_facturas_procesadas.registro_dian, fed_facturas_procesadas.fecha_registro FROM fed_facturas_procesadas INNER JOIN forward_sucursal ON fed_facturas_procesadas.sucursal = forward_sucursal.suc_numero WHERE (fed_facturas_procesadas.registro_dian = 0) ORDER BY fed_facturas_procesadas.num_factura DESC"></asp:SqlDataSource>
                    <asp:SqlDataSource ID="SqlDataSourceFacturasErroresDIANFiltrados" runat="server" ConnectionString="<%$ ConnectionStrings:EC_GROUPConnectionString %>" SelectCommand="SELECT forward_sucursal.suc_descripcion, fed_facturas_procesadas.num_factura, fed_facturas_procesadas.mensaje_respuesta, fed_facturas_procesadas.registro_dian, fed_facturas_procesadas.fecha_registro FROM fed_facturas_procesadas INNER JOIN forward_sucursal ON fed_facturas_procesadas.sucursal = forward_sucursal.suc_numero WHERE (fed_facturas_procesadas.registro_dian = 0) and fed_facturas_procesadas.num_factura = @factura and fed_facturas_procesadas.sucursal = @sucursal ORDER BY fed_facturas_procesadas.num_factura DESC">
                        <SelectParameters>
                            <asp:ControlParameter ControlID="TextBox4" Name="factura" PropertyName="Text" />
                            <asp:ControlParameter ControlID="DropDownList4" Name="sucursal" PropertyName="SelectedValue" />
                        </SelectParameters>
                    </asp:SqlDataSource>
                </asp:Panel>

                <ajaxToolkit:CollapsiblePanelExtender ID="CollapsiblePanelExtender4" runat="server" TargetControlID="pBody4" CollapseControlID="pHeader4" ExpandControlID="pHeader4"
                    Collapsed="true" TextLabelID="lblText4" CollapsedText="Facturas Rechazadas Por La DIAN" ExpandedText="Facturas Rechazadas Por La DIAN"
                    CollapsedSize="0"></ajaxToolkit:CollapsiblePanelExtender>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdatePanel ID="UpdatePanel5" runat="server">
        <ContentTemplate>
            <div class="panel panel-default">
                <asp:Panel ID="pHeader5" runat="server" CssClass="panel-heading">
                    <asp:Label ID="lblText5" runat="server" />
                </asp:Panel>

                <asp:Panel ID="pBody5" runat="server" CssClass="panel-body">
                    <asp:GridView ID="GridView5" runat="server" AutoGenerateColumns="False" DataKeyNames="fed_parametro_id" DataSourceID="SqlDataSourceParametros"
                        CssClass="table table-bordered table-hover texter-center" AllowPaging="True" AllowSorting="True"
                        BorderWidth="0px" GridLines="None" AlternatingRowStyle-HorizontalAlign="Center" AlternatingRowStyle-VerticalAlign="Middle"
                        EditRowStyle-HorizontalAlign="Center" EditRowStyle-VerticalAlign="Middle"
                        EmptyDataRowStyle-HorizontalAlign="Center" EmptyDataRowStyle-VerticalAlign="Middle"
                        HeaderStyle-HorizontalAlign="Center" HeaderStyle-VerticalAlign="Middle" HorizontalAlign="Center"
                        RowStyle-HorizontalAlign="Center" RowStyle-VerticalAlign="Middle" EmptyDataText="No hay datos disponibles." AutoGenerateEditButton="True">
                        <Columns>
                            <asp:BoundField DataField="fed_parametro_id" HeaderText="ID" ReadOnly="True" InsertVisible="False" SortExpression="fed_parametro_id"></asp:BoundField>
                            <asp:BoundField DataField="fed_ruta_certificado" HeaderText="RUTA CERTIFICADO" SortExpression="fed_ruta_certificado"></asp:BoundField>
                            <asp:BoundField DataField="fed_password_certificado" HeaderText="PASSWORD CERTIFICADO" SortExpression="fed_password_certificado"></asp:BoundField>

                            <asp:TemplateField HeaderText="FECHA INICIAL CONSULTA">
                                <ItemTemplate>
                                    <%#Eval("fed_fecha_consulta_inicial", "{0:dd/MM/yyyy}")%>
                                </ItemTemplate>

                                <EditItemTemplate>
                                    <asp:TextBox ID="d2" runat="server" Text='<%#Bind("fed_fecha_consulta_inicial","{0:dd/MM/yyyy}")%>'></asp:TextBox>
                                    <asp:ImageButton ID="ImageButtonDateTime" runat="server" CausesValidation="False" ImageUrl="~/img/calendar.png" Width="20px" Height="20px" />
                                    <ajaxToolkit:CalendarExtender TargetControlID="d2" Format="dd/MM/yyyy" PopupButtonID="ImageButtonDateTime" ID="CalendarExtender1" runat="server"></ajaxToolkit:CalendarExtender>
                                    <asp:RequiredFieldValidator
                                        ControlToValidate="d2"
                                        runat="server"
                                        ErrorMessage="Por favor ingrese una fecha válida."
                                        Text="*" />
                                </EditItemTemplate>
                            </asp:TemplateField>

                            <asp:BoundField DataField="fed_ruta_documentos" HeaderText="RUTA DOCUMENTOS GENERADOS" SortExpression="fed_ruta_documentos"></asp:BoundField>
                            <asp:BoundField DataField="fed_ruta_pdfs" HeaderText="RUTA PDF's FACTURAS" SortExpression="fed_ruta_pdfs"></asp:BoundField>
                        </Columns>
                        <AlternatingRowStyle HorizontalAlign="Center" VerticalAlign="Middle"></AlternatingRowStyle>
                        <EditRowStyle HorizontalAlign="Center" VerticalAlign="Middle"></EditRowStyle>
                        <EmptyDataRowStyle HorizontalAlign="Center" VerticalAlign="Middle"></EmptyDataRowStyle>
                        <HeaderStyle BackColor="#45BAEB" Font-Bold="True" Font-Overline="False" ForeColor="White" />
                        <PagerStyle ForeColor="Black" />
                        <RowStyle HorizontalAlign="Center" VerticalAlign="Middle"></RowStyle>
                    </asp:GridView>
                    <asp:SqlDataSource runat="server" ID="SqlDataSourceParametros" ConnectionString='<%$ ConnectionStrings:EC_GROUPConnectionString %>' 
                        SelectCommand="SELECT [fed_parametro_id], [fed_ruta_certificado], [fed_password_certificado], [fed_fecha_consulta_inicial], [fed_ruta_documentos], [fed_ruta_pdfs] FROM [fed_parametros]" 
                        ConflictDetection="CompareAllValues" 
                        DeleteCommand="DELETE FROM [fed_parametros] WHERE [fed_parametro_id] = @original_fed_parametro_id AND [fed_ruta_certificado] = @original_fed_ruta_certificado AND [fed_password_certificado] = @original_fed_password_certificado AND [fed_fecha_consulta_inicial] = @original_fed_fecha_consulta_inicial AND [fed_ruta_documentos] = @original_fed_ruta_documentos AND [fed_ruta_pdfs] = @original_fed_ruta_pdfs" 
                        InsertCommand="INSERT INTO [fed_parametros] ([fed_ruta_certificado], [fed_password_certificado], [fed_fecha_consulta_inicial], [fed_ruta_documentos], [fed_ruta_pdfs]) VALUES (@fed_ruta_certificado, @fed_password_certificado, @fed_fecha_consulta_inicial, @fed_ruta_documentos, @fed_ruta_pdfs)" 
                        OldValuesParameterFormatString="original_{0}"
                        UpdateCommand="UPDATE [fed_parametros] SET [fed_ruta_certificado] = @fed_ruta_certificado, [fed_password_certificado] = @fed_password_certificado, [fed_fecha_consulta_inicial] = @fed_fecha_consulta_inicial, [fed_ruta_documentos] = @fed_ruta_documentos, [fed_ruta_pdfs] = @fed_ruta_pdfs WHERE [fed_parametro_id] = @original_fed_parametro_id AND [fed_ruta_certificado] = @original_fed_ruta_certificado AND [fed_password_certificado] = @original_fed_password_certificado AND [fed_fecha_consulta_inicial] = @original_fed_fecha_consulta_inicial AND [fed_ruta_documentos] = @original_fed_ruta_documentos AND [fed_ruta_pdfs] = @original_fed_ruta_pdfs">
                        <DeleteParameters>
                            <asp:Parameter Name="original_fed_parametro_id" Type="Int32"></asp:Parameter>
                            <asp:Parameter Name="original_fed_ruta_certificado" Type="String"></asp:Parameter>
                            <asp:Parameter Name="original_fed_password_certificado" Type="String"></asp:Parameter>
                            <asp:Parameter Name="original_fed_fecha_consulta_inicial" Type="DateTime"></asp:Parameter>
                            <asp:Parameter Name="original_fed_ruta_documentos" Type="String"></asp:Parameter>
                            <asp:Parameter Name="original_fed_ruta_pdfs" Type="String"></asp:Parameter>
                        </DeleteParameters>
                        <InsertParameters>
                            <asp:Parameter Name="fed_ruta_certificado" Type="String"></asp:Parameter>
                            <asp:Parameter Name="fed_password_certificado" Type="String"></asp:Parameter>
                            <asp:Parameter Name="fed_fecha_consulta_inicial" Type="DateTime"></asp:Parameter>
                            <asp:Parameter Name="fed_ruta_documentos" Type="String"></asp:Parameter>
                            <asp:Parameter Name="fed_ruta_pdfs" Type="String"></asp:Parameter>
                        </InsertParameters>
                        <UpdateParameters>
                            <asp:Parameter Name="fed_ruta_certificado" Type="String"></asp:Parameter>
                            <asp:Parameter Name="fed_password_certificado" Type="String"></asp:Parameter>
                            <asp:Parameter Name="fed_fecha_consulta_inicial" Type="DateTime"></asp:Parameter>
                            <asp:Parameter Name="fed_ruta_documentos" Type="String"></asp:Parameter>
                            <asp:Parameter Name="fed_ruta_pdfs" Type="String"></asp:Parameter>
                            <asp:Parameter Name="original_fed_parametro_id" Type="Int32"></asp:Parameter>
                            <asp:Parameter Name="original_fed_ruta_certificado" Type="String"></asp:Parameter>
                            <asp:Parameter Name="original_fed_password_certificado" Type="String"></asp:Parameter>
                            <asp:Parameter Name="original_fed_fecha_consulta_inicial" Type="DateTime"></asp:Parameter>
                            <asp:Parameter Name="original_fed_ruta_documentos" Type="String"></asp:Parameter>
                            <asp:Parameter Name="original_fed_ruta_pdfs" Type="String"></asp:Parameter>
                        </UpdateParameters>
                    </asp:SqlDataSource>
                </asp:Panel>

                <ajaxToolkit:CollapsiblePanelExtender ID="CollapsiblePanelExtender5" runat="server" TargetControlID="pBody5" CollapseControlID="pHeader5" ExpandControlID="pHeader5"
                    Collapsed="true" TextLabelID="lblText5" CollapsedText="Parametros Aplicación" ExpandedText="Parametros Aplicación"
                    CollapsedSize="0"></ajaxToolkit:CollapsiblePanelExtender>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:SqlDataSource ID="SqlDataSourceSucursales" runat="server" ConnectionString="<%$ ConnectionStrings:EC_GROUPConnectionString %>" SelectCommand="SELECT [suc_descripcion], [suc_numero] FROM [forward_sucursal]"></asp:SqlDataSource>
</asp:Content>
