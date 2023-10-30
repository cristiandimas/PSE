<%@ Page Title="" Language="C#" MasterPageFile="~/Interno/MasterPageInterna.master" AutoEventWireup="true" CodeFile="RelacionPagos.aspx.cs" Inherits="Interno_RelacionPagos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <div class="container">
        <div class="jumbotron">
            <h4>RELACIÓN DE PAGOS REALIZADOS<br />
            </h4>
            Aquí podrá revisar los pagos realizados y a que fueron aplicados.
        </div>
    </div>

    <div class="table-responsive">
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataKeyNames="IDREFERENCIAPAGO" DataSourceID="SqlDataSourceReferenciasPago"
            CssClass="table table-striped table-bordered table-hover">
            <Columns>
                <asp:BoundField DataField="IDREFERENCIAPAGO" HeaderText="IDREFERENCIAPAGO" ReadOnly="True" InsertVisible="False" SortExpression="IDREFERENCIAPAGO"></asp:BoundField>
                <asp:BoundField DataField="VALORREFERENCIA" HeaderText="VALORREFERENCIA" SortExpression="VALORREFERENCIA"></asp:BoundField>
                <asp:BoundField DataField="FECHACREACION" HeaderText="FECHACREACION" SortExpression="FECHACREACION"></asp:BoundField>
                <asp:BoundField DataField="FECHAPAGO" HeaderText="FECHAPAGO" SortExpression="FECHAPAGO"></asp:BoundField>
                <asp:TemplateField HeaderText="Ver" >
                    <ItemTemplate>
                        <asp:LinkButton ID="Ver" runat="server" CommandArgument='<%# Eval("IDREFERENCIAPAGO") %>' OnClick="LinkButton_Click">Ver</asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>

    <asp:Panel ID="Panel1" runat="server" Visible="False">
        <h5>Lista de facturas que se pagaron con la referencia <asp:Label ID="Label1" runat="server"></asp:Label></h5>

        <asp:Label ID="Label2" runat="server"></asp:Label>
    </asp:Panel>

    <asp:SqlDataSource runat="server" ID="SqlDataSourceReferenciasPago" ConnectionString='<%$ ConnectionStrings:EC_GROUPConnectionString %>' SelectCommand="SELECT REFERENCIASPAGOS.IDREFERENCIAPAGO, REFERENCIASPAGOS.VALORREFERENCIA, REFERENCIASPAGOS.FECHACREACION, REFERENCIASPAGOS.FECHAPAGO
FROM REFERENCIASPAGOS INNER JOIN USUARIOSWEB ON REFERENCIASPAGOS.IDUSUARIOWEB = USUARIOSWEB.IDUSUARIOWEB WHERE (REFERENCIASPAGOS.IDESTADOREFERENCIA = @IDESTADOREFERENCIA) AND (USUARIOSWEB.CODIGO_ECCARGO = @CODIGOEC)">
        <SelectParameters>
            <asp:Parameter DefaultValue="2" Name="IDESTADOREFERENCIA" Type="Decimal" />
            <asp:ControlParameter ControlID="HiddenFieldCodigoCliente" DefaultValue="" Name="CODIGOEC" PropertyName="Value" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:HiddenField ID="HiddenFieldCodigoCliente" runat="server" />
</asp:Content>

