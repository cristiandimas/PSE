<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="RecuperarContrasena.aspx.cs" Inherits="RecuperarContrasena" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="col-lg-12 text-center center-block">
        <h3 class="section-heading" style="padding-bottom: 35px;">Recuperar Contraseña</h3>
        <hr size="10px" />
        <br>
        <div class="alert alert-info">
            <strong>Atención! </strong>
            Para recuperar su contraseña, <strong>debe ingresar la dirección de correo electrónico asociada a su cuenta de usuario.</strong> Allí se le enviará un enlace con su nueva contraseña.
        </div>
        <br>
        <!--formulario-->
        <div class=" text-center center-block">
            <div class=" form-group center-block">
                <asp:TextBox ID="TextBox4" runat="server" CssClass="form-control" TextMode="SingleLine" Placeholder="Introduce tu email" Style="border-radius: 25px;"></asp:TextBox>
            </div>

            <div>
                <asp:Button ID="Button2" runat="server" CssClass="botonimagenact center-block" ImageUrl="img/boton_actualizar1.png" Style="border-radius: 25px; height: 40px; width: 220px;" OnClick="Button2_Click" />
            </div>
            <br>

            <a href="../manuales/contrasena.pdf" target="_blank" >
                <img class="img-responsive center-block" src="img/boton_pdf.jpg" />
            </a>
        </div>

        <br />

        <div runat="server" id="div_recuperar" visible="false" class="alert alert-danger">
            <strong>Error!</strong>
            <asp:Label ID="Label3" runat="server"></asp:Label>
        </div>

        <div runat="server" class="alert alert-success" id="div_recuperar_success" visible="false">
            Se envió tu nueva contraseña al correo electrónico: 
            <strong>
                <asp:Label ID="Label1" runat="server"></asp:Label>
            </strong>. 
            <a href="Index.aspx" style="color:#01210a">Volver aquí.</a>
        </div>

    </div>
</asp:Content>

