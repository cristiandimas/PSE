<%@ Page Title="" Language="C#" MasterPageFile="~/Interno/MasterPageInterna.master" AutoEventWireup="true" CodeFile="PagosPSE.aspx.cs" Inherits="Interno_PagosPSE" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <div class="container">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
            <ContentTemplate>
                <div class="row">
                    <div>
                        <p class="" style="color: #144381; float: right;">
                            <strong>TMR HOY:
                            <asp:Label ID="Label5" runat="server"></asp:Label></strong>
                        </p>
                    </div>
                    <br>
                    <hr size="10px" />
                </div>
                <div class="row center-block">
                    <div class="col-lg-12 text-center center-block">
                        <!--titulo 1-->
                        <h4 class="section-heading">Total cartera</h4>
                        <hr size="10px" />

                        <!--formulario saldo-->
                        <div class="container">
                            <div class="table-responsive center-block" style="width: 50%;">
                                <table class="table table-bordered ">
                                    <tr>
                                        <td>Saldo corriente</td>
                                        <td>
                                            <asp:Label ID="Label6" runat="server"></asp:Label>

                                        </td>
                                    </tr>
                                    <tr>
                                        <td>Saldo Vencido</td>
                                        <td>
                                            <asp:Label ID="Label7" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>Saldo Total</td>
                                        <td>
                                            <asp:Label ID="Label8" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <!--container-->
                        </div>
                    </div>
                </div>
                <asp:HiddenField ID="HiddenFieldCliente" runat="server" />
                <asp:Button ID="btnActualizarValores" runat="server" OnClick="BtnActualizarValores_Click" Style="display: none" />
            </ContentTemplate>
        </asp:UpdatePanel>

        <asp:Panel ID="Panel2" runat="server">
            <div class="row center-block">
                <!--titulo-->
                <div class="col-lg-12 text-center center-block">
                    <h4 class="section-heading ">Favor seleccionar los documentos a realizar el pago</h4>
                    <hr size="10px" />
                </div>

                <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataSourceID="SqlDataSourceFacturasPendientes"
                    CssClass="table table-bordered table-hover texter-center" DataKeyNames="fac_numero,fac_sucursal" AllowPaging="True"
                    OnDataBound="GridView1_DataBound" ShowFooter="True" EmptyDataText="No tiene facturas pendientes por pagar."
                    OnRowDataBound="GridView1_RowDataBound" OnPageIndexChanging="PaginateTheData">
                    <Columns>
                        <asp:TemplateField HeaderText="Seleccionar">
                            <EditItemTemplate>
                                <asp:CheckBox AutoPostBack="True" ID="CheckBox1" runat="server" />
                            </EditItemTemplate>
                            <HeaderTemplate>
                                <asp:CheckBox ID="CheckBox2" runat="server" AutoPostBack="True"
                                    OnCheckedChanged="CheckBox2_CheckedChanged" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:CheckBox AutoPostBack="True" ID="CheckBox3" runat="server"
                                    OnCheckedChanged="CheckBox3_CheckedChanged" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="tipo" HeaderText="Tipo Documento" SortExpression="tipo" />
                        <asp:BoundField DataField="suc_descripcion" HeaderText="Sucursal" SortExpression="suc_descripcion" />
                        <asp:BoundField DataField="fac_numero" HeaderText="Número Documento" SortExpression="fac_numero" />
                        <asp:BoundField DataField="fac_fecvto" HeaderText="Fecha Vencimiento" SortExpression="fac_fecvto" />
                        <asp:BoundField DataField="fac_mora" HeaderText="Días de Mora" SortExpression="fac_mora" />
                        <asp:TemplateField HeaderText="Valor Total (USD)">
                            <EditItemTemplate>
                                <asp:Label ID="LabelGW1" runat="server" />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelGW2" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor Total (COP)">
                            <EditItemTemplate>
                                <asp:Label ID="LabelGW3" runat="server" />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelGW4" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Saldo (USD)">
                            <EditItemTemplate>
                                <asp:Label ID="LabelGW5" runat="server" />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelGW6" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Saldo (COP)">
                            <EditItemTemplate>
                                <asp:Label ID="LabelGW7" runat="server" />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelGW8" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Factura">
                            <ItemTemplate>
                                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/img/icono_pdf.png" OnClick="ImageButton1_Click" OnClientClick="target ='_blank';" ToolTip='<%# Container.DataItemIndex %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <HeaderStyle BackColor="#45BAEB" Font-Bold="True" Font-Overline="False" ForeColor="White" />
                </asp:GridView>
            </div>
        </asp:Panel>

        <asp:Panel ID="Panel1" runat="server" Visible="False" Style="text-align: center">

            <div>
                <div class="col-lg-12 text-center center-block">
                    <h4 class="section-heading ">Información para realizar el pago</h4>
                    <hr size="10px" />
                </div>

                <table class="table">
                    <tr>
                        <td style="width: 40%">Números de factura seleccionados: </td>
                        <td colspan="2" style="background-color: #FFFFFF"></td>
                        <td style="width: 30%; background-color: #FFFFFF">
                            <asp:Label ID="Label2" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="height: 36px">Valor total: (Pesos colombianos COP)</td>
                        <td colspan="2" style="height: 36px; background-color: #FFFFFF"></td>
                        <td style="height: 36px; background-color: #FFFFFF">
                            <asp:Label ID="Label4" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>Valor a pagar: (Pesos colombianos COP)</td>
                        <td colspan="2" style="background-color: #45BAEB">&nbsp;</td>
                        <td style="background-color: #45BAEB">
                            <asp:Label ID="Label10" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4">
                            <asp:ImageButton ID="Button1" runat="server" CssClass="btn btn-default" OnClick="Button1_Click" OnClientClick="$('#overlay').modal('show');" Text="Pagar" Width="150px" ImageUrl="~/img/pagar.png" />
                            &nbsp;<asp:ImageButton ID="Button2" runat="server" CssClass="btn btn-default" OnClick="Button2_Click" Text="Regresar" Width="150px" ImageUrl="~/img/regresar.png" />
                        </td>
                    </tr>
                </table>
            </div>

        </asp:Panel>
    </div>

    <div id="overlay" class="modal fade" role="dialog" data-keyboard="false" data-backdrop="static">
        <div class="modal-dialog">
            <div class="dialogTopCenter dialogTopCenterTitle">
                <div class="dialogTopCenterClose fRight" data-dismiss="modal"></div>
            </div>
            <div class="dialogContentCenter p0_15">
            </div>
        </div>
    </div>

    <asp:SqlDataSource ID="SqlDataSourceFacturasPendientes" runat="server" ConnectionString="<%$ ConnectionStrings:EC_GROUPConnectionString %>"
        SelectCommand="SELECT tipo, suc_descripcion, fac_numero, fac_fecha, fac_rsocial, fac_total, cta_saldo, fac_fecvto, fac_mora, fac_sucursal from
       (SELECT 'FACTURA' as tipo, v_forward_facturas.fac_numero, CONVERT (VARCHAR(100), v_forward_facturas.fac_fecha, 103) AS fac_fecha, v_forward_facturas.fac_rsocial,
        v_forward_facturas.fac_total, forward_ctacte.cta_saldo, CONVERT (VARCHAR(100), v_forward_facturas.fac_fecvto, 103) AS fac_fecvto, CASE WHEN DATEDIFF(day , [fac_fecvto],
        GETDATE()) &lt; 0 THEN 0 ELSE DATEDIFF(day , [fac_fecvto] , GETDATE()) END AS fac_mora, v_forward_facturas.fac_sucursal
        FROM v_forward_facturas INNER JOIN forward_ctacte ON v_forward_facturas.fac_numero = forward_ctacte.cta_numero 
        WHERE (v_forward_facturas.fac_cliente =  @fac_cliente) AND (forward_ctacte.cta_saldo &gt; 0) AND (forward_ctacte.cta_cliente =  @fac_cliente) 
        AND (v_forward_facturas.fac_anulada = 0) and (forward_ctacte.cta_debehaber = 'D') and (forward_ctacte.cta_comprobante = 'FC')) as fact JOIN forward_sucursal as suc on fact.fac_sucursal = suc.suc_numero	
        ORDER BY fac_mora ASC, fact.fac_numero">
        <SelectParameters>
            <asp:ControlParameter ControlID="HiddenFieldCliente" Name="fac_cliente" PropertyName="Value" Type="String" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSourceReteICA" runat="server" ConnectionString="<%$ ConnectionStrings:EC_GROUPConnectionString %>" SelectCommand="SELECT [RETEICA], [VALORRETEICA] FROM [DROPDOWNRETEICA] ORDER BY [RETEICA]"></asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSourceReteIVA" runat="server" ConnectionString="<%$ ConnectionStrings:EC_GROUPConnectionString %>" SelectCommand="SELECT [RETEIVA], [VALORRETEIVA] FROM [DROPDOWNRETEIVA]"></asp:SqlDataSource>
    <asp:HiddenField ID="HiddenFieldValorAPagar" runat="server" />
    <asp:HiddenField ID="HiddenFieldListaFacturas" runat="server" />
    <asp:HiddenField ID="HiddenFieldListaValores" runat="server" />
    <asp:HiddenField ID="HiddenFieldDivisaLocal" runat="server" />
    <asp:HiddenField ID="HiddenFieldDivisaOtra" runat="server" />
    <asp:HiddenField ID="HiddenFieldReteICA" runat="server" />
    <asp:HiddenField ID="HiddenFieldSucursalFactura" runat="server" />
    <asp:HiddenField ID="HiddenFieldReteIVA" runat="server" />

    <script>
        document.getElementById('<%= btnActualizarValores.ClientID %>').click();
    </script>
</asp:Content>

