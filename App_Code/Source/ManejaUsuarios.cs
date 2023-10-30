using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Security;

/// <summary>
/// Descripción breve de ManejaUsuarios
/// </summary>
public class ManejaUsuarios
{
    /// <summary>
    /// Contexto de conexión a base de datos EC_Grop Hosting.
    /// </summary>
    private EC_GROUPEntities dbContextHosting;

    /// <summary>
    /// Correo que envia.
    /// </summary>
    private static string correo_envia = "cartera2@eccargosa.com";

    /// <summary>
    /// Nombre del correo que envia.
    /// </summary>
    private static string nombre_envia = "EC GROUP";

    /// <summary>
    /// Contraseña correo que envia.
    /// </summary>
    private static string servidor_envia = "smtp.gmail.com";

    /// <summary>
    /// Servidor de correo que envia.
    /// </summary>
    private static string password_envia = "Colombia2020+";

    /// <summary>
    /// Opcion de correo que se envía.
    /// </summary>
    private static int correo_opcion_nuevo = 1;

    /// <summary>
    /// Opcion de correo que se envía.
    /// </summary>
    private static int correo_opcion_recupera = 2;

    /// <summary>
    /// Tipo de correo a los cuales se les enviará el informe de pagos.
    /// </summary>
    private static string tipo_correo_pago = "PAGOS";

    /// <summary>
    /// 
    /// </summary>
    public static int correo_password_send_success = 1;

    /// <summary>
    /// 
    /// </summary>
    public static int correo_password_send_error = 2;

    /// <summary>
    /// 
    /// </summary>
    public static int correo_password_send_correo_no_existe = 3;

    /// <summary>
    /// 
    /// </summary>
    public static int numero_intentos = 3;

