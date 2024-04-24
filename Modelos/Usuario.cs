using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlkilaApp.Modelos
{
    public class Usuario
    {

        private string? _IdUsuario;
        public string IdUsuario
        {
            get => _IdUsuario;
            set
            {
                _IdUsuario = value;

            }
        }



        private string? _Nombre;
        public string Nombre
        {
            get => _Nombre;
            set
            {
                _Nombre = value;

            }
        }



        private string? _Apellido;
        public string Apellido
        {
            get => _Apellido;
            set
            {
                _Apellido = value;
            }
        }



        private string? _CorreoElectronico;
        public string CorreoElectronico
        {
            get => _CorreoElectronico;
            set
            {
                _CorreoElectronico = value;
            }
        }



        private string _Contrasenya;
        public string Contrasenya
        {
            get => _Contrasenya;
            set
            {
                _Contrasenya = value;
            }
        }



        private DateTime _FechaNacimiento;
        public DateTime FechaNacimiento
        {
            get => _FechaNacimiento;
            set
            {
                _FechaNacimiento = value;
            }
        }


        private string? _Foto;
        public string Foto
        {
            get => _Foto;
            set
            {
                _Foto = value;
            }
        }



        private bool _EsEmpresa;
        public bool EsEmpresa
        {
            get => _EsEmpresa;
            set
            {
                _EsEmpresa = value;
            }
        }


        private List<Productos>? _ListaProductos;

        public List<Productos> ListaProductos

        {
            get => _ListaProductos;
            set
            {
                _ListaProductos = value;
            }

        }
    }
}
