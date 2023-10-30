<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Index.aspx.cs" Inherits="Index" %>

<!DOCTYPE html>
<html lang="en">

<head>

    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <meta name="description" content="">
    <meta name="author" content="">

    <title>EC Cargo</title>

    <!-- Bootstrap Core CSS -->
    <link href="css/bootstrap.min.css" rel="stylesheet">

    <!-- Custom CSS -->
    <link href="css/agency.css" rel="stylesheet">

    <!-- Custom Fonts -->


    <!-- HTML5 Shim and Respond.js IE8 support of HTML5 elements and media queries -->
    <!-- WARNING: Respond.js doesn't work if you view the page via file:// -->
    <!--[if lt IE 9]>
        <script src="https://oss.maxcdn.com/libs/html5shiv/3.7.0/html5shiv.js"></script>
        <script src="https://oss.maxcdn.com/libs/respond.js/1.4.2/respond.min.js"></script>
    <![endif]-->
	<link rel="shortcut icon" type="image/x-icon" href="img/favicon.ico" />

    <script type="text/javascript" src="https://unpkg.com/sweetalert/dist/sweetalert.min.js"></script>
</head>

<body class="logueo">
   
    <div>
        <div class="row">
            <img class="img-responsive center-block" src="img/LOGO.png" style="padding-top: 150px;">
        </div>
        <br>

        <!--div pagos-->
        <div class="col-lg-12">
            <img class="img-responsive center-block " style="" src="img/pagos2.png">
        </div>

        <div class="container">            
            <form runat="server">
                <asp:Panel ID="Panel1" runat="server">
                    <asp:Login ID="Login1" runat="server" OnAuthenticate="LogAcceso_Authenticate" DestinationPageUrl="~/Index.aspx">
                        <LayoutTemplate>
                            <!--login-->
                            <div class="top-content">
                                <div class="inner-bg">
                                    <div class="container">
                                        <div class="row">
                                            <div class="col-sm-6 col-sm-offset-3 form-box">
                                                <div class="form-bottom">
                                                    <div class="row">
                                                        <asp:TextBox ID="UserName" runat="server" CssClass="form-control" Style="border-radius: 25px; height: 45px;" Placeholder="Nombre de usuario" Width="100%"></asp:TextBox>
                                                    </div>
                                                    <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName" ErrorMessage="El nombre de usuario es obligatorio." ToolTip="El nombre de usuario es obligatorio." ValidationGroup="Login1" Style="color:#ffffff">El nombre de usuario es obligatorio.</asp:RequiredFieldValidator>

                                                    <br />

                                                    <div class="row"  >                                                        
                                                        <asp:TextBox ID="Password" runat="server" visible="false" TextMode="Password" CssClass="form-password form-control" Style="border-radius: 25px;  height: 45px;" Placeholder="Código" Width="100%"></asp:TextBox>
                                                    </div>
                                                    <asp  runat="server" ControlToValidate="Password"   ValidationGroup="Login1" Style="color:#ffffff"></asp>

                                                    <br>

                                                    <div>
                                                        <asp:Button ID="LoginButton" runat="server" CommandName="Login" CssClass="botonimagen center-block" ImageUrl="img/ingresar.png" ValidationGroup="Login1" Style="border-radius: 25px; height: 40px; width: 220px;" />
                                                    </div>

                                                    <br />

                                                    <asp:LinkButton ID="LinkButton3" runat="server" CssClass="btn center-block" OnClick="LinkButton3_Click">¿Olvidó su contraseña?</asp:LinkButton>
                                                    <div>
                                                        <br>
                                                        <a href="manuales/instructivo.pdf">
                                                            <img src="img/instructivo.png" class="img-responsive center-block"></a>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>                   
                        </LayoutTemplate>
                    </asp:Login>

                    <br />

                    <div class="alert alert-info">
                        <strong>Atención!</strong>
                        Si es primera vez que ingresa, debe colocar como usuario el NIT de la empresa a la que pertenece, incluyendo el dígito de verificación y una contraseña cualquiera, luego, se le solicitará registrar una dirección de correo electrónico y una nueva contraseña.
                    </div>

                    <div runat="server" class="alert alert-danger" id="div_error" visible="false">
                        <strong>Error!</strong>
                        <asp:Label ID="Label2" runat="server"></asp:Label>
                    </div>
                </asp:Panel>         
            </form>
        </div>
    </div>

    <br>
    <br>
    <br>
    <!--footer-->
    <div class="col-lg-12">
        <img class="img-responsive center-block "  src="img/pie2.png" style="width: 2000px">
    </div>
    <br>
    <br>

    <!-- jQuery -->
    <script src="js/jquery.js"></script>

    <!-- Bootstrap Core JavaScript -->
    <script src="js/bootstrap.min.js"></script>

    <!-- Plugin JavaScript -->
    <script src="http://cdnjs.cloudflare.com/ajax/libs/jquery-easing/1.3/jquery.easing.min.js"></script>
    <script src="js/classie.js"></script>
    <script src="js/cbpAnimatedHeader.js"></script>

    <!-- Custom Theme JavaScript -->
    <script src="js/agency.js"></script>

</body>

</html>

