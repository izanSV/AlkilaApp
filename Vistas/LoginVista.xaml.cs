
namespace AlkilaApp
{
    public partial class LoginVista : ContentPage
    {
        public LoginVista()
        {
            InitializeComponent();

            BindingContext = new LoginVistaModelo(Navigation);
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
