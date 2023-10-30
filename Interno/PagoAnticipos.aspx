<%@ Page Title="" Language="C#" MasterPageFile="~/Interno/MasterPageInterna.master" AutoEventWireup="true" CodeFile="PagoAnticipos.aspx.cs" Inherits="Interno_PagoAnticipos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <div class="container">
        <div class="jumbotron">
            <h4>PAGOS EN LÍNEA DE ANTICIPOS<br />
            </h4>
            Aquí podrá realizar el pago de anticipos.
            <br />
            Según decreto 0862 de 2013, favor abtenerse de aplicar retenciones en la fuente para la equidad-cree ya que EC GROUP es autorretenedor a título de renta.
            <br />
        </div>

        <div class="table-responsive">
            <table class="table table-striped table-bordered table-hover">
                <tr>
                    <td style="width: 50%">Número documento:</td>
                    <td>
                        <asp:TextBox ID="TextBox1" CssClass="form-control" runat="server" CausesValidation="True"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td style="width: 50%">Detalle del pago:</td>
                    <td>
                        <asp:TextBox ID="TextBox3" CssClass="form-control" runat="server" CausesValidation="True"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>Valor a pagar (Pesos Colombianos COP):</td>
                    <td>
                        <asp:TextBox ID="TextBox2" CssClass="form-control" runat="server" CausesValidation="True"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="text-align:center">
                        <asp:Button ID="Button1" runat="server" CssClass="btn btn-default" Text="Pagar" OnClick="Button1_Click" />
                    </td>
                </tr>
            </table>
        </div>

        <div runat="server" class="alert alert-danger" id="div_error" visible="false">
            <span><i class="glyphicon glyphicon-remove-circle"></i></span>
            <strong>Error!</strong>
            <asp:Label ID="Label1" runat="server"></asp:Label>
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

    </div>

    <asp:HiddenField ID="HiddenFieldCliente" runat="server" />

</asp:Content>

