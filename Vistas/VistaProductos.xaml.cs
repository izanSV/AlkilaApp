using AlkilaApp.Modelos;
using AlkilaApp.Servicios;
using AlkilaApp.Vistas;
using Firebase.Database;
using Microsoft.Maui.Controls;


namespace AlkilaApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VistaProductos : ContentPage
    {

        FirebaseClient firebase;

        ServicioUsuario servicioUsuario;

        ServicioAlquiler servicioAlquiler;

        ServicioProducto servicioProducto = new ServicioProducto();


        Productos producto = new Productos();



        public VistaProductos( string id)
        {
            InitializeComponent();
            BindingContext = this;

            // Inicializa la lista de tipos de productos con los nombres de la enumeración TipoProducto

            firebase = new FirebaseClient(Setting.FireBaseDatabaseUrl, new FirebaseOptions

            {
                AuthTokenAsyncFactory = () => Task.FromResult(Setting.FireBaseSeceret)
            });
            servicioUsuario = new ServicioUsuario();
            servicioAlquiler = new ServicioAlquiler();
            servicioUsuario.IdUsuario = id;
            CargarTiposProducto();
            CargarListaProductos();
            ComprobarEstadoFinal();
        }

        private async void CargarListaProductos()
        {
            try
            {

                

                // Obtener la lista de productos del usuario
                var listaProductos = await servicioUsuario.ObtenerProductosUsuariosNoLogeados(servicioUsuario.IdUsuario);
                // Asignar la lista de productos a la propiedad ItemsSource del CollectionView vertical
                ProductosCollectionView.ItemsSource = listaProductos;
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción que ocurra
                Console.WriteLine("Error al cargar productos: " + ex.Message);
            }
        }


        private void CargarTiposProducto()
        {
            // Obtiene todos los valores del enum TipoProducto y los convierte en una lista de string
            var tiposProducto = Enum.GetNames(typeof(TipoProducto)).ToList();

            // Asigna la lista de tiposProducto al ItemsSource del CollectionView
            TiposProductoCollectionView.ItemsSource = tiposProducto;
        }


        private async void OnCardTapped(object sender, EventArgs e)
        {
           
            var frame = (Frame)sender;
            var producto = frame.BindingContext as Productos;

            // Verificar si el producto pertenece al usuario registrado
            if (producto != null && ProductoPerteneceAlUsuarioRegistrado(producto))
            {
                // Crear la instancia de DetallesProducto con el producto y el indicador de si es del usuario registrado
                var detallesProductoPage = new DetallesProducto(servicioUsuario.IdUsuario, producto, esProductoDelUsuario: true);

                // Mostrar los detalles del producto seleccionado en una nueva página
                await Navigation.PushAsync(detallesProductoPage);
            }
            else
            {
                 // Crear la instancia de DetallesProducto con el producto y el indicador de si es del usuario registrado
                var detallesProductoPage = new DetallesProducto(servicioUsuario.IdUsuario,producto, esProductoDelUsuario: false);

                // Mostrar los detalles del producto seleccionado en una nueva página
                await Navigation.PushAsync(detallesProductoPage);
            }
        }


        // Método para verificar si el producto pertenece al usuario registrado
        private bool ProductoPerteneceAlUsuarioRegistrado(Productos producto)
        {
            // Obtener la lista de productos del usuario registrado
            var listaProductosUsuarioRegistrado = (List<Productos>)ProductosCollectionView.ItemsSource;

            // Verificar si el producto está presente en la lista de productos del usuario registrado
            return listaProductosUsuarioRegistrado.Contains(producto);
        }



        private async void OnCollectionViewItemTapped(object sender, EventArgs e)
        {
            var frame = sender as Frame;
            if (frame != null)
            {
                string? item = frame.BindingContext as string;

                var tipoProducto = (TipoProducto)Enum.Parse(typeof(TipoProducto), item);

                var listaProductos = await servicioUsuario.ObtenerListaProductosPorTipo( tipoProducto, servicioUsuario.IdUsuario);

                // Asignar la lista de productos a la propiedad ItemsSource del CollectionView vertical
                ProductosCollectionView.ItemsSource = listaProductos;

            }
        }

        private async void ComprobarEstadoFinal()
        {
          await  EstadoFechaFinal();
        }

        private async Task<bool> EstadoFechaFinal()
        {
            try
            {
                List<Alquiler> listaProductosAlquilados = await servicioAlquiler.GetAlquileresByUsuarioCompradorYVendedorId(servicioUsuario.IdUsuario);

                bool primeraCondicionCumplida = false;

                foreach (Alquiler item in listaProductosAlquilados)
                {
                    if (item.EstadoAlquiler == Estado.Pendiente && item.IdUsuarioVendedor.Equals(servicioUsuario.IdUsuario) && !primeraCondicionCumplida)
                    {
                        await DisplayAlert("Éxito", $"Comprueba el buzón de mensajes (🐖), tienes una propuesta para alquilar un producto de un usuario", "Aceptar");
                        primeraCondicionCumplida = true;
                    }

                    if (item.EstadoAlquiler == Estado.Aceptado && item.FechaFin <= DateTime.Now)
                    {
                        if (item.IdUsuarioComprador.Equals(servicioUsuario.IdUsuario))
                        {
                            await DisplayAlert("Éxito", $"Tu producto está listo para devolverse", "Aceptar");
                        }
                        else if (item.IdUsuarioVendedor.Equals(servicioUsuario.IdUsuario))
                        {
                            await DisplayAlert("Éxito", $"Se ha finalizado el plazo de entrega de este producto, debes de devolverlo al dueño en la ubicación donde lo recogiste", "Aceptar");
                        }
                        else
                        {
                            await DisplayAlert("Error", $"No deberías estar aquí", "Aceptar");
                        }

                        item.EstadoAlquiler = Estado.Finalizado;
                        producto = await servicioProducto.ObtenerProductoPorId(item.IdUsuarioVendedor);

                        // Verificar que el producto no sea nulo antes de intentar acceder a sus propiedades
                        if (producto != null)
                        {
                            producto.EstaAlquilado = false;
                            await servicioUsuario.ActualizarProductoAUsuario(producto);
                        }

                        await servicioAlquiler.InsertOrUpdateAlquiler(item);
                    }
                }

                return primeraCondicionCumplida;
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción que ocurra sin detener la aplicación
                Console.WriteLine($"Se produjo un error en EstadoFechaFinal(): {ex.Message}");
                return false;
            }
        }


        private async void ShowOptions()
        {
            string action = await DisplayActionSheet("Valora nuestro producto", "Cancelar", "0", "1", "2", "3", "4", "5");

            // Aquí puedes manejar la opción seleccionada
            switch (action)
            {
                case "1":
                    // Código para la opción 1
                    break;
                case "2":
                    // Código para la opción 2
                    break;
                    // y así sucesivamente para las opciones 3, 4 y 5
                    default: throw new Exception();
            }
               
        }


        // botones app vista principal

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



        private async void btnAnyadirProducto(object sender, EventArgs e)
        {

        
            await animaacionButton(sender, e);
            await Navigation.PushAsync(new AnyadirProductos(servicioUsuario.IdUsuario));

        }


        private async void btnMisAlquileres(object sender, EventArgs e)
        {

            await animaacionButton(sender, e);
            await Navigation.PushAsync(new ProductosAlquiladosVista(servicioUsuario.IdUsuario));

        }


        private async void btnMostrarDatosUsuario(object sender, EventArgs e)
        {
            await animaacionButton(sender, e);
            await Navigation.PushAsync(new EditarDatosUsuario(this.Navigation, servicioUsuario.IdUsuario));
        }


        private async void btnVistaPrincipal(object sender, EventArgs e)
        {

            await animaacionButton(sender, e);
            await Navigation.PushAsync(new VistaProductos(servicioUsuario.IdUsuario));

        }


        private async void CerrarSesionClicked(object sender, EventArgs e)
        {


            bool answer = await DisplayAlert("Salir", "¿Estas seguro que quieres salir de la aplicación?", "Confirmar", "Cancelar");
            if (answer)
            {
                await Navigation.PushAsync(new LoginVista());
            }

        }






        // Cuando el usaurio presiona el botón hacia atras

        [Obsolete]
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                bool answer = await DisplayAlert("Salir", "¿Estas seguro que quieres salir de la aplicación?", "Confirmar", "Cancelar");
                if (answer)
                {
                    App.Current.Quit();
                }
            });

            return true; // Indicamos que hemos manejado el evento nosotros mismos
        }
    }
}