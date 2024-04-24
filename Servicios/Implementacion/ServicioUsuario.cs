using AlkilaApp.Modelos;
using AlkilaApp.Servicios.Interfaces;
using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Storage;
using Newtonsoft.Json;
using Firebase.Auth;
using AlkilaApp.Servicios;




namespace AlkilaApp
{
    public class ServicioUsuario : IServicioUsuario
    {


        private string _IdUsuario;

        public string IdUsuario
        {
            get => _IdUsuario;
            set
            {
                _IdUsuario = value;
            }
        }


        // Metodo para registrar al usuario
        public async Task RegistroUsuariosAsync(Usuario usaurio)
        {
            // Llamar al servicio de autenticación para crear el usuario
            FirebaseAuthProvider authProvider = new FirebaseAuthProvider(new FirebaseConfig(Setting.FirebaseApiKey));
            var auth = await authProvider.CreateUserWithEmailAndPasswordAsync(usaurio.CorreoElectronico, usaurio.Contrasenya);
            string token = auth.FirebaseToken;

            // Si se ha creado el usuario correctamente
            if (token != null)
            {
                // Obtener el ID único del usuario recién registrado
                this.IdUsuario = auth.User.LocalId;
                // Mostrar un mensaje de éxito y pop la página actual
                await App.Current.MainPage.DisplayAlert("Alert", "El usuario se ha registrado correctamente", "Aceptar");

            }
        }



        // Metodo para validar que el usuario esta dentro de la base de datos de Firebase
        public async Task ValidarUsuariosAsync(string correo, string contrasenya)
        {
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Setting.FirebaseApiKey));
            var auth = await authProvider.SignInWithEmailAndPasswordAsync(correo, contrasenya);
            var content = await auth.GetFreshAuthAsync();
            var serializedContent = JsonConvert.SerializeObject(content);
            Preferences.Set("FreshFirebaseToken", serializedContent);

