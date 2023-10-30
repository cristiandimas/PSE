<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RegistroUsuarios.aspx.cs" Inherits="RegistroUsuarios" MasterPageFile="~/MasterPage.master" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
    <div class="col-lg-12 text-center center-block">
        <h3 class="section-heading" style="padding-bottom: 35px;">Registro de datos de ingreso</h3>
        <hr size="10px" />
        <br>
        <!--formulario-->
        <div class="container">
            <div class=" form-group center-block">
                <label for="ejemplo_email_1">Direccion de correo electrónico</label>
                <asp:TextBox ID="TextBox3" runat="server" CssClass="form-control" TextMode="SingleLine" Placeholder="Introduce tu email" Style="border-radius: 25px;"></asp:TextBox>
            </div>

           <%-- <div class=" form-group">
                <label for="ejemplo_password_1">Nueva contraseña</label>
                <asp:TextBox ID="TextBox1" runat="server" CssClass="form-control" TextMode="Password" Placeholder="Nueva Contraseña" Style="border-radius: 25px;"></asp:TextBox>
            </div>

            <div class=" form-group">
                <label for="ejemplo_password_1">Confirmar contraseña</label>
                <asp:TextBox ID="TextBox2" runat="server" CssClass="form-control" TextMode="Password" Placeholder="Confirmar Contraseña" style="border-radius: 25px;"></asp:TextBox>
            </div>--%>

            <div>
                <asp:Button ID="Button1" runat="server" OnClick="Button1_Click1" CssClass="botonimagenact center-block" ImageUrl="img/ingresar.png" Style="border-radius: 25px; height: 40px; width: 220px;" />
            </div>

            <br />

            <div runat="server" id="div_actualizar" visible="false" class="alert alert-danger">
                <strong>Error!</strong>
                <asp:Label ID="Label1" runat="server"></asp:Label>
            </div>

            <div runat="server" class="alert alert-success" id="div_success" visible="false">
                <strong>Éxito!</strong> El usuario ha sido actualizado correctamente. Para continuar haga clic
                <asp:LinkButton ID="LinkButton1" runat="server" OnClick="LinkButton1_Click"> aquí.</asp:LinkButton>
            </div>
        </div>
        <!--container-->
    </div>
</asp:Content>

