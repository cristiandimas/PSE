using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Net;
using System.Net.Mail;

/// <summary>
/// Descripción breve de ManejaPagos
/// </summary>
public class ManejaPagos
{
    /// <summary>
    /// Correo que envia.
    /// </summary>
    private static string correo_envia = "info@biccrm.com";

    /// <summary>
    /// Nombre del correo que envia.
    /// </summary>
    private static string nombre_envia = "EC GROUP";

    /// <summary>
    /// Contraseña correo que envia.
    /// </summary>
    private static string servidor_envia = "smtp.office365.com";

    /// <summary>
    /// Servidor de correo que envia.
    /// </summary>
    private static string password_envia = "Colombia2020+";

    /// <summary>
    /// Tipo de correo a los cuales se les enviará el informe de pagos.
    /// </summary>
    private static string tipo_correo_pago = "PAGOS";

    /// <summary>
    /// 
    /// </summary>
    private static int cantidad_valores = 4;

    /// <summary>
    /// 
    /// </summary>
    private static int tamano_redondeo = 2;

    /// <summary>
    /// 
    /// </summary>
    public static string divisa_colombia = "COP";

    /// <summary>
    /// 
    /// </summary>
    public static string divisa_usd = "USD";

    /// <summary>
    /// 
    /// </summary>
    public static string estado_referencia_pagado = "PAGADO";

    /// <summary>
    /// 
    /// </summary>
    public static string estado_referencia_pendiente = "PENDIENTE";

    /// <summary>
    /// 
    /// </summary>
    public static string factura = "FC";

    /// <summary>
    /// 
    /// </summary>
    public static string recibo = "RC";

    /// <summary>
    /// 
    /// </summary>
    public static string debe = "D";

    /// <summary>
    /// 
    /// </summary>
    public static string haber = "H";

    /// <summary>
    /// 
    /// </summary>
    public static int posicion_total = 3;

    /// <summary>
    /// 
    /// </summary>
    public static int posicion_vencidas = 2;

    /// <summary>
    /// 
    /// </summary>
    public static int posicion_corrientes = 1;

    /// <summary>
    /// 
    /// </summary>
    public static int posicion_trm = 0;

    /// <summary>
    /// 
    /// </summary>
    public static string tipo_asiento = "R";

    /// <summary>
    /// 
    /// </summary>
    private EC_GROUPEntities dbContextHosting;

