using AlkilaApp.Modelos;
using AlkilaApp.Servicios;


namespace AlkilaApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DetallesProducto : ContentPage
    {


        public bool EsEmpresa { get; set; }

        private ServicioUbicacion _servicioUbicacion;
        private ServicioUsuario servicioUsuario;
        private ServicioProducto servicioProducto;
        private Usuario infoUsuario;
        private Productos _producto;
        private EditarDatosUsuario _editarDatosUsuario;
        private AnyadirProductos _anyadirProductos;
        private Ubicacion _ubicacion;
        private ServicioAlquiler _servicioAlquiler = new ServicioAlquiler();


        public DetallesProducto(string idUsuario, Productos producto, bool esProductoDelUsuario)
        {

            try
            {
              
                InitializeComponent();
                iconoEmpresa.IsVisible = false;

                servicioProducto = new ServicioProducto();

                _servicioUbicacion = new ServicioUbicacion();
                servicioUsuario = new ServicioUsuario();
                servicioUsuario.IdUsuario = idUsuario;
                _editarDatosUsuario = new EditarDatosUsuario(this.Navigation, servicioUsuario.IdUsuario);
                _anyadirProductos = new AnyadirProductos(servicioUsuario.IdUsuario);


                BindingContext = producto;

                NombreLabel.Text = producto.Nombre;
                PrecioLabel.Text = $"Precio: {producto.Precio}";
                DescripcionLabel.Text = producto.DescripcionProducto;
                FotoImage.Source = producto.Foto;
                TipoLabel.Text = Enum.GetName(typeof(TipoProducto), producto.Tipo);

                //almacenamos en la variable _producto los datos del producto 
                _producto = producto;



                // Comprobamos que el estado del producto este alquilado, si lo está, deshabilitamos el botón por que no se podrá alquilar mas de una vez 
                // no se podrá editar el producto mientras esté alquilado
                if (producto.EstaAlquilado == true)
                {
                    AlquilarButton.IsEnabled = false;
                    EditarButton.IsEnabled = false;

                }
                else
                {

                    // En caso contrario, vemos si es del usuario o es de otra presona, si es de nosotros no se prodrá alquilar, por lo contrario si
                    AlquilarButton.IsEnabled = esProductoDelUsuario;
                }


                // Habilitar o deshabilitar el botón basado en si el producto es del usuario registrado

                EditarButton.IsEnabled = !esProductoDelUsuario;
                EliminarButton.IsEnabled = !esProductoDelUsuario;

                _ubicacion = new Ubicacion();

                DetallesUsuario(producto.IdProducto);

            }
            catch (Exception)
            {

                throw;
            }

        }


        public async void DetallesUsuario(String idProducto)
        {

            try
            {
                infoUsuario = await servicioUsuario.ObtenerUsuarioPorIdProducto(idProducto);
                UsuarioLabel.Text = infoUsuario.Nombre;
                FotoLabel.Source = infoUsuario.Foto;


                iconoEmpresa.IsVisible = infoUsuario.EsEmpresa;

                // si es empresa se mostrará el logotipo como verificado, para diferenciar de la empresa




                // recogemos la ubicación y la mostramos en el mapa
                _ubicacion = await _servicioUbicacion.GetLocationAsync(infoUsuario.IdUsuario);

                if (_ubicacion != null) 
                {

                    DatosUbicacionUsuario.Text = _ubicacion.Calle + "," + _ubicacion.Localidad;

                }
                else
                {
                    DatosUbicacionUsuario.Text = "El usuaro no ha registrado una ubicación";
                }

            }
            catch (Exception)
            {

                Console.WriteLine("Error al insetar el nombre del usuario");
            }

        }


        private async void AlquilarButton_Clicked(object sender, EventArgs e)
        {

            // Ir a la ventana de alquilar productos
            await Navigation.PushAsync(new AlquilarProducto(servicioUsuario.IdUsuario, infoUsuario, _producto));

        }


    
        // Metodo que te da la ubicación del maps con una ruta
      
        public async Task RutaNavegacionUbicacion(Ubicacion ubicacion)
        {
            var options = await DisplayActionSheet("Selecciona el modo de navegación", "Cancelar", null, "Conducción", "Caminando", "En bicicleta");

            if (options == "Conducción")
            {
                await RutaNavegacionUbicacion(ubicacion, NavigationMode.Driving);
            }
            else if (options == "Caminando")
            {
                await RutaNavegacionUbicacion(ubicacion, NavigationMode.Walking);
            }
            else if (options == "En bicicleta")
            {
                await RutaNavegacionUbicacion(ubicacion, NavigationMode.Bicycling);
            }
        }




        private async Task RutaNavegacionUbicacion(Ubicacion ubicacion, NavigationMode mode)
        {
            var location = new Location(ubicacion.Latitud, ubicacion.Longitud);
            var options = new MapLaunchOptions
            {
                NavigationMode = mode
            };

            try
            {
                await Map.Default.OpenAsync(location, options);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Ocurrió un error al abrir la navegación: " + ex.Message, "Aceptar");
            }
        }



        // Metodo donde almaceno los datos del maps
        public async Task<string> ObtenerNombreLugar(double latitud, double longitud)
        {
            try
            {

                string cadena = "";

                var placemarks = await Geocoding.GetPlacemarksAsync(latitud, longitud);
                var placemark = placemarks?.FirstOrDefault();
                if (placemark != null)
                {
                    cadena = placemark.Thoroughfare + "\n";
                    cadena += placemark.Locality + "\n";
                    cadena += placemark.SubLocality + "\n";
                    cadena += placemark.SubThoroughfare + "\n";
                    cadena += placemark.Location + "\n";
                    cadena += placemark.CountryName + "\n";

                    return cadena;
                }
                else
                {
                    return "Nombre de lugar no encontrado";
                }
            }
            catch (Exception ex)
            {
                return "Error al obtener el nombre del lugar: " + ex.Message;
            }
        }



        // Al presionar sobre a imagen del mapa, se abrira el google maps con la ruta iniciada
        void OnFrameTapped(object sender, EventArgs e)
        {
            
            // Dirigirse a Google Maps con la ubicación registrada

             RutaNavegacionUbicacion(_ubicacion);

        }





        // Método para cerrar la ventana

        private async void EliminarButton_Clicked(object sender, EventArgs e)
        {


            // Si no esta alquilado puedes eliminarlo
            if (_producto.EstaAlquilado == false)
            {
                var result = await DisplayAlert("Éxito", $"¿Deseas eliminar el siguiente producto?", "OK", "Cancel");

                if (result == true)
                {
                        await servicioProducto.EliminarProductoPorId(_producto.IdProducto);
                        await DisplayAlert("Éxito", $"El producto ha sido eliminado correctamente", "OK");

                        // Actualizamos la lista de productos
                        _editarDatosUsuario.CargarMiListaProductos();

                        // Navegamos a la pagina anterior
                        await Navigation.PushAsync(_editarDatosUsuario);

                }

            }
            else
            {
                await DisplayAlert("Éxito", $"No puedes eliminar un producto mientras esté alquilado ", "OK");

            }

        }



        private async void EditarButton_Clicked(object sender, EventArgs e)
        {


            if (_producto.EstaAlquilado == true)
            {
                await DisplayAlert("Éxito", $"No puedes eliminar un producto mientras esté alquilado ", "OK");

            }

            try
            {

                _anyadirProductos.EditarProductos(_producto);

                // vamos a la ventana para modificar los productos
                await Navigation.PushAsync(_anyadirProductos);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }


    }
}