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
using SHomies.Conexion;

namespace SHomies.Tienda.Usuario
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private EEstadoFormulario estadoFormulario;
        public Login()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.estadoFormulario = EEstadoFormulario.Load;
            try
            {
                this.txbTituloDelFormulario.Text = "Identificación de Usuario";
                this.txtUsuario.Focus();
                this.estadoFormulario = EEstadoFormulario.EndLoad;
            }
            catch (Exception ex)
            {
                this.estadoFormulario = EEstadoFormulario.ErrorLoad;
                MessageBox.Show(ex.Message);
            }
            Clases.FuncionFormulario.ValidaCargaFormulario(estadoFormulario, this);
        }
        private void btnCerrar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnAceptar_Click(object sender, RoutedEventArgs e)
        {
            this.ValidaUsuario();
        }
        private void ValidaUsuario()
        {
            try
            {
                IConexion conexion = new Conexion.Oracle.ConexionOracle("DATA SOURCE=XE;USER ID=SISTEMA;PASSWORD=shomies2012");
                Core.Sistema.Usuario usuario = new Core.Sistema.Usuario(conexion);
                usuario.UserName = this.txtUsuario.Text.Trim();
                string clave = this.pasClave.Password.Trim();

                if (string.IsNullOrEmpty(usuario.UserName))
                    Funcion.EjecutaExepcionShomies("Ingrese nombre de usuario");
                if (string.IsNullOrEmpty(clave))
                    Funcion.EjecutaExepcionShomies("Ingrese clave de usuario");

                usuario.GetDatos();

                if (!usuario.Estado)
                    Funcion.EjecutaExepcionShomies("Usuario no activo");
                if (usuario.Clave != clave)
                    Funcion.EjecutaExepcionShomies("Clave no valida de usuario");

                new MainWindow(new Core.Sistema.AuditoriaSistema(conexion) { Usuario = usuario }, conexion).Show();

                this.Close();
            }
            catch (Utilitario.ExepcionSHomies sx)
            {
                MessageBox.Show(sx.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                this.txtUsuario.Focus();
            }
        }
        private void pasClave_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                    this.ValidaUsuario(); ;
            }
            catch (Exception)
            {
                throw;
            }
        }


    }
}