            // Actualizar el campo IdUsuario del objeto Usuario con el Id del usuario autenticado
            this.IdUsuario = auth.User.LocalId;

        }



        public async Task<bool> AnyadirOActualizarUsuario(Usuario usuario)
        {
            try
            {
                if (this.IdUsuario != null)
                {
                    usuario.IdUsuario = this.IdUsuario;
                    // Crear un nodo en la base de datos de Firebase para almacenar la información del usuario
                    var firebase = new FirebaseClient(Setting.FireBaseDatabaseUrl);
                    var usuarios = firebase.Child("Usuario");
                    await usuarios.Child(this.IdUsuario).PutAsync(usuario);
                    return true;
                }

                return false;
            }
            catch (FirebaseException ex)
            {
                // Manejar la excepción de Firebase
                Console.WriteLine("Error de Firebase: " + ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                // Manejar otras excepciones
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }



        public async Task<Usuario> GetUsuarioRegistrado()
        {
            var firebase = new FirebaseClient(Setting.FireBaseDatabaseUrl);
            return await firebase.Child("Usuario").Child(IdUsuario).OnceSingleAsync<Usuario>();
        }




        // Métodos de la clase ServicioUsuario

        public string LastErrorMessage { get; private set; } // Propiedad para almacenar el último mensaje de error

      

        public async Task<string> AddFoto(Stream imageStream, string fileName)
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



        public async Task<bool> AgregarProductoAUsuario(Productos producto)
        {
            try

            {
                var firebaseClient = new FirebaseClient(Setting.FireBaseDatabaseUrl);

                // Obtener una referencia al nodo del usuario en la base de datos
                var usuarioNode = firebaseClient.Child("Usuario").Child(IdUsuario);

                // Obtener la lista de productos del usuario
                var listaProductos = await usuarioNode.Child("ListaProductos").OnceSingleAsync<List<Productos>>();

                // Si la lista de productos no existe, inicializarla como una nueva lista
                if (listaProductos == null)
                {
                    listaProductos = new List<Productos>();
                }

                // Agregar el nuevo producto a la lista de productos del usuario
                listaProductos.Add(producto);

                // Guardar la lista actualizada de productos en la base de datos
                await usuarioNode.Child("ListaProductos").PutAsync(listaProductos);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al agregar producto al usuario: " + ex.Message);
                return false;
            }
        }


        public async Task<bool> ActualizarProductoAUsuario(Productos producto)
        {
            try
            {
                var firebaseClient = new FirebaseClient(Setting.FireBaseDatabaseUrl);

                // Obtener una referencia al nodo del usuario en la base de datos
                var usuarioNode = firebaseClient.Child("Usuario").Child(IdUsuario);

                // Obtener la lista de productos del usuario
                var listaProductos = await usuarioNode.Child("ListaProductos").OnceSingleAsync<List<Productos>>();

                // Si la lista de productos no existe, inicializarla como una nueva lista
                if (listaProductos == null)
                {
                    listaProductos = new List<Productos>();
                }

                // Buscar si ya existe un producto con el mismo ID en la lista
                var productoExistente = listaProductos.FirstOrDefault(p => p.IdProducto == producto.IdProducto);

                if (productoExistente != null)
                {
                    // Actualizar los datos del producto existente con los datos del nuevo producto
                    productoExistente.Nombre = producto.Nombre;
                    productoExistente.DescripcionProducto = producto.DescripcionProducto;
                    productoExistente.Precio = producto.Precio;
                    productoExistente.Foto = producto.Foto;
                    productoExistente.Tipo = producto.Tipo;
                    productoExistente.EstaAlquilado = producto.EstaAlquilado;



                    // Guardar la lista actualizada de productos en la base de datos
                    await usuarioNode.Child("ListaProductos").PutAsync(listaProductos);

                    return true;
                }

                // controlar else
                return false;

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al agregar producto al usuario: " + ex.Message);
                return false;
            }
          }



        public async Task<List<Productos>> ObtenerListaProductos(string id)
        {
            var listaProductos = new List<Productos>();
            var firebaseClient = new FirebaseClient(Setting.FireBaseDatabaseUrl);
            try
            {
                // Obtener una referencia al nodo del usuario en la base de datos
                var usuarioNode = firebaseClient.Child("Usuario").Child(id);

                // Obtener la lista de productos del usuario
                var productosSnapshot = await usuarioNode.Child("ListaProductos").OnceSingleAsync<List<Productos>>();

                // Agregar los productos directamente a la lista
                listaProductos.AddRange(productosSnapshot);
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción que ocurra
                Console.WriteLine("Error al obtener productos: " + ex.Message);
            }

            return listaProductos;
        }



        public async Task<List<Productos>> ObtenerProductosUsuariosNoLogeados(string idUsuarioActual)
        {
            try
            {
                var firebaseClient = new FirebaseClient(Setting.FireBaseDatabaseUrl);
                // Obtiene una referencia a todos los usuarios en la base de datos
                var usuarios = await firebaseClient.Child("Usuario").OnceAsync<Usuario>();

                // Crea una lista para almacenar los productos de los usuarios
                List<Productos> productosUsuarios = new List<Productos>();

                // Itera sobre todos los usuarios y agrega sus productos a la lista
                foreach (var usuario in usuarios)
                {
                    // Verifica si el usuario es diferente del usuario actual
                    if (usuario.Key != idUsuarioActual)
                    {
                        // Verifica si el usuario tiene productos
                        if (usuario.Object.ListaProductos != null)
                        {
                            // Agrega los productos del usuario a la lista
                            productosUsuarios.AddRange(usuario.Object.ListaProductos);


                        }
                    }
                }

                return productosUsuarios;
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción que pueda ocurrir
                Console.WriteLine("Error al obtener los productos de los usuarios: " + ex.Message);
                return null;
            }
        }



        public async Task<List<Productos>> ObtenerListaProductosPorTipo(TipoProducto tipoProducto, string idUsuarioActual)
        {

            try
            {
                var listaProductos = new List<Productos>();
                listaProductos = await ObtenerProductosUsuariosNoLogeados(idUsuarioActual);

                // Filtrar la lista general de productos por TipoProducto
                listaProductos = listaProductos.Where(p => p.Tipo == tipoProducto).ToList();

                return listaProductos;

            }
            catch (Exception ex)
            {

                Console.WriteLine("Error al obtener los productos de los usuarios: " + ex.Message);
                return null;
            }   
        }


        public async Task<Usuario> ObtenerUsuarioPorIdProducto(string idProducto)
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
                    if (usuario != null && usuario.ListaProductos != null && usuario.ListaProductos.Any(producto => producto.IdProducto == idProducto))
                    {
                        // Si se encuentra un producto con el ID buscado, devuelve el usuario
                        return usuario;
                    }
                }

                // Si no se encuentra ningún usuario con el producto buscado, devuelve null
                return null;
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción que pueda ocurrir
                Console.WriteLine("Error al obtener el usuario por ID de producto: " + ex.Message);
                return null;
            }
        }



        public async Task<List<string>> ObtenerTodosLosIdsUsuarios()
        {
            try
            {
                var firebaseClient = new FirebaseClient(Setting.FireBaseDatabaseUrl);
                var usuarios = await firebaseClient.Child("Usuario").OnceAsync<Usuario>();

                List<string> listaIdsUsuarios = new List<string>();

                foreach (var usuarioEntry in usuarios)
                {
                    listaIdsUsuarios.Add(usuarioEntry.Key);
                }

                return listaIdsUsuarios;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener todos los IDs de los usuarios: " + ex.Message);
                return null;
            }
        }


        public async Task<Usuario> ObtenerUsuarioPorId(string idUsuario)
        {
            try
            {
                var firebaseClient = new FirebaseClient(Setting.FireBaseDatabaseUrl);
                // Obtener una referencia al nodo del usuario por su ID en la base de datos
                var usuarioSnapshot = await firebaseClient.Child("Usuario").Child(idUsuario).OnceSingleAsync<Usuario>();

                // Si se encuentra un usuario con el ID proporcionado, devuelve el usuario
                return usuarioSnapshot;
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción que pueda ocurrir
                Console.WriteLine("Error al obtener el usuario por ID: " + ex.Message);
                return null;
            }
        }



    }
}

