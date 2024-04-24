using AlkilaApp.Modelos;
using AlkilaApp.Servicios;

namespace AlkilaApp.Vistas;

public partial class ProductosAlquiladosVista : ContentPage
{

    private ServicioUsuario servicioUsuario = new ServicioUsuario();

    private ServicioAlquiler servicioAlquilar = new ServicioAlquiler();

    private ServicioProducto servicioProducto = new ServicioProducto();



    public ProductosAlquiladosVista(string id)


	{
        servicioUsuario.IdUsuario = id;

        // Cambiar el texto del Label directamente desde el código de la vista
        InitializeComponent();

        CargarListaProductos();

    }


    private async void CargarListaProductos()
    {

        circuloCarga.IsRunning = true;

        try
        {
            List<Alquiler> listaProductosAlquilados = await servicioAlquilar.GetAlquileresByUsuarioCompradorYVendedorId(servicioUsuario.IdUsuario);

            ProductosAlquiladosCollectionView.ItemsSource = listaProductosAlquilados;

            foreach (Alquiler item in listaProductosAlquilados)
            {
                if (item.EstadoAlquiler == Estado.Pendiente || item.IdUsuarioVendedor.Equals(servicioUsuario.IdUsuario))
                {
                    await ComprobarProductosAlquilados(item);
                }
            }


            circuloCarga.IsRunning = false;
            // Esto es un texto descriptivo mientras está cargando el circulo
            textoEspera.Text = "";
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al cargar productos: " + ex.Message);
        }
    }


    // Al inicializar la clase, se mostrará el estado en pendiente, el usuario deberá aceptar o cancelar si desea alquilar el producto
    public async Task ComprobarProductosAlquilados(Alquiler item)
    {

        Productos producto = await servicioProducto.ObtenerProductoPorId(item.IdProducto);

        if (item.EstadoAlquiler.Equals(Estado.Pendiente) && item.IdUsuarioVendedor.Equals(servicioUsuario.IdUsuario))
        {
            var result = await DisplayAlert("Éxito", $"¿Alquilar producto?", "OK", "Cancel");
           
            if (result)
            {
                item.EstadoAlquiler = Estado.Aceptado;
                producto.EstaAlquilado = true;
                await servicioUsuario.ActualizarProductoAUsuario(producto);
            }
            else
            {
                item.EstadoAlquiler = Estado.Cancelado;
                
            }

            await servicioAlquilar.InsertOrUpdateAlquiler(item);
        }
    }


}