    /// <summary>
    /// 
    /// </summary>
    public ManejaPagos()
    {
        dbContextHosting = new EC_GROUPEntities();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="lista_facturas"></param>
    /// <returns></returns>
    public decimal CalcularValorTotalAPagar(List<Tuple<int, int>> lista_numero_facturas, string codigo_ec)
    {
        if (lista_numero_facturas == null)
        {
            return 0;
        }
        else if (lista_numero_facturas.Count == 0)
        {
            return 0;
        }
        else
        {
            decimal valor_a_pagar = 0;
            for (int i = 0; i < lista_numero_facturas.Count; i++)
            {
                var num_factura = lista_numero_facturas[i].Item1;
                var sucursal = lista_numero_facturas[i].Item2;

                var factura = dbContextHosting.forward_ctacte.Where(facturaT => facturaT.cta_numero == num_factura && facturaT.cta_sucoper == sucursal && facturaT.cta_debehaber == "D" && 
                                                                                facturaT.cta_saldo > 0 && facturaT.cta_comprobante == "FC" && facturaT.cta_cliente == codigo_ec).FirstOrDefault();

                valor_a_pagar += SaldoFacturaCOP(factura);
            }
            return decimal.Round(valor_a_pagar, 2);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="lista_numero_facturas"></param>
    /// <param name="lista_valores_facturas"></param>
    /// <param name="lista_divisas_local"></param>
    /// <param name="lista_divisas_otra"></param>
    /// <param name="lista_valores_ica"></param>
    /// <param name="usuario"></param>
    /// <param name="valor_total_referencia"></param>
    /// <returns></returns>
    public int GuardarRegistros(List<decimal> lista_numero_facturas, List<decimal> lista_valores_facturas, List<decimal> lista_divisas_local,
                                List<decimal> lista_divisas_otra, List<decimal> lista_valores_ica, List<decimal> lista_valores_reteiva, 
                                decimal usuario, decimal valor_total_referencia, List<int> lista_sucursales)
    {
        List<ESTADOSREFERENCIAS> lista_estados_referencia = dbContextHosting.ESTADOSREFERENCIAS.Where(estadoT => estadoT.ESTADOREFERENCIA == estado_referencia_pendiente).ToList();
        ESTADOSREFERENCIAS estado_referencia;

        if (lista_estados_referencia.Count > 0)
        {
            estado_referencia = lista_estados_referencia.First();

            List<MONEDAS> lista_monedas = dbContextHosting.MONEDAS.Where(monedaT => monedaT.MONEDA == divisa_colombia).ToList();

            if (lista_monedas.Count > 0)
            {
                MONEDAS moneda_pago = lista_monedas.First();

                REFERENCIASPAGOS referencia = new REFERENCIASPAGOS()
                {
                    IDUSUARIOWEB = usuario,
                    IDESTADOREFERENCIA = estado_referencia.IDESTADOREFERENCIA,
                    IDMONEDA = moneda_pago.IDMONEDA,
                    VALORREFERENCIA = valor_total_referencia,
                    FECHACREACION = DateTime.Now
                };

                dbContextHosting.REFERENCIASPAGOS.Add(referencia);

                for (int i = 0; i < lista_numero_facturas.Count; i++)
                {
                    RELACIONDOCUMENTOS relacion_factura = new RELACIONDOCUMENTOS()
                    {
                        NUMERODOCUMENTO = lista_numero_facturas[i].ToString(),
                        VALORPAGO = lista_valores_facturas[i],
                        ICA = lista_valores_ica[i],
                        RETEIVA = lista_valores_reteiva[i],
                        COTDIVISALOCAL = lista_divisas_local[i],
                        COTDIVISAOTRA = lista_divisas_otra[i],
                        FECHACREACION = DateTime.Now,
                        IDREFERENCIAPAGO = referencia.IDREFERENCIAPAGO,
                        IDTIPODOCUMENTO = 1,
                        IDTIPORELACIONDOCUMENTO = 1,
                        DETALLE = "FACTURA",
                        SUCURSAL = lista_sucursales[i]
                    };

                    dbContextHosting.RELACIONDOCUMENTOS.Add(relacion_factura);
                }

                dbContextHosting.SaveChanges();
                return Convert.ToInt32(referencia.IDREFERENCIAPAGO);
            }
            else
            {
                return -1;
            }            
        }
        else
        {
            return -1;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="usuario"></param>
    /// <param name="valor_total_referencia"></param>
    /// <param name="numeroDocumento"></param>
    /// <param name="detalle"></param>
    /// <returns></returns>
    public int GuardarRegistroAnticipos(decimal usuario, decimal valor_total_referencia, string numeroDocumento, string detalle)
    {
        List<ESTADOSREFERENCIAS> lista_estados_referencia = dbContextHosting.ESTADOSREFERENCIAS.Where(estadoT => estadoT.ESTADOREFERENCIA == estado_referencia_pendiente).ToList();
        ESTADOSREFERENCIAS estado_referencia;

        if (lista_estados_referencia.Count > 0)
        {
            estado_referencia = lista_estados_referencia.First();

            List<MONEDAS> lista_monedas = dbContextHosting.MONEDAS.Where(monedaT => monedaT.MONEDA == divisa_colombia).ToList();

            if (lista_monedas.Count > 0)
            {
                MONEDAS moneda_pago = lista_monedas.First();

                REFERENCIASPAGOS referencia = new REFERENCIASPAGOS()
                {
                    IDUSUARIOWEB = usuario,
                    IDESTADOREFERENCIA = estado_referencia.IDESTADOREFERENCIA,
                    IDMONEDA = moneda_pago.IDMONEDA,
                    VALORREFERENCIA = valor_total_referencia,
                    FECHACREACION = DateTime.Now
                };

                dbContextHosting.REFERENCIASPAGOS.Add(referencia);

                RELACIONDOCUMENTOS relacion_factura = new RELACIONDOCUMENTOS()
                {
                    NUMERODOCUMENTO = numeroDocumento,
                    VALORPAGO = valor_total_referencia,
                    ICA = 0,
                    RETEIVA = 0,
                    COTDIVISALOCAL = 1,
                    COTDIVISAOTRA = Convert.ToDecimal(CalcularCambioDivisas(divisa_usd, divisa_colombia).VALOR),
                    FECHACREACION = DateTime.Now,
                    IDREFERENCIAPAGO = referencia.IDREFERENCIAPAGO,
                    IDTIPODOCUMENTO = 2,
                    IDTIPORELACIONDOCUMENTO = 2,
                    DETALLE = detalle
                };

                dbContextHosting.RELACIONDOCUMENTOS.Add(relacion_factura);

                dbContextHosting.SaveChanges();
                return Convert.ToInt32(referencia.IDREFERENCIAPAGO);
            }
            else
            {
                return -1;
            }
        }
        else
        {
            return -1;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="codigoReferencia"></param>
    /// <param name="valorreferencia"></param>
    /// <param name="numeroTransaccion"></param>
    /// <param name="metodoPago"></param>
    /// <returns></returns>
    public bool RegistrarPagos(int codigoReferencia, decimal valorreferencia, string numeroTransaccion, string metodoPago)
    {
        List<REFERENCIASPAGOS> lista_referencias = dbContextHosting.REFERENCIASPAGOS.Where(ReferenciasT => ReferenciasT.IDREFERENCIAPAGO == codigoReferencia).ToList();
        REFERENCIASPAGOS referenciaAPagar;

        if (lista_referencias.Count > 0)
        {
            referenciaAPagar = lista_referencias.First();

            if (!referenciaAPagar.ESTADOSREFERENCIAS.ESTADOREFERENCIA.Equals(estado_referencia_pagado))
            {
                List<ESTADOSREFERENCIAS> lista_estados_referencia = dbContextHosting.ESTADOSREFERENCIAS.Where(estadoT => estadoT.ESTADOREFERENCIA == estado_referencia_pagado).ToList();
                ESTADOSREFERENCIAS estado_referencia;

                if (lista_estados_referencia.Count > 0)
                {
                    estado_referencia = lista_estados_referencia.First();
                    referenciaAPagar.ESTADOSREFERENCIAS = estado_referencia;
                    referenciaAPagar.FECHAPAGO = DateTime.Now;
                    referenciaAPagar.VALORREPORTADOTC = valorreferencia;
                    referenciaAPagar.NUMEROTRANSACCIONTC = numeroTransaccion;
                    referenciaAPagar.METODOPAGO = metodoPago;

                    //Actualizar ctacte
                    List<RELACIONDOCUMENTOS> lista_facturas_pagas = dbContextHosting.RELACIONDOCUMENTOS.Where(relacionT => relacionT.IDREFERENCIAPAGO == referenciaAPagar.IDREFERENCIAPAGO).ToList();

                    if (lista_facturas_pagas.Count > 0)
                    {
                        for (int i = 0; i < lista_facturas_pagas.Count; i++)
                        {
                            decimal cta_numero_T = Convert.ToDecimal(lista_facturas_pagas[i].NUMERODOCUMENTO);

                            List<forward_ctacte> lista_ctacte = dbContextHosting.forward_ctacte.Where(predicate: x => x.cta_numero == cta_numero_T && x.cta_saldo > 0 && x.cta_comprobante == factura).ToList();

                            for (int j = 0; j < lista_ctacte.Count; j++)
                            {
                                lista_ctacte[j].cta_saldo = 0;
                            }
                        }

                        //ManejaUsuarios manejaUsuarios = new ManejaUsuarios();
                        //manejaUsuarios.MandarCorreoPagos(referenciaAPagar.USUARIOSWEB, ListarFacturasReferencia(referenciaAPagar.IDREFERENCIAPAGO), referenciaAPagar.IDREFERENCIAPAGO);
                        dbContextHosting.SaveChanges();
                    }
                    else
                    {
                        return false;
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="factura"></param>
    /// <returns></returns>
    public decimal ValorFacturaCOP(v_forward_facturas factura)
    {
        if (factura.div_codigo.Equals(divisa_colombia))
        {
            return decimal.Round(Convert.ToDecimal(factura.fac_total), 2);
        }
        else
        {
            CAMBIOSDIVISAS divisa = CalcularCambioDivisas(factura.div_codigo, divisa_colombia);

            if (factura.cot_divlocal >= (decimal?)divisa.VALOR)
            {
                return decimal.Round(Convert.ToDecimal(factura.fac_total * factura.cot_divlocal), 2);
            }
            else
            {
                return decimal.Round(Convert.ToDecimal(factura.fac_total * (decimal?)divisa.VALOR), 2);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="factura"></param>
    /// <returns></returns>
    public decimal ValorFacturaUSD(v_forward_facturas factura)
    {
        if (factura.div_codigo.Equals(divisa_usd))
        {
            return decimal.Round(Convert.ToDecimal(factura.fac_total), 2);
        }
        else
        {
            CAMBIOSDIVISAS divisa = CalcularCambioDivisas(factura.div_codigo, divisa_usd);

            if (factura.cot_divotra >= (decimal?)divisa.VALOR)
            {
                return decimal.Round(Convert.ToDecimal(factura.fac_total * factura.cot_divotra), 2);
            }
            else
            {
                return decimal.Round(Convert.ToDecimal(factura.fac_total * (decimal?)divisa.VALOR), 2);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ctacte"></param>
    /// <returns></returns>
    public decimal SaldoFacturaCOP(forward_ctacte ctacte)
    {
        if (ctacte.div_codigo.Equals(divisa_colombia))
        {
            return decimal.Round(Convert.ToDecimal(ctacte.cta_saldo), 2);
        }
        else
        {
            CAMBIOSDIVISAS divisa = CalcularCambioDivisas(ctacte.div_codigo, divisa_colombia);

            if (ctacte.cot_divlocal >= (decimal?)divisa.VALOR)
            {
                return decimal.Round(Convert.ToDecimal(ctacte.cta_saldo * ctacte.cot_divlocal), 2);
            }
            else
            {
                return decimal.Round(Convert.ToDecimal(ctacte.cta_saldo * (decimal?)divisa.VALOR), 2);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ctacte"></param>
    /// <returns></returns>
    public decimal SaldoFacturaUSD(forward_ctacte ctacte)
    {
        if (ctacte.div_codigo.Equals(divisa_usd))
        {
            return decimal.Round(Convert.ToDecimal(ctacte.cta_saldo), 2);
        }
        else
        {
            CAMBIOSDIVISAS divisa = CalcularCambioDivisas(ctacte.div_codigo, divisa_usd);

            if (ctacte.cot_divotra >= (decimal?)divisa.VALOR)
            {
                return decimal.Round(Convert.ToDecimal(ctacte.cta_saldo * ctacte.cot_divotra), 2);
            }
            else
            {
                return decimal.Round(Convert.ToDecimal(ctacte.cta_saldo * (decimal?)divisa.VALOR), 2);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="divisa_inicial"></param>
    /// <param name="divisa_final"></param>
    /// <returns></returns>
    public CAMBIOSDIVISAS CalcularCambioDivisas(string divisa_inicial, string divisa_final)
    {
        List<CAMBIOSDIVISAS> list_valor_divisas = dbContextHosting.CAMBIOSDIVISAS.Where(divisa => divisa.MONEDAORIGEN == divisa_inicial && divisa.MONEDADESTINO == divisa_final && divisa.FECHA == DateTime.Today).ToList();

        if (list_valor_divisas.Count > 0)
        {
            return list_valor_divisas.First();
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="numFactura"></param>
    /// <returns></returns>
    public decimal SaldoFacturaCOP(decimal numFactura, int sucursal, string codigo_ec)
    {
        var list_cte = dbContextHosting.forward_ctacte.Where(ctacteT => ctacteT.cta_numero == numFactura && ctacteT.cta_debehaber == "D" && ctacteT.cta_comprobante == "FC" && 
                                                                        ctacteT.cta_cbteaplica == "FC" && ctacteT.cta_cliente == codigo_ec && ctacteT.cta_sucoper == sucursal);
        forward_ctacte ctacte = new forward_ctacte();

        if (list_cte.AsEnumerable().Count() > 0)
        {
            ctacte = list_cte.AsEnumerable().First();
            return SaldoFacturaCOP(ctacte);
        }
        else
        {
            return -1;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ctacte"></param>
    /// <returns></returns>
    public decimal DivisaUSD(forward_ctacte ctacte)
    {
        if (ctacte.div_codigo.Equals(divisa_usd))
        {
            CAMBIOSDIVISAS divisa = CalcularCambioDivisas(divisa_usd, divisa_colombia);

            if (ctacte.cot_divlocal >= (decimal?)divisa.VALOR)
            {
                return decimal.Round(Convert.ToDecimal(1 / ctacte.cot_divlocal), 6);
            }
            else
            {
                return decimal.Round(Convert.ToDecimal(1 / divisa.VALOR), 6);
            }
        }
        else
        {
            CAMBIOSDIVISAS divisa = CalcularCambioDivisas(ctacte.div_codigo, divisa_usd);

            if (ctacte.cot_divotra >= (decimal?)divisa.VALOR)
            {
                return Convert.ToDecimal(ctacte.cot_divotra);
            }
            else
            {
                return Convert.ToDecimal(divisa.VALOR);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="numFactura"></param>
    /// <returns></returns>
    public decimal DivisaUSD(decimal numFactura, int sucursal, string codigo_ec)
    {
        var list_cte = dbContextHosting.forward_ctacte.Where(ctacteT => ctacteT.cta_numero == numFactura && ctacteT.cta_debehaber == "D" && ctacteT.cta_comprobante == "FC" && 
                                                                        ctacteT.cta_cbteaplica == "FC" && ctacteT.cta_cliente == codigo_ec && ctacteT.cta_sucoper == sucursal);
        forward_ctacte ctacte = new forward_ctacte();

        if (list_cte.AsEnumerable().Count() > 0)
        {
            ctacte = list_cte.AsEnumerable().First();
            return DivisaUSD(ctacte);
        }
        else
        {
            return -1;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cliente"></param>
    /// <returns></returns>
    public List<decimal> CalcularSaldosCliente(string cliente)
    {
        try
        {
            List<decimal> lista_saldos = new List<decimal>();

            List<forward_ctacte> lista_facturas_todas = dbContextHosting.forward_ctacte.Where(ctacteT => ctacteT.cta_cliente == cliente && ctacteT.cta_debehaber == debe
                                                && ctacteT.cta_comprobante == factura && ctacteT.cta_cbteaplica == factura && ctacteT.cta_saldo > 0).ToList();

            List<forward_ctacte> lista_facturas_venci = dbContextHosting.forward_ctacte.Where(ctacteT => ctacteT.cta_cliente == cliente && ctacteT.cta_debehaber == debe
                                                && ctacteT.cta_comprobante == factura && ctacteT.cta_cbteaplica == factura && ctacteT.cta_saldo > 0
                                                && SqlFunctions.DateDiff("day", ctacteT.cta_fecvto, DateTime.Today) > 0).ToList();

            List<forward_ctacte> lista_facturas_corri = dbContextHosting.forward_ctacte.Where(ctacteT => ctacteT.cta_cliente == cliente && ctacteT.cta_debehaber == debe
                                                && ctacteT.cta_comprobante == factura && ctacteT.cta_cbteaplica == factura && ctacteT.cta_saldo > 0
                                                && SqlFunctions.DateDiff("day", ctacteT.cta_fecvto, DateTime.Today) <= 0).ToList();

            for (int i = 0; i < cantidad_valores; i++)
            {
                lista_saldos.Add(0);
            }

            lista_saldos[posicion_trm] = Convert.ToDecimal(CalcularCambioDivisas(divisa_usd, divisa_colombia).VALOR);

            for (int i = 0; i < lista_facturas_todas.Count; i++)
            {
                lista_saldos[posicion_total] += SaldoFacturaCOP(lista_facturas_todas[i]);
            }

            for (int i = 0; i < lista_facturas_corri.Count; i++)
            {
                lista_saldos[posicion_corrientes] += SaldoFacturaCOP(lista_facturas_corri[i]);
            }

            for (int i = 0; i < lista_facturas_venci.Count; i++)
            {
                lista_saldos[posicion_vencidas] += SaldoFacturaCOP(lista_facturas_venci[i]);
            }

            return lista_saldos;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="factura"></param>
    /// <param name="ica"></param>
    /// <returns></returns>
    public decimal CalcularICAFactura(decimal factura, decimal ica)
    {
        List<v_forward_factitem> lista_items_factura = dbContextHosting.v_forward_factitem.Where(factitemT => factitemT.fac_numero == factura).ToList();
        List<INGRESOSPROPIOS> lista_ingresos_propios = dbContextHosting.INGRESOSPROPIOS.ToList();

        decimal valor_ica = 0;

        for (int i = 0; i < lista_items_factura.Count; i++)
        {
            for (int j = 0; j < lista_ingresos_propios.Count; j++)
            {
                if (lista_items_factura[i].gas_codigo == lista_ingresos_propios[j].CODIGO)
                {
                    if (ObtenerDivisaFactura(lista_items_factura[i].fac_tipo, lista_items_factura[i].fac_sucursal, lista_items_factura[i].fac_numero, lista_items_factura[i].fac_comprobante) == divisa_colombia)
                    {
                        valor_ica += decimal.Round(Convert.ToDecimal(lista_items_factura[i].fac_impneto * ica), tamano_redondeo);
                    }
                    else
                    {
                        valor_ica += decimal.Round(Convert.ToDecimal(lista_items_factura[i].fac_impneto * DivisaFactura(lista_items_factura[i].fac_numero) * ica), tamano_redondeo);
                    }
                }
            }
        }

        return valor_ica;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="factura"></param>
    /// <param name="ica"></param>
    /// <returns></returns>
    public decimal CalcularRetIVAFactura(decimal factura, decimal retIVA)
    {
        List<v_forward_factitem> lista_items_factura = dbContextHosting.v_forward_factitem.Where(factitemT => factitemT.fac_numero == factura).ToList();
        List<INGRESOSPROPIOS> lista_ingresos_propios = dbContextHosting.INGRESOSPROPIOS.ToList();

        decimal valor_ret_iva = 0;

        for (int i = 0; i < lista_items_factura.Count; i++)
        {
            for (int j = 0; j < lista_ingresos_propios.Count; j++)
            {
                if (lista_items_factura[i].gas_codigo == lista_ingresos_propios[j].CODIGO)
                {
                    if (ObtenerDivisaFactura(lista_items_factura[i].fac_tipo, lista_items_factura[i].fac_sucursal, lista_items_factura[i].fac_numero, lista_items_factura[i].fac_comprobante) == divisa_colombia)
                    {
                        valor_ret_iva += decimal.Round(Convert.ToDecimal((lista_items_factura[i].fac_imptotal - lista_items_factura[i].fac_impneto) * retIVA), tamano_redondeo);
                    }
                    else
                    {
                        decimal divisa_factura = DivisaFactura(lista_items_factura[i].fac_numero);
                        valor_ret_iva += decimal.Round(Convert.ToDecimal(((lista_items_factura[i].fac_imptotal - lista_items_factura[i].fac_impneto) * divisa_factura) * retIVA), tamano_redondeo);
                    }
                }
            }
        }

        return valor_ret_iva;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ctacte"></param>
    /// <returns></returns>
    public decimal DivisaFactura(forward_ctacte ctacte)
    {
        if (ctacte.div_codigo.Equals(divisa_colombia))
        {
            return decimal.Round(Convert.ToDecimal(ctacte.cot_divlocal), tamano_redondeo);
        }
        else
        {
            CAMBIOSDIVISAS divisa = CalcularCambioDivisas(ctacte.div_codigo, divisa_colombia);

            if (ctacte.cot_divlocal >= (decimal?)divisa.VALOR)
            {
                return decimal.Round(Convert.ToDecimal(ctacte.cot_divlocal), tamano_redondeo);
            }
            else
            {
                return decimal.Round(Convert.ToDecimal(divisa.VALOR), tamano_redondeo);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="numFactura"></param>
    /// <returns></returns>
    public decimal DivisaFactura(decimal numFactura)
    {
        var list_cte = dbContextHosting.forward_ctacte.Where(ctacteT => ctacteT.cta_numero == numFactura && ctacteT.cta_debehaber == "D" && ctacteT.cta_comprobante == "FC" && ctacteT.cta_cbteaplica == "FC");
        forward_ctacte ctacte = new forward_ctacte();

        if (list_cte.AsEnumerable().Count() > 0)
        {
            ctacte = list_cte.AsEnumerable().First();
            return DivisaFactura(ctacte);
        }
        else
        {
            return -1;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="referencia"></param>
    /// <returns></returns>
    public string ListarFacturasReferencia(decimal referencia)
    {
        List<RELACIONDOCUMENTOS> listaDocumentos = dbContextHosting.RELACIONDOCUMENTOS.Where(relacionT => relacionT.IDREFERENCIAPAGO == referencia).ToList();

        string listaFacturas = "";

        for (int i = 0; i < listaDocumentos.Count; i++)
        {
            if (i < (listaDocumentos.Count - 1))
            {
                listaFacturas += listaDocumentos[i].NUMERODOCUMENTO.ToString() + ", ";
            }
            else
            {
                listaFacturas += listaDocumentos[i].NUMERODOCUMENTO.ToString();
            }
        }

        return listaFacturas;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="facTipo"></param>
    /// <param name="facSucursal"></param>
    /// <param name="facNumero"></param>
    /// <param name="facComprobante"></param>
    /// <returns></returns>
    public string ObtenerDivisaFactura(string facTipo, decimal facSucursal, decimal facNumero, string facComprobante)
    {
        List<v_forward_facturas> lista_factura = dbContextHosting.v_forward_facturas.Where(x => x.fac_tipo == facTipo && x.fac_sucursal == facSucursal && x.fac_numero == facNumero && x.fac_comprobante == facComprobante).ToList();

        if (lista_factura.Count > 0)
        {
            return lista_factura[0].div_codigo;
        }

        return "";
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
    /// <param name="codigoReferencia"></param>
    public void MandarCorreoAdministrativo(int codigoReferencia)
    {
        List<CORREOS> listaCorreos = dbContextHosting.CORREOS.Where(predicate: correos => correos.TIPOCORREOS.TIPOCORREO == tipo_correo_pago).ToList();

        if (listaCorreos.Count > 0)
        {
            MailAddress origen = new MailAddress(correo_envia, nombre_envia);

            MailMessage Email = new MailMessage
            {
                From = origen,
                Sender = origen
            };

            for (int i = 0; i < listaCorreos.Count; i++)
            {
                MailAddress destino = new MailAddress(listaCorreos[i].CORREO);
                Email.To.Add(destino);
            }

            Email.Subject = "EC CARGO: REPORTE DE PAGOS REF. " + codigoReferencia.ToString();
            Email.IsBodyHtml = true;
            Email.Body = "";
            Email.Priority = MailPriority.High;

            Attachment attachment = new Attachment((new ManejaExcel().GenerarArchivoPagos(codigoReferencia)), "PAGOS REF-" + codigoReferencia + ".xlsx", "application/vnd.ms-excel");

            Email.Attachments.Add(attachment);

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
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="factura"></param>
    /// <param name="sucursal"></param>
    /// <returns></returns>
    public string ObtenerCUFE(int factura, int sucursal)
    {
        var fact = dbContextHosting.fed_facturas_enviadas.Where(x => x.fed_num_factura == factura && x.fed_sucursal_id == sucursal).OrderByDescending(x => x.fed_fecha_registro).FirstOrDefault();
        if (fact != null) return fact.fed_factura_uuid;
        else return string.Empty;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="nombre"></param>
    /// <returns></returns>
    public List<string> ObtenerListaClientes(string nombre)
    {
        return dbContextHosting.forward_clientes.Where(x => x.cli_descripcion.Contains(nombre)).Select(x => x.cli_descripcion).ToList(); 
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="nombre"></param>
    /// <returns></returns>
    public forward_clientes ObtenerCliente(string nombre)
    {
        return dbContextHosting.forward_clientes.Where(x => x.cli_descripcion.Contains(nombre)).FirstOrDefault();
    }
}