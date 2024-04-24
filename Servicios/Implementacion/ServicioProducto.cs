using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Storage;
using AlkilaApp.Modelos;
using AlkilaApp.Servicios.Interfaces;




namespace AlkilaApp.Servicios
{
    public class ServicioProducto : IProducto
    {

        FirebaseClient firebaseClient;
        private string _IdProducto;

        public string IdUsuario
        {
            get => _IdProducto;
            set
            {
                _IdProducto = value;
            }
        }


        public ServicioProducto()
        {
            // Inicializar el cliente Firebase con la URL de la base de datos
            firebaseClient = new FirebaseClient(Setting.FireBaseDatabaseUrl);
        }



        public async Task<bool> AnyadirOActualizarProducto(Productos producto)
        {
            try
            {
                // Crear un nodo en la base de datos de Firebase para almacenar la información del producto
                await firebaseClient.Child("Productos").PostAsync(producto);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al añadir o actualizar el producto: " + ex.Message);
                return false;
            }
        }


        public async Task<Productos> GetProductosRegistrados()
        {

            var firebase = new FirebaseClient(Setting.FireBaseDatabaseUrl);
            return await firebase.Child("Productos").Child(_IdProducto).OnceSingleAsync<Productos>();
        }


        public async Task<string> AddFoto(MemoryStream imageStream, string fileName)
        {
            try
            {
                // Crear una referencia a la raíz de Firebase Storage
                var firebaseStorage = new FirebaseStorage(Setting.FirebaseStoreBucket);

                // Subir la imagen a Firebase Storage en el directorio raíz del bucket con el nombre del archivo
                var imageUrl = await firebaseStorage
                    .Child(fileName)   // Nombre del archivo (sin incluir la ruta al directorio)
                    .PutAsync(imageStream);

                Console.WriteLine("URL de la imagen generada: " + imageUrl);

                // Si la carga fue exitosa, imageUrl contiene la URL de la imagen almacenada en Firebase Storage
                return imageUrl;
            }
            catch (FirebaseStorageException ex)
            {
                // Almacenar el mensaje de error específico en la propiedad LastErrorMessage
                LastErrorMessage = "Error de Firebase Storage: " + ex.Message;
                Console.WriteLine(LastErrorMessage); // Imprimir el mensaje de error en la consola
                return null;
            }
            catch (Exception ex)
            {
                // Almacenar el mensaje de error genérico en la propiedad LastErrorMessage
                LastErrorMessage = "Error: " + ex.Message;
                Console.WriteLine(LastErrorMessage); // Imprimir el mensaje de error en la consola
                return null;
            }

            
    }


    public string LastErrorMessage { get; private set; } // Propiedad para almacenar el último mensaje de error



        FirebaseClient firebase = new FirebaseClient(Setting.FireBaseDatabaseUrl, new FirebaseOptions

        {
            AuthTokenAsyncFactory = () => Task.FromResult(Setting.FireBaseSeceret)
        });

        
        public async Task<List<Productos>> ObtenerProductos()
        {
            return (await firebase.Child(nameof(Productos)).OnceAsync<Productos>()).Select(f => new Productos

            {
   
            }).ToList();
        }



        public async Task<bool> AddProducto(Productos producto)
        {
            try
            {
                var response = await firebase.Child(nameof(Productos)).PostAsync(producto);
                if (response.Key != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al agregar producto: " + ex.Message);
                return false;
            }
        }




        public async Task<bool> AnyadirOActualizarProductos(Productos employeeModel)
        {

            if (!string.IsNullOrWhiteSpace(""))
            {
                try
                {
                    await firebase.Child(nameof(Productos)).Child("").PutAsync(employeeModel);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            else
            {
                var response = await firebase.Child(nameof(Productos)).PostAsync(employeeModel);
                if (response.Key != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }



        public async Task<Productos> ObtenerProductoPorId(string idProducto)
        {
            try
            {
                var firebaseClient = new FirebaseClient(Setting.FireBaseDatabaseUrl);
                // Obtiene una referencia a todos los usuarios en la base de datos
                var usuarios = await firebaseClient.Child("Usuario").OnceAsync<Usuario>();

                // Itera sobre todos los usuarios para encontrar el que tiene el producto con el ID buscado
                foreach (var usuarioEntry in usuarios)
                {
                    var usuarioDic = usuarioEntry.Object; // Obtiene el diccionario del usuario
                    var usuario = usuarioDic; // Obtiene el usuario del diccionario

                    // Verifica si el usuario tiene una lista de productos y si alguno de los productos tiene el ID buscado
                    if (usuario != null && usuario.ListaProductos != null)
                    {
                        // Busca el producto por su ID en la lista de productos del usuario
                        var producto = usuario.ListaProductos.FirstOrDefault(p => p.IdProducto == idProducto);

                        // Si se encuentra el producto, lo devuelve
                        if (producto != null)
                        {
                            return producto;
                        }
                    }
                }

                // Si no se encuentra ningún producto con el ID buscado, devuelve null
                return null;
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción que pueda ocurrir
                Console.WriteLine("Error al obtener el producto por ID: " + ex.Message);
                return null;
            }
        }







        public async Task<bool> EliminarProductoPorId(string idProducto)
        {
            try
            {
                var firebaseClient = new FirebaseClient(Setting.FireBaseDatabaseUrl);

                // Obtiene una referencia a todos los usuarios en la base de datos
                var usuarios = await firebaseClient.Child("Usuario").OnceAsync<Usuario>();

                // Itera sobre todos los usuarios para encontrar el que tiene el producto con el ID buscado
                foreach (var usuarioEntry in usuarios)
                {
                    var usuarioDic = usuarioEntry.Object; // Obtiene el diccionario del usuario
                    var usuario = usuarioDic; // Obtiene el usuario del diccionario

                    // Verifica si el usuario tiene una lista de productos y si alguno de los productos tiene el ID buscado
                    if (usuario != null && usuario.ListaProductos != null)
                    {
                        // Busca el producto por su ID en la lista de productos del usuario
                        var producto = usuario.ListaProductos.FirstOrDefault(p => p.IdProducto == idProducto);

                        // Si se encuentra el producto, lo elimina de la lista de productos del usuario
                        if (producto != null)
                        {
                            usuario.ListaProductos.Remove(producto);

                            // Actualiza el usuario en la base de datos Firebase
                            await firebaseClient.Child("Usuario").Child(usuarioEntry.Key).PutAsync(usuario);

                            // Devuelve true para indicar que se ha eliminado el producto exitosamente
                            return true;
                        }
                    }
                }

                // Si no se encuentra ningún producto con el ID buscado, devuelve false
                return false;
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción que pueda ocurrir
                Console.WriteLine("Error al eliminar el producto por ID: " + ex.Message);
                return false;
            }
        }





    }

}
