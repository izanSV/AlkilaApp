﻿using AlkilaApp.Modelos;
using Microsoft.Maui.Controls;
using System.ComponentModel;



namespace AlkilaApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AlquilarProducto : ContentPage, INotifyPropertyChanged


    {

        private ServicioAlquiler servicioAlquilar = new ServicioAlquiler();

        private ServicioUsuario servicioUsuario = new ServicioUsuario();

        private Productos producto = new Productos();


        // TODO: Obtener alquiler por id
        private Alquiler alquiler = new Alquiler();


        public event PropertyChangedEventHandler? PropertyChanged;


        public AlquilarProducto(string? id, Usuario usuario, Productos pro)
        {

            // obtenemos los datos temporales del producto con el usuario comprador y vendedor
            InitializeComponent();

            // El botón de los terminos y condiciones, al iniciar la app, quiero que este inactivo, al presionar cambiará el estado
            imgBtnGuardar.IsEnabled = false;
            checkBox.IsEnabled = false;
            btnLeerCondicion.IsEnabled = false;

            servicioUsuario.IdUsuario = id;
            alquiler.IdUsuarioComprador = id;
            alquiler.IdUsuarioVendedor = usuario.IdUsuario;
            alquiler.IdProducto = pro.IdProducto;
            alquiler.NombreProductoAlquilado = pro.Nombre;
            alquiler.FotoProductoAlquilado = pro.Foto;
            alquiler.NombreUsuarioComprador = usuario.Nombre;

            //obtenemos los datos del producto obtenido por el usuario
            producto = pro;

            // Suscribirse al evento DateSelected para los DatePickers
            FechaInicio.DateSelected += FechaInicio_DateSelected;
            FechaFin.DateSelected += FechaFin_DateSelected;


        }


        public int CalcularFecha()
        {
            // Obtener las fechas seleccionadas
            DateTime fechaRecogida = FechaInicio.Date;
            DateTime fechaDevolucion = FechaFin.Date;

            // Calcular la diferencia en días
            TimeSpan diferencia = fechaDevolucion - fechaRecogida;
            int diasDiferencia = diferencia.Days;
            return diasDiferencia;
        }





        // Si no activa el checkBox no podrá alquilar el producto

        private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {

            if (e.Value) // Si el CheckBox está activado
            {

                imgBtnGuardar.IsEnabled = true;

            }
            else
            {
                imgBtnGuardar.IsEnabled = false;

            }
        }


        // Botones

        private async void GuardarDatosClicked(object sender, EventArgs e)
        {

            await animaacionButton(sender, e);

            // Almacenamos los datos del alquiler
            DateTime fechaActual = DateTime.Now.Date;
            DateTime fechaInicio = FechaInicio.Date;
            DateTime fechaFin = FechaFin.Date;
            alquiler.EstadoAlquiler = Estado.Pendiente;
            alquiler.IdAlquiler = Guid.NewGuid().ToString();

            if (fechaInicio < fechaActual || fechaFin < fechaActual)
            {
                await DisplayAlert("Error", "No se puede seleccionar una fecha anterior al día actual", "Aceptar");
                return;
            }

            if (fechaInicio > fechaFin)
            {
                await DisplayAlert("Error", "La fecha de devolución no puede ser anterior a la fecha de recogida", "Aceptar");
                return;
            }

            int diasAlquiler = CalcularFecha();

            if (diasAlquiler <= 0)
            {
                await DisplayAlert("Error", "La duración del alquiler debe ser de al menos un día", "Aceptar");
                return;
            }
            else
            {
                alquiler.FechaInicio = FechaInicio.Date;
                alquiler.FechaFin = FechaFin.Date;
               

                await servicioAlquilar.InsertOrUpdateAlquiler(alquiler);

                //volver a la pagina donde estan los alquileres
                await DisplayAlert("Información", "Recibiras un mensaje cuando el usuario haya visto tu propuesta", "Aceptar");
                await Navigation.PushAsync(new VistaProductos(servicioUsuario.IdUsuario));

            }
        }



        private void FechaInicio_DateSelected(object sender, DateChangedEventArgs e)
        {
            btnRealizarCalculo(sender, e);
        }

        private void FechaFin_DateSelected(object sender, DateChangedEventArgs e)
        {
            btnRealizarCalculo(sender, e);
        }


        private async void CancelarDatosClicked(object sender, EventArgs e)
        {
            await animaacionButton(sender, e);
            await Navigation.PopAsync();

        }


        private async void btnRealizarCalculo(object sender, EventArgs e)
        
        {
            btnLeerCondicion.IsEnabled = true;

            // Obtener la cantidad de días de alquiler, por ejemplo, llamando a un método para calcular la diferencia entre las fechas de inicio y fin
            int diasAlquiler = CalcularFecha();

            // Calcular el costo del alquiler
            double costoAlquiler = CalcularCostoAlquiler(diasAlquiler);

            // Mostrar el costo del alquiler en la etiqueta
            CostoAlquilerLabel.Text = $"{costoAlquiler} €";
            DiasTotalesLabel.Text = $"☀️ Dias totales: {diasAlquiler}";
           
            // almacenamos el precio total
            alquiler.PrecioTotal = costoAlquiler;

        }


        private double CalcularCostoAlquiler(int diasAlquiler)
        {

            // El calculo es sobre el precio base

            double tarifaDiaria = producto.Precio; // Tarifa diaria fija en euros
            double costoTotal = 0.0;

            if (diasAlquiler <= 3)
            {
                // Si el alquiler es igual o menor a 3 días, se aplica la tarifa diaria estándar
                costoTotal = diasAlquiler * tarifaDiaria;
            }
            else
            {
                // Si el alquiler es mayor a 3 días, se aplica un descuento del 20% a partir del quarto día
                int diasRestantesDespuesDeUnaSemana = diasAlquiler - 3;
                double costoPrimeraSemana = 3 * tarifaDiaria;
                double costoDespuesDeUnaSemana = diasRestantesDespuesDeUnaSemana * (tarifaDiaria * 0.8); // Aplicar descuento del 20%
                costoTotal = costoPrimeraSemana + costoDespuesDeUnaSemana;
            }

            // Redondear el costo total a dos decimales
            costoTotal = Math.Round(costoTotal, 2);

            return costoTotal;
        }

        private async void btnLeerCondiciones(object sender, EventArgs e)
        {
           
            await DisplayAlert("Términos y Condiciones de Uso", "Bienvenido a [AlkilaApp.inc].\r\n\r\nPor favor, lee estos términos y condiciones cuidadosamente antes de utilizar nuestra aplicación/servicio.\r\n\r\nAceptación de los Términos\r\n\r\nAl acceder o utilizar la aplicación/servicio de cualquier manera, aceptas estar sujeto a estos términos y condiciones. Si no estás de acuerdo con alguno de estos términos, no utilices la aplicación/servicio.\r\n\r\nUso del Servicio\r\n\r\nLa aplicación/servicio y su contenido son propiedad de [Nombre de la Empresa] y están protegidos por las leyes de derechos de autor correspondientes. Estás autorizado a utilizar la aplicación/servicio solo con fines personales y no comerciales.\r\n\r\nPrivacidad\r\n\r\nTu privacidad es importante para nosotros. Consulta nuestra Política de Privacidad para comprender cómo recopilamos, utilizamos y protegemos tu información personal.\r\n\r\nContenido del Usuario\r\n\r\nAl utilizar la aplicación/servicio, puedes proporcionar cierta información, como comentarios, opiniones, etc. Al hacerlo, otorgas a [Nombre de la Empresa] una licencia no exclusiva, transferible, sublicenciable, libre de regalías para utilizar, reproducir, modificar, adaptar, publicar, traducir y distribuir dicho contenido en cualquier forma, medio o tecnología.\r\n\r\nResponsabilidades del Usuario\r\n\r\nEres responsable de mantener la confidencialidad de tu cuenta y contraseña, así como de restringir el acceso a tu dispositivo. Eres responsable de todas las actividades que ocurran bajo tu cuenta.\r\n\r\nModificaciones\r\n\r\nNos reservamos el derecho de modificar o revisar estos términos y condiciones en cualquier momento. Te notificaremos cualquier cambio mediante la publicación de los términos y condiciones actualizados en la aplicación/servicio. El uso continuado de la aplicación/servicio después de dichos cambios constituye tu aceptación de los términos y condiciones revisados.\r\n\r\nTerminación\r\n\r\nNos reservamos el derecho de suspender o dar por terminado tu acceso a la aplicación/servicio en cualquier momento y por cualquier motivo sin previo aviso.\r\n\r\nContacto\r\n\r\nSi tienes alguna pregunta sobre estos términos y condiciones, contáctanos en [Correo Electrónico de Contacto].\r\n\r\nÚltima actualización: [Fecha de la última actualización]", "Aceptar");

            checkBox.IsEnabled = true;

        }


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

    }
}
