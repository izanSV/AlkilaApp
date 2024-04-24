using AlkilaApp.Modelos;
using AlkilaApp.Servicios;
using System.ComponentModel;



//todo Crear excepciones personalizadas


namespace AlkilaApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditarDatosUsuario : ContentPage, INotifyPropertyChanged

    {
        private INavigation _Navigation;

        private string? _Nombre;

        public string? Nombre
        {
            get => _Nombre;
            set
            {
                _Nombre = value;
                RaisePropertyChanged("Nombre");

            }
        }

        private string? _Apellido;

        public string? Apellido
        {
            get => _Apellido;
            set
            {
                _Apellido = value;
                RaisePropertyChanged("Apellido");
            }
        }

        private string _FechaNacimiento;

        public string FechaNacimiento
        {
            get => _FechaNacimiento;
            set
            {
                _FechaNacimiento = value;
                RaisePropertyChanged(nameof(FechaNacimiento));
            }
        }


        // Obtener ubicación

        private string? _Localidad;
        public string? Localidad
        {
            get => _Localidad;
            set
            {
                _Localidad = value;
                RaisePropertyChanged("Localidad");
            }
        }


        private string? _Calle;
        public string? Calle
        {
            get => _Calle;
            set
            {
                _Calle = value;
                RaisePropertyChanged("Calle");
            }
        }

        private ServicioUsuario servicioUsuario;

        private Usuario usuario = new Usuario();



        private Ubicacion ubicacion = new Ubicacion();

        private ServicioUbicacion _servicioUbicacion = new ServicioUbicacion();



        private void RaisePropertyChanged(string v)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(v));
        }



        public event PropertyChangedEventHandler? PropertyChanged;


        public EditarDatosUsuario(INavigation navigation, string idUsuario)
        {
            InitializeComponent();
            _Navigation = navigation; // Asigna el objeto INavigation recibido como parámetro
            servicioUsuario = new ServicioUsuario();
            servicioUsuario.IdUsuario = idUsuario;

            CargarDatosUsuario();
            CargarMiListaProductos();
        }


        private async void CargarDatosUsuario()
        {
            try
            {

                // Obtener una referencia al nodo que contiene los datos del usuario
                usuario = await servicioUsuario.GetUsuarioRegistrado();

                ubicacion = await _servicioUbicacion.GetLocationAsync(servicioUsuario.IdUsuario);


                // Asignar los datos del usuario a los campos de nombre y apellido
                NombreEntry.Text = usuario.Nombre;
                ApellidoEntry.Text = usuario.Apellido;
                ContrasenyaEntry.Text = usuario.Contrasenya;
                CorreoElectronicoEntry.Text = usuario.CorreoElectronico;
                FechaNacimientoEntry.Text = usuario.FechaNacimiento.ToString("dd/MM/yyyy");
                boton_foto_perfil.Source = usuario.Foto;


             if (ubicacion != null)
                {
                    // Cargamos los datos de la ubicación
                    CalleEntry.Text = ubicacion.Calle;
                    LocalidadEntry.Text = ubicacion.Localidad;
                }

            }
            catch (Exception)
            {

                throw;
            }
        }


     


        // Adaptar para utilizarlo con los productos
        private async void LogoEmpresa_Clicked(object sender, EventArgs e)
        {
            try
            {
                // Abrir la galería de imágenes
                var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Seleccione una imagen"
                });

                // Verificar si se seleccionó una imagen
                if (result != null)
                {

                    // Obtener el nombre del archivo único para la imagen
                    string fileName = Guid.NewGuid().ToString() + ".jpg";


                    // Subir la imagen a Firebase Storage con el nombre del archivo único
                    var stream = await result.OpenReadAsync();

                    {
                        var imageUrl = await servicioUsuario.AddFoto(stream, fileName);
                        if (imageUrl != null)
                        {
                            usuario.Foto = imageUrl;
                            boton_foto_perfil.Source = imageUrl;


                            // Hacer algo con la URL de la imagen, como mostrarla en algún lugar de la interfaz de usuario
                            Console.WriteLine("URL de la imagen: " + imageUrl);
                        }
                        else
                        {
                            Console.WriteLine("URL de la imagen: " + imageUrl);
                            // Manejar el caso en el que no se pudo subir la imagen correctamente
                            Console.WriteLine("Error al subir la imagen.");
                            await DisplayAlert("Error", "No se pudo subir la imagen.", "OK");
                        }
                    }
                }
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine("Error: La operación fue cancelada.");
                // Manejar el error de cancelación de la tarea
                await DisplayAlert("Error", "La operación fue cancelada.", "OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                // Manejar cualquier otra excepción
                await DisplayAlert("Error", "Ocurrió un error inesperado: " + ex.Message, "OK");
            }

        }


        public async void ActualizarListaProductos()
        {
            try
            {
                // Obtener la lista de productos actualizada
                var listaProductosActualizada = await servicioUsuario.ObtenerListaProductos(servicioUsuario.IdUsuario);

                // Asignar la lista de productos actualizada a la propiedad ItemsSource del CollectionView vertical
                ProductosCollectionView.ItemsSource = listaProductosActualizada;

                // Verificar si la lista de productos está vacía
                if (listaProductosActualizada == null || listaProductosActualizada.Count == 0)
                {
                    // Si la lista está vacía, hacer visible el Label que indica que no hay productos
                    NoProductosLabel.IsVisible = true;
                }
                else
                {
                    // Si la lista no está vacía, ocultar el Label
                    NoProductosLabel.IsVisible = false;
                }
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción que ocurra
                Console.WriteLine("Error al cargar productos: " + ex.Message);
            }
        }

        // Método para cargar inicialmente la lista de productos
        public void CargarMiListaProductos()
        {
            try
            {
                // Cargar la lista de productos inicial
                 ActualizarListaProductos();
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción que ocurra
                Console.WriteLine("Error al cargar productos: " + ex.Message);
            }
        }

        private async void OnCardTapped(object sender, EventArgs e)
        {

            try
            {

                var frame = (Frame)sender;
                var producto = frame.BindingContext as Productos;

                var detallesProductoPage = new DetallesProducto(servicioUsuario.IdUsuario, producto, esProductoDelUsuario: false);
                await Navigation.PushAsync(detallesProductoPage);


            }
            catch (Exception ex)
            {

                Console.WriteLine("Error al cargar productos: " + ex.Message);
            }


        }










        // Botón para registrar la  ubicación en tiempo real

        private async void BtnUbicacionActual(object sender, EventArgs e)
        {

            try
            {
                // Guardamos la ubicación guardada al presionar el botón

                // No puede haber campos escritos

                ubicacion = await RegistrarUbicacionEnTiempoReal();
                if (ubicacion != null)
                {
                    Console.WriteLine(ubicacion);
                    await DisplayAlert("Ubicación registrada", (ubicacion.Calle + " ," + ubicacion.Localidad), "Aceptar");
                    await _servicioUbicacion.InsertOrUpdateUbicacion(ubicacion);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("error" + ex.Message);
                throw;
            }
        }


            // Boton para comprobar que la ubicación insertada por el usuario es correcta


            private async void BtnComprobarUbicacion(object sender, EventArgs e)
    {

        try

        {
            string localidad = LocalidadEntry.Text;
            string calle = CalleEntry.Text;

            ubicacion = await RegistrarUbicacion(localidad, calle);
            Console.WriteLine(ubicacion);
               


            }
        catch (Exception)
        {

            throw;
        }

    }


        public async Task<Ubicacion> RegistrarUbicacion(string localidad, string calle)
        {

            try
            {

                var coordenadas = await ObtenerCoordenadas(localidad, calle);


                if (coordenadas != null)
                {
                    // Las coordenadas están disponibles, puedes acceder a coordenadas.Latitude y coordenadas.Longitude
                    double latitud = coordenadas.Latitude;
                    double longitud = coordenadas.Longitude;


                    string lugar = await ObtenerNombreLugar(latitud, longitud);


                    // La cadena nos da toda la información del lugar, lo he separado en dos parametros, primero la calle y después los datos del lugar
                    string[] partes = lugar.Split(',');

                    calle = partes[0];
                    localidad = partes[1];


                    await DisplayAlert("Ubicación registrada", lugar, "Aceptar");

                    return new Ubicacion
                    {
                        Latitud = latitud,
                        Longitud = longitud,
                        Calle = calle,
                        Localidad = localidad,
                    };

                }
                else
                {
                    await DisplayAlert("Error", "La ubicación no es correcta o no está activada", "Aceptar");
                    return null;
                }

            }
            catch (Exception)
            {

                await DisplayAlert("Error", $"No se ha podido encontrar la ubicación", "Aceptar");

                return null;
            }

        }

        public async Task<string> ObtenerNombreLugar(double latitud, double longitud)
        {
            try
            {

                string cadena = "";

                var placemarks = await Geocoding.GetPlacemarksAsync(latitud, longitud);
                var placemark = placemarks?.FirstOrDefault();
                if (placemark != null)
                {
                    cadena = placemark.Thoroughfare + ",";
                    cadena += placemark.SubThoroughfare + "\n";
                    cadena += placemark.Locality + ",";
                    cadena += placemark.SubLocality + ",";
                    cadena += placemark.CountryName;

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

        public static async Task<Location?> ObtenerCoordenadas(string localidad, string calle)
        {
            try
            {
                var locations = await Geocoding.GetLocationsAsync($"{calle}, {localidad}");
                return locations?.FirstOrDefault();
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Manejar caso de que la geocodificación no sea compatible en este dispositivo/plataforma
                return null;
            }
            catch (Exception ex)
            {
                // Manejar otros errores
                return null;
            }
        }

        public async Task<Ubicacion> RegistrarUbicacionEnTiempoReal()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                {
                    // Si el permiso de ubicación no está concedido, solicitamos permiso al usuario
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                    if (status != PermissionStatus.Granted)
                    {
                        await DisplayAlert("Permiso denegado", "No se puede acceder a la ubicación porque el permiso fue denegado.", "Aceptar");
                        return null;
                    }
                }

                var location = await Geolocation.GetLocationAsync();
                if (location != null)
                {
                    // Almacenamos aquí la longitud y la latitud del objeto ubicacion
                    double latitud = location.Latitude;
                    double longitud = location.Longitude;

                    string lugar = await ObtenerNombreLugar(latitud, longitud);

                    // La cadena nos da toda la información del lugar, lo he separado en dos parametros, primero la calle y después los datos del lugar
                    string[] partes = lugar.Split(' ');
                    string calle = partes[0];
                    string localidad = partes[1];

                    return new Ubicacion
                    {
                        Latitud = latitud,
                        Longitud = longitud,
                        Calle = calle,
                        Localidad = localidad,
                    };
                }
                else
                {
                    // Si no se pudo obtener la ubicación
                    await DisplayAlert("Error", "No se pudo obtener la ubicación actual.", "Aceptar");
                    return null;
                }
            }
            catch (FeatureNotSupportedException)
            {
                // La funcionalidad de geolocalización no es soportada en este dispositivo
                await DisplayAlert("Error", "La geolocalización no es soportada en este dispositivo.", "Aceptar");
                return null;
            }
            catch (PermissionException)
            {
                // El permiso para acceder a la ubicación fue denegado por el usuario
                await DisplayAlert("Error", "El permiso para acceder a la ubicación fue denegado.", "Aceptar");
                return null;
            }
            catch (Exception ex)
            {
                // Si ocurre algún otro error
                await DisplayAlert("Error", $"Error al obtener la ubicación: {ex.Message}", "Aceptar");
                return null;
            }
        }

        public async Task AbrirGoogleMaps(string localidad, string calle, double longitud, double latitud)
        {
            var placemark = new Placemark
            {
                CountryName = "Spain",
                Thoroughfare = calle,
                Locality = localidad
            };

            var coordenadas = await ObtenerCoordenadas(localidad, calle);

            if (coordenadas != null)
            {
                // Las coordenadas están disponibles, puedes acceder a coordenadas.Latitude y coordenadas.Longitude
                latitud = coordenadas.Latitude;
                longitud = coordenadas.Longitude;

                await DisplayAlert("Error", $"goolge maps: {latitud},{longitud}", "Aceptar");

                string lugar = await ObtenerNombreLugar(latitud, longitud);

                await DisplayAlert("Error", lugar, "Aceptar");

            }
            else
            {

            }


            try
            {
                await placemark.OpenMapsAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al obtener la ubicación en goolge maps: {ex.Message}", "Aceptar");
            }
        }








        private async void OnEditarCamposClicked(object sender, EventArgs e)
        {
            try
            {
                await animaacionButton(sender, e);
                // Habilita la edición de los campos de entrada
                NombreEntry.IsEnabled = true;
                ApellidoEntry.IsEnabled = true;
                FechaNacimientoEntry.IsEnabled = true;

                CalleEntry.IsEnabled = true;
                LocalidadEntry.IsEnabled = true;

            }
            catch (Exception)
            {
                throw;
            }
        }


        private async void CancelarDatosClicked(object sender, EventArgs e)
        {
            try
            {

                // Vuelta atrás sin guardar datos
                await animaacionButton(sender, e);
                await _Navigation.PushAsync(new VistaProductos(servicioUsuario.IdUsuario));

            }
            catch (Exception)
            {

                throw;
            }
        }


        private async void GuardarDatosClicked(object sender, EventArgs e)
        {

            try
            {

                await animaacionButton(sender, e);

                // Guardar los datos del usuario en la base de datos Firebase
                Nombre = NombreEntry.Text;
                Apellido = ApellidoEntry.Text;
                FechaNacimiento = FechaNacimientoEntry.Text;

                usuario.Nombre = Nombre;
                usuario.Apellido = Apellido;

               // ubicacion.Calle = Calle;
               // ubicacion.Localidad = Localidad;

                await servicioUsuario.AnyadirOActualizarUsuario(usuario);



                // Guardar id para hacer referencia al usuario

                ubicacion.IdUsuario = servicioUsuario.IdUsuario;

                Console.WriteLine(ubicacion);
                // metodo Firebase Guardar Ubicacion (ubicacion)
                await _servicioUbicacion.InsertOrUpdateUbicacion(ubicacion);


                await _Navigation.PushAsync(new VistaProductos(servicioUsuario.IdUsuario));

            }
            catch (Exception)
            {

                throw;
            }



            // await Navigation.PushAsync(new VistaProductos());
        }




        // Esta animación se aplica cada vez que se presiona un botón
        private async Task animaacionButton(object sender, EventArgs e)
        {
            ImageButton button = (ImageButton)sender;

            // Desactivar la interacción
            button.InputTransparent = true; 

            // Simular la animación de presionar el botón
            await button.ScaleTo(0.6, 40);
            await button.ScaleTo(1, 50);
            await button.ScaleTo(0.9, 60);
            await button.ScaleTo(1.1, 70);

            // Revertir la desactivación de la interacción
            button.InputTransparent = false;


        }


        //Navegamos a la página principal
        [Obsolete]
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                
                    await Navigation.PushAsync(new VistaProductos( servicioUsuario.IdUsuario)); 
            });

            return true; // Indicamos que hemos manejado el evento nosotros mismos
        }

    }
}