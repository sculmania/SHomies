﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SHomies.Utilitario;

namespace SHomies.Tienda.Venta
{
    /// <summary>
    /// Interaction logic for DatosOrden.xaml
    /// </summary>
    public partial class DatosOrden : Window
    {
        private EEstadoFormulario estadoFormulario;
        private Conexion.IConexion conexion;
        private Core.Venta.Orden orden;

        public DatosOrden(Core.Venta.Orden iOrden, Conexion.IConexion iConexion)
        {
            this.conexion = iConexion;
            this.orden = iOrden;
            InitializeComponent();
        }
        public DatosOrden()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.estadoFormulario = EEstadoFormulario.Load;
            try
            {
                this.txbTituloDelFormulario.Text = "Datos de la Orden";
                orden.GetDatosOrden();

                this.txbNroOrden.Text = this.orden.Id.ToString();
                this.txbFechaSistema.Text = this.orden.AuditoriaSistema.FechaSistema.ToShortDateString();
                this.txbEstado.Text = this.orden.Estado.Codigo;
                this.txbTotal.Text = Funcion.FormatoDecimal(this.orden.DetalleProducto.Sum(x => x.Producto.PrecioVenta));
                this.txbComision.Text = Funcion.FormatoDecimal(this.orden.Fichadoras.Sum(x => x.Monto));
                this.btnImprimir.IsEnabled = true;
                this.DetalleOrden();
                this.DistribulleComision();
                this.estadoFormulario = EEstadoFormulario.EndLoad;
            }
            catch (Exception ex)
            {
                this.estadoFormulario = EEstadoFormulario.ErrorLoad;
                MessageBox.Show(ex.Message);
            }
            Clases.FuncionFormulario.ValidaCargaFormulario(estadoFormulario, this);
        }

        private void DetalleOrden()
        {
            try
            {
                List<Clases.DetalleOrdenViewModel> detalle =
                    new List<Clases.DetalleOrdenViewModel>();

                foreach (Core.Venta.DetalleOrden item in this.orden.DetalleProducto)
                {
                    detalle.Add(
                        new Clases.DetalleOrdenViewModel(this.conexion)
                        {
                            DescripcionProducto = item.Producto.Descripcion,
                            PrecioProducto = Funcion.FormatoDecimal(item.Producto.PrecioVenta),
                            Cantidad = item.Cantidad,
                            Imagen = item.Producto.Imagen
                        }
                        );
                }

                this.dtgDetalleVenta.ItemsSource = detalle.OrderBy(x => x.DescripcionProducto);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DistribulleComision()
        {
            try
            {
                decimal totalComision = this.orden.DetalleProducto.Sum(x => x.Producto.Comision * x.Cantidad);
                decimal montoFichaje = totalComision / (this.orden.Fichadoras.Count == 0 ? 1 : this.orden.Fichadoras.Count);

                List<Clases.DetalleFichajeViewModel> detalle =
                    new List<Clases.DetalleFichajeViewModel>();

                foreach (Core.Venta.Fichaje item in this.orden.Fichadoras)
                {
                    item.Monto = montoFichaje;
                    detalle.Add(
                        new Clases.DetalleFichajeViewModel(this.conexion)
                        {
                            NombreFichadora = item.Fichadora.Nombres,
                            MontoFichaje = Funcion.FormatoDecimal(item.Monto)
                        }
                        );
                }

                this.dtgFichadoras.ItemsSource = detalle.OrderBy(x => x.NombreFichadora);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnCerrar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnImprimir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ImprimirBoleto();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ImprimirBoleto()
        {
            Ticket ticket = new Ticket();
            ticket.Title = "EL RELAX";
            ticket.AddCabecera("TICKET", Alineacion.Left, Alineacion.Left, 10, this.orden.Id);
            ticket.AddCabecera("CAJERO", Alineacion.Left, Alineacion.Left, 10, this.orden.AuditoriaSistema.Usuario.UserName);
            ticket.AddCabecera("FECHA", Alineacion.Left, Alineacion.Left, 10, DateTime.Now);
            ticket.AddCebeceraDetalle("Cant.", Alineacion.Left, Alineacion.Left, 6);
            ticket.AddCebeceraDetalle("Descripción", Alineacion.Left, Alineacion.Left, 20);
            ticket.AddCebeceraDetalle("Importe", Alineacion.Right, Alineacion.Right, 9);

            foreach (Core.Venta.DetalleOrden detalle in this.orden.DetalleProducto)
            {
                ticket.AddItemsDetails(detalle.Cantidad, detalle.Producto.Descripcion, Funcion.FormatoDecimal(detalle.Total));
            }
            ticket.AddTotal("Total Venta", Alineacion.Right, Alineacion.Right, 25, Funcion.FormatoDecimal(this.orden.DetalleProducto.Sum(o => o.Total)));

            foreach (Core.Venta.Fichaje fichaje in this.orden.Fichadoras)
            {
                ticket.AddTotal(fichaje.Fichadora.Nombres, Alineacion.Right, Alineacion.Right, 25, Funcion.FormatoDecimal(fichaje.Monto));
            }
            ticket.itemsPie.Add("Los esperamos");
            String impresora = System.Configuration.ConfigurationSettings.AppSettings["Impresora"].ToString();

            ticket.Imprimir(impresora);
        }

    }
}
