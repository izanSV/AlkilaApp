using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;

namespace AlkilaApp.Modelos
{
    public class Alquiler
    {
        private DateTime _FechaInicio;
        public DateTime FechaInicio
        {
            get => _FechaInicio;
            set
            {
                _FechaInicio = value;
            }
        }

        private DateTime _FechaFin;
        public DateTime FechaFin
        {
            get => _FechaFin;
            set
            {
                _FechaFin = value;
            }
        }

        private string _IdProducto;
        public string IdProducto
        {
            get => _IdProducto;
            set
            {
                _IdProducto = value;
            }
        }

        private string _IdAlquiler;
        public string IdAlquiler
        {
            get => _IdAlquiler;
            set
            {
                _IdAlquiler = value;
            }
        }

        private string _IdUsuarioComprador;
        public string IdUsuarioComprador
        {
            get => _IdUsuarioComprador;
            set
            {
                _IdUsuarioComprador = value;
            }
        }

        private string _IdUsuarioVendedor;
        public string IdUsuarioVendedor
        {
            get => _IdUsuarioVendedor;
            set
            {
                _IdUsuarioVendedor = value;
            }
        }


        private string _NombreProductoAlquilado;
        public string NombreProductoAlquilado
        {
            get => _NombreProductoAlquilado;
            set
            {
                _NombreProductoAlquilado = value;
            }
        }


        private string _FotoProductoAlquilado;
        public string FotoProductoAlquilado
        {
            get => _FotoProductoAlquilado;
            set
            {
                _FotoProductoAlquilado = value;
            }
        }



        private double _PrecioTotal;
        public double PrecioTotal
        {
            get => _PrecioTotal;
            set
            {
                _PrecioTotal = value;
            }
        }




        private string _NombreUsuarioComprador;
        public string NombreUsuarioComprador
        {
            get => _NombreUsuarioComprador;
            set
            {
                _NombreUsuarioComprador = value;
            }
        }

        // Tipo Producto
        public Estado EstadoAlquiler { get; set; }

    }


    public enum Estado
    {
        Pendiente = 0,
        Aceptado = 1,
        Cancelado= 2,
        Finalizado = 3
    }



}
