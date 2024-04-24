using AlkilaApp.Modelos;

namespace AlkilaApp.Servicios.Interfaces
{
    public interface IProducto
    {
        Task<List<Productos>> ObtenerProductos();

        Task<bool> AnyadirOActualizarProductos(Productos employeeModel);

    }
}
