using System;

namespace AlkilaApp.Modelos
{
    public class Ubicacion
    {
        // Atributos
        private string localidad;
        private double latitud;
        private double longitud;
        private string calle;
        private string idUsuario;

        // Propiedades
        public string Localidad
        {
            get { return localidad; }
            set { localidad = value; }
        }

        public double Latitud
        {
            get { return latitud; }
            set { latitud = value; }
        }

        public double Longitud
        {
            get { return longitud; }
            set { longitud = value; }
        }

        public string Calle
        {
            get { return calle; }
            set { calle = value; }
        }

        public string IdUsuario
        {
            get { return idUsuario; }
            set { idUsuario = value; }
        }  
    }
}
