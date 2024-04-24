using AlkilaApp.Modelos;
using AlkilaApp.Servicios;
using SkiaSharp;
using System.ComponentModel;


namespace AlkilaApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AnyadirProductos : ContentPage, INotifyPropertyChanged

    {

        const string FOTO_PERFIL = "https://firebasestorage.googleapis.com/v0/b/alkila-bbdd.appspot.com/o/FotoPerfil.jpeg?alt=media&token=72d2a9e6-63e9-4b00-8040-24f3370dc765";


        private ServicioProducto servicioProductos;

        private Productos producto = new Productos();

        private ServicioUsuario servicioUsuario;

        private EditarDatosUsuario _editarDatosUsuario;

        // Esta variable es para controlar que el usuario pueda volver a la pagina anterior en el caso de que quiera anyadir un producto, por lo contrario no podrá volver 
        // hacia atrás, por que se elimina de la memoria el objeto y se pierden los datos
        private bool _paginaDetalleProducto = true;

    



        public event PropertyChangedEventHandler? PropertyChanged;


        public AnyadirProductos(string id)
        {

            NavigationPage.SetHasBackButton(this, true);
            InitializeComponent();
            servicioProductos = new ServicioProducto();
            servicioUsuario = new ServicioUsuario();
            servicioUsuario.IdUsuario = id;
            editarButton.IsEnabled = false;

            // variable para actualizar la lista de productos
            _editarDatosUsuario = new EditarDatosUsuario(this.Navigation,id);

           

            // Agregar las opciones del enum TipoProducto al Picker
            foreach (TipoProducto tipo in Enum.GetValues(typeof(TipoProducto)))
            {
                TipoProductoPicker.Items.Add(tipo.ToString());
            }
        }


        private void CancelarDatosClicked(object sender, EventArgs e)
        {
            try
            {
                // Aquí puedes agregar tu lógica para determinar si mostrar u ocultar el botón de retroceso

                 animaacionButton(sender, e);
                 Navigation.PushAsync(new EditarDatosUsuario(this.Navigation,servicioUsuario.IdUsuario));

            }
            catch (Exception)
            {

                throw;
            }
        }


        private async void GuardarDatosClicked(object sender, EventArgs e)
        {


             animaacionButton(sender, e);


            // Crear una variable para almacenar el precio
            double precio;

            // Obtener el tipo de producto seleccionado del Picker
            string? tipoSeleccionado = TipoProductoPicker.SelectedItem?.ToString();

            if (!string.IsNullOrEmpty(tipoSeleccionado))
            {
                // Convertir el string seleccionado al enum correspondiente
                TipoProducto tipoProductoEnum;
                Enum.TryParse(tipoSeleccionado, out tipoProductoEnum);

                // Ahora puedes asignar el tipo de producto al objeto Productos
                producto.Tipo = tipoProductoEnum;

                // Convertir el texto ingresado en PrecioEntry a double
                if (double.TryParse(PrecioEntry.Text, out precio))
                {

                    // Si no existe creamos uno nuevo y le añadimos su id
                    producto.IdProducto = Guid.NewGuid().ToString();
                    producto.Nombre = NombreEntry.Text;
                    producto.DescripcionProducto = DescripcionEditor.Text;
                    //Por defecto esta disponible
                    producto.EstaAlquilado = false;
                    producto.Precio = precio;

                    
                  //  SendWhatsApp("+34695218574", "Hola jhon");

                    await servicioUsuario.AgregarProductoAUsuario(producto);
                    _editarDatosUsuario.CargarMiListaProductos();
                  await  Navigation.PushAsync(new EditarDatosUsuario(this.Navigation,servicioUsuario.IdUsuario));

                }
                else
                {
                    // El texto ingresado no es un número válido, mostrar un mensaje de error
                    await DisplayAlert("Error", "El precio ingresado no es válido.", "OK");
                }
            }
        }


       
        //TODOOOOOO, no  se actualiza
        public async void EditarProductos(Productos prodEdit)
        {
            // deshabilitamos el botón para que el usuario sepa que se están editando los datos
            guardarButton.IsEnabled = false;
            editarButton.IsEnabled = true;
            // Deshabilitamos el botón, por que al retroceder se eliminan datos de la pila de memoria
            _paginaDetalleProducto = false;

            try
            {

                if (prodEdit != null)
                {

                    // TODO Quizá
                    boton_foto_perfil.Source = prodEdit.Foto;
                    NombreEntry.Text = prodEdit.Nombre;
                    DescripcionEditor.Text = prodEdit.DescripcionProducto;
                    PrecioEntry.Text = prodEdit.Precio.ToString();
                    TipoProductoPicker.SelectedItem = prodEdit.Tipo.ToString();

                    producto = prodEdit;
                }


            }
            catch (Exception)
            {

                throw;
            }


        }



        private async void EditarDatosClicked(object sender, EventArgs e)
        {


             animaacionButton(sender, e);


            double precio;

            // Obtener el tipo de producto seleccionado del Picker
            string? tipoSeleccionado = TipoProductoPicker.SelectedItem?.ToString();

            if (!string.IsNullOrEmpty(tipoSeleccionado))
            {
                // Convertir el string seleccionado al enum correspondiente
                TipoProducto tipoProductoEnum;
                Enum.TryParse(tipoSeleccionado, out tipoProductoEnum);

                // Ahora puedes asignar el tipo de producto al objeto Productos
                producto.Tipo = tipoProductoEnum;

                // Convertir el texto ingresado en PrecioEntry a double
                if (double.TryParse(PrecioEntry.Text, out precio))
                {
                    // Creamos un id único por cada producto

                    producto.Nombre = NombreEntry.Text;
                    producto.DescripcionProducto = DescripcionEditor.Text;
                    //Por defecto esta disponible
                    producto.Precio = precio;


                    await servicioUsuario.ActualizarProductoAUsuario(producto);
                    _editarDatosUsuario.CargarMiListaProductos();
                    await Navigation.PushAsync(_editarDatosUsuario);

                }
                else
                {
                    // El texto ingresado no es un número válido, mostrar un mensaje de error
                    await DisplayAlert("Error", "El precio ingresado no es válido.", "OK");
                }


            }
        }



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

                        // Convertir la imagen a un bitmap utilizando SkiaSharp
                        using (var stream = await result.OpenReadAsync())
                        using (var bitmap = SKBitmap.Decode(stream))
                        {
                            // Redimensionar la imagen a 300x300
                            using (var resizedBitmap = bitmap.Resize(new SKImageInfo(480, 640), SKFilterQuality.Medium))
                            {
                                // Convertir el bitmap redimensionado a un array de bytes
                                using (var resizedImageStream = new MemoryStream())
                                {
                                    resizedBitmap.Encode(resizedImageStream, SKEncodedImageFormat.Jpeg, 100);
                                    var resizedImageData = resizedImageStream.ToArray();

                                    // Subir la imagen redimensionada a Firebase Storage con el nombre del archivo único
                                    var imageUrl = await servicioProductos.AddFoto(new MemoryStream(resizedImageData), fileName);

                                    if (imageUrl != null)
                                    {
                                        producto.Foto = imageUrl;
                                        boton_foto_perfil.Source = imageUrl;

                                        // Hacer algo con la URL de la imagen, como mostrarla en algún lugar de la interfaz de usuario
                                        Console.WriteLine("URL de la imagen: " + imageUrl);
                                    }
                                    else
                                    {
                                        // Manejar el caso en el que no se pudo subir la imagen correctamente
                                        Console.WriteLine("Error al subir la imagen.");
                                        await DisplayAlert("Error", "No se pudo subir la imagen.", "OK");
                                    }
                                }
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


        // Esta animación se aplica cada vez que se presiona un botón
        private  void animaacionButton(object sender, EventArgs e)
        {
            ImageButton button = (ImageButton)sender;

            // Desactivar la interacción
            button.InputTransparent = true;

            // Simular la animación de presionar el botón
             button.ScaleTo(0.6, 40);
             button.ScaleTo(1, 50);
             button.ScaleTo(0.9, 60);
             button.ScaleTo(1.1, 70);

            // Revertir la desactivación de la interacción
            button.InputTransparent = false;


        }




        // Cuando el usuario intente salir de la vista sin guardar los datos
        [Obsolete]
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                bool answer = await DisplayAlert("Salir", "¿Salir sin guardar?", "Confirmar", "Cancelar");
                if (answer)
                {
                    await Navigation.PushAsync(new EditarDatosUsuario(this.Navigation,servicioUsuario.IdUsuario));
                }
            });

            return true; // Indicamos que hemos manejado el evento nosotros mismos
        }









        public static async void SendWhatsApp(string phoneNumber, string message = null)
        {
            try
            {
                string text = "whatsapp://send?phone=" + phoneNumber;
                if (!string.IsNullOrWhiteSpace(message))
                {
                    text = text + "&text=" + message;
                }

                await Browser.Default.OpenAsync(new Uri(text), BrowserLaunchMode.External);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }




        }
    }