    /// <summary>
    /// Constructor de la clase.
    /// </summary>
    public ManejaUsuarios()
    {
        dbContextHosting = new EC_GROUPEntities();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="nit"></param>
    /// <returns></returns>
    public TERCEROS BuscarClienteBDHosting(string nit)
    {
        List<TERCEROS> lista_clientes = dbContextHosting.TERCEROS.
                                                         Where(terceroT => terceroT.DOCUMENTO + terceroT.DIGITOVERIFICACION == nit ||
                                                                           terceroT.DOCUMENTO == nit).ToList();

        if (lista_clientes.Count > 0)
        {
            return lista_clientes.First();
        }

        return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="login"></param>
    /// <returns></returns>
    public fed_usuarios BuscarAdmin(string login)
    {
        return dbContextHosting.fed_usuarios.Where(x => x.fed_usuario_login == login).FirstOrDefault();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="login"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public fed_usuarios BuscarAdminPassword(string login)
    {
       
        return dbContextHosting.fed_usuarios.Where(x => x.fed_usuario_login == login).FirstOrDefault();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="nit"></param>
    /// <returns></returns>
    public USUARIOSWEB BuscarUsuarioBDHosting(string nit)
    {
        List<USUARIOSWEB> lista_usuarios = dbContextHosting.USUARIOSWEB.
                                                            Where(usuarioT => usuarioT.TERCEROS.DOCUMENTO + usuarioT.TERCEROS.DIGITOVERIFICACION == nit || 
                                                                              usuarioT.TERCEROS.DOCUMENTO == nit).ToList();

        if (lista_usuarios.Count > 0)
        {
            return lista_usuarios.FirstOrDefault();
        }

        return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="nit"></param>
    
    /// <returns></returns>
    public USUARIOSWEB BuscarUsuarioYPasswordBDHosting(string nit)
    {
        //string passT = CalculateMD5Hash(password);
        List<USUARIOSWEB> lista_usuarios = dbContextHosting.USUARIOSWEB.
                                                            Where(usuarioT => (usuarioT.TERCEROS.DOCUMENTO + usuarioT.TERCEROS.DIGITOVERIFICACION == nit) ||
                                                                              (usuarioT.TERCEROS.DOCUMENTO == nit )).ToList();

        if (lista_usuarios.Count > 0)
        {
            return lista_usuarios.First();
        }

        return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cliente"></param>
    /// <returns></returns>
    public TERCEROS IngresarClienteNoExistente(forward_clientes cliente)
    {
        string nitLimpio = LimpiarNIT(cliente.cli_cuit);

        TERCEROS tercero = new TERCEROS()
        {
            DOCUMENTO = cliente.cli_cuit.Contains("-") ? nitLimpio.Remove(nitLimpio.Length - 1) : nitLimpio,
            DIGITOVERIFICACION = cliente.cli_cuit.Contains("-") ? nitLimpio[nitLimpio.Length - 1].ToString() : "",
            RAZONSOCIAL = cliente.cli_descripcion.TrimEnd(),
            IDTIPOTERCERO = 7,
            TIPOTERCERO = "CLIENTE",
            DIRECCIONTERCERO = cliente.cli_domicilio1.TrimEnd(),
            TELEFONO = cliente.cli_telefono.TrimEnd(),
            EMAIL = cliente.cli_email.TrimEnd(),
            IDTIPOCONTRIBUYENTE = ObtenerIdTipoContribuyente(cliente),
            TIPOCONTRIBUYENTE = ObtenerTipoContribuyente(cliente),
            NOMBRESCONTACTO = cliente.cli_contacto1.TrimEnd(),
            DIASLIBRES = cliente.CLI_FREEDAYS.ToString(),
            TIENECREDITO = ObtenerCredito(cliente),
            DIASCREDITO = cliente.cli_creditdias.ToString(),
            FECHACREACION = DateTime.Now,
            USUARIOCREA = "WEB"
        };

        dbContextHosting.TERCEROS.Add(tercero);
        dbContextHosting.SaveChanges();

        return tercero;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cliente"></param>
    /// <returns></returns>
    public int ObtenerIdTipoContribuyente(forward_clientes cliente)
    {
        switch(Convert.ToInt32(cliente.iva_codigo))
        {
            case 1:
                return 1;
            case 2:
                return 2;
            case 3:
                return 5;
            case 4:
                return 3;
            default:
                return 1;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cliente"></param>
    /// <returns></returns>
    public string ObtenerTipoContribuyente(forward_clientes cliente)
    {
        switch (Convert.ToInt32(cliente.iva_codigo))
        {
            case 1:
                return "REGIMEN COMUN";
            case 2:
                return "REGIMEN SIMPLIFICADO";
            case 3:
                return "GRAN CONTRIBUYENTE";
            case 4:
                return "PERSONA NATURAL";
            default:
                return "REGIMEN COMUN";
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cliente"></param>
    /// <returns></returns>
    public string ObtenerCredito(forward_clientes cliente)
    {
        if (cliente.cli_creditmonto > 0)
            return "S";
        else
            return "N";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tercero"></param>
   
    /// <param name="codigoEC"></param>
    /// <param name="correo"></param>
    /// <returns></returns>
    public bool IngresarUsuarioWeb(decimal tercero,  string codigoEC, string correo)
    {
        try
        {
            //string passwordT = CalculateMD5Hash(password);

            USUARIOSWEB usuario = new USUARIOSWEB()
            {
                IDTERCERO = tercero,
                //PASSWORD = passwordT,
                CODIGO_ECCARGO = codigoEC,
                FECHACREACION = DateTime.Now,
                ESTADO = true,
                CORREO = correo,
                NUMEROINTENTOS = 0,
                IDTIPOUSUARIOWEB = 1
            };

            dbContextHosting.USUARIOSWEB.Add(usuario);
            dbContextHosting.SaveChanges();

            var cliente = dbContextHosting.forward_clientes.Where(x => x.cli_codigo == usuario.CODIGO_ECCARGO).FirstOrDefault();

            MandarCorreo(usuario, cliente.cli_cuit.Trim(), correo_opcion_nuevo);

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="nit"></param>
    /// <returns></returns>
    public forward_clientes BuscarClienteBD_ECCargo(string nit)
    {
        var dbContext = new EC_GROUPEntities();
        List<forward_clientes> lista_clientes = dbContext.forward_clientes.Where(clienteT => clienteT.cli_cuit.Trim().Contains(nit)).ToList();

        if (lista_clientes.Count > 0)
        {
            return lista_clientes.First();
        }

        return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="usuario"></param>
    public void ActualizarIntentosFallidos(USUARIOSWEB usuario)
    {
        usuario.NUMEROINTENTOS++;
        dbContextHosting.SaveChanges();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="admin"></param>
    public void ActualizarIntentosFallidosAdmin(fed_usuarios admin)
    {
        admin.fed_usuario_intentos++;
        dbContextHosting.SaveChanges();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="usuario"></param>
    public void ReiniciarIntentos(USUARIOSWEB usuario)
    {
        usuario.NUMEROINTENTOS =0;
        dbContextHosting.SaveChanges();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="admin"></param>
    public void ReiniciarIntentosAdmin(fed_usuarios admin)
    {
        admin.fed_usuario_intentos = 0;
        dbContextHosting.SaveChanges();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public int ActualizarPassword(string email)
    {
        try
        {
            List<USUARIOSWEB> lista_usuarios = dbContextHosting.USUARIOSWEB.Where(usuariowebT => usuariowebT.CORREO == email).ToList();

            if (lista_usuarios.Count > 0)
            {
                

                USUARIOSWEB usuario = lista_usuarios.FirstOrDefault();

                var cliente = dbContextHosting.forward_clientes.Where(x => x.cli_codigo == usuario.CODIGO_ECCARGO).FirstOrDefault();

                
                usuario.NUMEROINTENTOS = 0;
                usuario.FECHAULTIMAACTUALIZACION = DateTime.Now;
                dbContextHosting.SaveChanges();

                MandarCorreo(usuario, cliente.cli_cuit.Trim(),  correo_opcion_recupera);
                return correo_password_send_success;
            }
            else
            {
                return correo_password_send_correo_no_existe;
            }
        }
        catch
        {
            return correo_password_send_error;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void DesconectarBD()
    {
        dbContextHosting.Dispose();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="nit"></param>
    /// <returns></returns>
    public static string LimpiarNIT(string nit)
    {
        StringBuilder sb = new StringBuilder();
        nit = nit.Trim();
        foreach (char c in nit)
        {
            if (c >= '0' && c <= '9')
            {
                sb.Append(c);
            }
        }
        return sb.ToString();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string CalculateMD5Hash(string input)
    {
        // step 1, calculate MD5 hash from input
        MD5 md5 = System.Security.Cryptography.MD5.Create();
        byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
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
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool IsValidPassword(string input)
    {
        var hasNumber = new Regex(@"[0-9]+");
        var hasUpperChar = new Regex(@"[A-Z]+");
        var hasMinimum8Chars = new Regex(@".{8,12}");

        if(hasNumber.IsMatch(input) && hasUpperChar.IsMatch(input) && hasMinimum8Chars.IsMatch(input))
            return true;
        else
            return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="usuario"></param>
    /// <param name="password"></param>
    /// <param name="opcion"></param>
    public void MandarCorreo(USUARIOSWEB usuario, string cliente, int opcion)
    {
        try
        {
            MailAddress origen = new MailAddress(correo_envia, nombre_envia);
            MailAddress destino = new MailAddress(usuario.CORREO, usuario.CORREO);

            MailMessage Email = new MailMessage
            {
                From = origen,
                Sender = origen
            };
            Email.To.Add(destino);

            if (opcion.Equals(correo_opcion_nuevo))
            {
                Email.Subject = "EC GROUP: DATOS NUEVA CUENTA";
                Email.Body = "<b>Bienvenido!</b><br>" +
                    "<br>Estos son los datos nuevos de su cuenta:" +
                    "<br><b>Correo registrado:</b> " + usuario.CORREO +
                    "<br><b>Usuario:</b> " + cliente +                    
                    "<br><br>Cordialmente <br><br> <b>EC GROUP</b>";
            }
            else if (opcion.Equals(correo_opcion_recupera))
            {
                Email.Subject = "EC GROUP: RECUPERACIÓN CONTRASEÑA";
                Email.Body = "<b>Bienvenido!</b><br>" +
                    "<br>Los datos de acceso a su cuenta son:" +
                    "<br><b>Correo registrado:</b> " + usuario.CORREO +
                    "<br><b>Usuario:</b> " + cliente +                    
                    "<br><br>Cordialmente <br><br> <b>EC GROUP</b>";
            }

            Email.IsBodyHtml = true;
            Email.Priority = MailPriority.High;

            SmtpClient smtp = new SmtpClient
            {
                UseDefaultCredentials = false
            };
            NetworkCredential credencial = new NetworkCredential(correo_envia, password_envia);
            smtp.Credentials = credencial;
            smtp.Host = servidor_envia;
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.Send(Email);
        }
        catch(Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    public void MandarCorreoCode(USUARIOSWEB usuario, string codigoVerificacion)
    {
        try
        {
            MailAddress origen = new MailAddress(correo_envia, nombre_envia);
            MailAddress destino = new MailAddress(usuario.CORREO, usuario.CORREO);

            MailMessage Email = new MailMessage
            {
                From = origen,
                Sender = origen
            };
            Email.To.Add(destino);

            Email.Subject = "EC GROUP: Código de Verificación";
            Email.Body = "Bienvenido!<br>" +
                 "<br><p>Este es el código de verificación de su cuenta: </p>" +
                 "<br><b>Código de Verificación: </b>" + codigoVerificacion +
                 "<br><br>Cordialmente <br><br> <b>EC GROUP</b>";


            Email.IsBodyHtml = true;
            Email.Priority = MailPriority.High;

            SmtpClient smtp = new SmtpClient
            {
                UseDefaultCredentials = false
            };
            NetworkCredential credencial = new NetworkCredential(correo_envia, password_envia);
            smtp.Credentials = credencial;
            smtp.Host = servidor_envia;
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.Send(Email);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="usuario"></param>
    /// <param name="documentos"></param>
    /// <param name="referencia"></param>
    public void MandarCorreoPagos(USUARIOSWEB usuario, string documentos, decimal referencia)
    {
        try
        {
            MailAddress origen = new MailAddress(correo_envia, nombre_envia);
            MailAddress destino = new MailAddress(usuario.CORREO, usuario.CORREO);

            MailMessage Email = new MailMessage
            {
                From = origen,
                Sender = origen
            };
            Email.To.Add(destino);
            Email.Subject = "EC GROUP: RELACION PAGOS REFERENCIA: " + referencia.ToString();
            Email.Body = "Buen día!<br>" +
                "<br>A continuación se relacionan los números de documentos a los cuales se les realizó pagos:" + documentos +
                "<br><br>Cordialmente <br><br> <b>EC GROUP</b>";

            Email.IsBodyHtml = true;
            Email.Priority = MailPriority.High;

            SmtpClient smtp = new SmtpClient
            {
                UseDefaultCredentials = false
            };
            NetworkCredential credencial = new NetworkCredential(correo_envia, password_envia);
            smtp.Credentials = credencial;
            smtp.Host = servidor_envia;
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.Send(Email);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="correo"></param>
    /// <returns></returns>
    public bool ExisteCorreo(string correo)
    {
        if (dbContextHosting.USUARIOSWEB.Where(usuariowebT => usuariowebT.CORREO == correo).ToList().Count > 0 ||
            dbContextHosting.fed_usuarios.Where(x => x.fed_usuario_email == correo).ToList().Count > 0)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    
    /// <param name="email"></param>
    /// <returns></returns>
    //public int ActualizarPassword(string email)
    //{
    //    try
    //    {
    //        List<USUARIOSWEB> lista_usuarios = dbContextHosting.USUARIOSWEB.Where(usuariowebT => usuariowebT.CORREO == email).ToList();

    //        if (lista_usuarios.Count > 0)
    //        {
    //            USUARIOSWEB usuario = lista_usuarios.First();
                
    //            usuario.NUMEROINTENTOS = 0;
    //            usuario.FECHAULTIMAACTUALIZACION = DateTime.Now;
    //            dbContextHosting.SaveChanges();
    //            return correo_password_send_success;
    //        }
    //        else
    //        {
    //            return correo_password_send_correo_no_existe;
    //        }
    //    }
    //    catch
    //    {
    //        return correo_password_send_error;
    //    }
    //}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public int RestaurarUsuario(int idUsuario)
    {
        try
        {
            List<USUARIOSWEB> lista_usuarios = dbContextHosting.USUARIOSWEB.Where(usuariowebT => usuariowebT.IDUSUARIOWEB == idUsuario).ToList();

            if (lista_usuarios.Count > 0)
            {
                USUARIOSWEB usuario = lista_usuarios.First();
                //usuario.PASSWORD = CalculateMD5Hash(password);
                usuario.NUMEROINTENTOS = 0;
                usuario.FECHAULTIMAACTUALIZACION = DateTime.Now;
                dbContextHosting.SaveChanges();

                var cliente = dbContextHosting.forward_clientes.Where(x => x.cli_codigo == usuario.CODIGO_ECCARGO).FirstOrDefault();

                MandarCorreo(usuario, cliente.cli_cuit.Trim(), correo_opcion_recupera);
                return correo_password_send_success;
            }
            else
            {
                return correo_password_send_correo_no_existe;
            }
        }
        catch
        {
            return correo_password_send_error;
        }
    }
}