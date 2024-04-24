using AlkilaApp.Modelos;
using AlkilaApp.Servicios;
using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.Maui.Storage;

namespace AlkilaApp
{
    public class ServicioUbicacion
    {

        private FirebaseClient firebase;

        public ServicioUbicacion()
        {
            firebase = new FirebaseClient(Setting.FireBaseDatabaseUrl, new FirebaseOptions
            {
                AuthTokenAsyncFactory = () => Task.FromResult(Setting.FireBaseSeceret)
            });
        }


        public async Task<bool> InsertOrUpdateUbicacion(Ubicacion ubicacion)
        {
            try
            {
                if (ubicacion.IdUsuario != null)
                {
                    await firebase.Child("Ubicacion").Child(ubicacion.IdUsuario).PutAsync(ubicacion);
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


        public async Task<Ubicacion> GetLocationAsync(string idUsuario)
        {
            try
            {
                var location = await firebase.Child("Ubicacion").Child(idUsuario).OnceSingleAsync<Ubicacion>();
                return location;
            }
            catch (Exception ex)
            {
                // Manejar excepciones aquí
                Console.WriteLine($"Error al obtener la ubicación: {ex.Message}");
                return null;
            }
        }
    }
}
