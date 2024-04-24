namespace AlkilaApp.Modelos
{
    public class Productos
    {


        private string? _IdProducto;
        public string IdProducto
        {
            get => _IdProducto;
            set
            {
                _IdProducto = value;
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


        private string? _DescripcionProducto;
        public string DescripcionProducto
        {
            get => _DescripcionProducto;
            set
            {
                _DescripcionProducto = value;
            }
        }


        private double _Precio;
        public double Precio
        {
            get => _Precio;
            set
            {
                _Precio = value;
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




        private bool _estaAlquilado;
        public bool EstaAlquilado
        {
            get => _estaAlquilado;
            set
            {
                _estaAlquilado = value;
            }
        }



        // Tipo Producto
        public TipoProducto Tipo { get; set; }


        //valoración

        private double? _Valoracion;
        public double? Valoracion
        {
            get => _Valoracion;
            set
            {
                _Valoracion = value;
            }
        }


        public Productos()
        {

        }

    }
}

public enum TipoProducto
{
    Entretenimiento = 0,
    Moda = 1,
    Deporte = 2,
    Herramientas = 3,
    Eventos = 4
}

