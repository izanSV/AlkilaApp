using AlkilaApp.Modelos;
using AlkilaApp.Servicios;
using Firebase.Database;
using Firebase.Database.Query;

namespace AlkilaApp
{
    public class ServicioAlquiler
    {

        FirebaseClient firebase;

        public ServicioAlquiler()
        {
            firebase = new FirebaseClient(Setting.FireBaseDatabaseUrl);
        }

        public async Task<bool> InsertOrUpdateAlquiler(Alquiler alquiler)
        {
            try
            {
                if (alquiler.IdAlquiler != null)
                {
                    await firebase.Child("Alquiler").Child(alquiler.IdAlquiler).PutAsync(alquiler);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }


        public async Task<List<Alquiler>> GetAlquileresByUsuarioCompradorYVendedorId(string idUsuario)
        {
            try
            {
                // Obtener todos los alquileres
                var alquileres = await firebase.Child("Alquiler").OnceAsync<Alquiler>();

                // Filtrar alquileres que coincidan con el idUsuarioComprador o el idUsuarioVendedor
                var alquileresCoincidentes = alquileres
                    .Select(alquiler => alquiler.Object)
                    .Where(alquiler => alquiler.IdUsuarioComprador == idUsuario || alquiler.IdUsuarioVendedor == idUsuario)
                    .ToList();

                // Si no se encontraron alquileres coincidentes
                if (alquileresCoincidentes.Count == 0)
                {
                    Console.WriteLine("No se encontraron alquileres con el IdUsuarioComprador especificado.");
                }

                return alquileresCoincidentes;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }


    }
}
