using AlkilaApp.VistaModelo;


namespace AlkilaApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegistroVista : ContentPage
    {
        public RegistroVista(bool esEmpresa)
        {
            try
            {
                InitializeComponent();
                dominioEmpresa(esEmpresa); // Llama al método essEmpresa con el valor de esEmpresa
                BindingContext = new RegistroVistaModelo(Navigation, dominioEmpresa(esEmpresa));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }


        //TODO HAcer estatico
        public string dominioEmpresa( bool esEmpresa)
        {

            string dominio = "";

            try
            {

                if (esEmpresa == true)
                {
                    dominio =(CorreoEntry.Text = "@enterprise.com");

                }
                else
                {
                  dominio = (CorreoEntry.Text = "@alkila.com");

                }

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
            }
            return dominio;

        }


    }
}