using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App1
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Home : ContentPage
    {
        string apiUrl = "https://gestor-comunitario-backend.vercel.app/api/v1/community/clients";
        public Home()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Verificar si el token existe
            var token = await SecureStorage.GetAsync("jwt_token");

            if (string.IsNullOrEmpty(token))
            {
                // Si el token no existe, navegar a la página de login
                var loginPage = new Login();
                await Navigation.PushModalAsync(new NavigationPage(loginPage));
            }

            getPartners();
        }

        private async void getPartners()
        {
            using (var webClient = new HttpClient())
            {
                try{
                    var token = await SecureStorage.GetAsync("jwt_token");
                    webClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    var resp = webClient.GetStringAsync(apiUrl);
                    resp.Wait();

                    var json = resp.Result;
                    var lib = Newtonsoft.Json.JsonConvert.DeserializeObject<Usuario>(json);

                    txtResp.Text = lib.message;

                    if (lib.data is List<Partner> listaSocios)
                    {
                        listViewPartners.ItemsSource = listaSocios;
                    }else
                    {
                        listViewPartners.ItemsSource = null;
                    }
                    //txtId.Text = "Si funciona";
                }
                catch (Exception ex)
                {
                    txtResp.Text = "Error: " + ex.Message;
                }

                
            }
        }

        private async void cmdNewPartner_Clicked(object sender, EventArgs e)
        {

            Partner partner = new Partner();
            partner.id_cliente = 0;

            var updatePartner = new NewPartner
            {
                title = "Registrar Socio",
                partner = partner
            };

            await Navigation.PushAsync(updatePartner);
        }

        private DateTime lastClicked = DateTime.MinValue;

        private void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            var currentTime = DateTime.Now;
            var timeSinceLastClick = currentTime - lastClicked;
            if (timeSinceLastClick.TotalMilliseconds < 500) // Se considera un doble clic si es dentro de medio segundo
            {
                // Doble clic detectado
                lastClicked = DateTime.MinValue; // Restablece el tiempo del último clic
                var selectedPartner = e.Item as Partner;
                if (selectedPartner != null)
                {
                    // Llama al método "Ver" pasando el objeto seleccionado
                    //cmdView_Clicked(selectedPartner, EventArgs.Empty);
                    txtResp.Text = "Activado";

                }
            }
            else
            {
                // Guarda el tiempo del último clic para el próximo tap
                lastClicked = currentTime;
            }

            // Desactiva la selección del elemento de la ListView
            ((ListView)sender).SelectedItem = null;
        }


        private async void cmdView_Clicked(object sender, EventArgs e) {
            Button button = sender as Button;
            int id_cliente = (int)button.CommandParameter;
            using (var webClient = new HttpClient())
            {
                try
                {
                    Partner socio = await GetPartnerById(id_cliente);
                    string deudasMensaje = "Total Deudas: " + socio.deudas.Count + "\n\n";

                    if (socio.deudas.Count > 0)
                    {
                        deudasMensaje += "      Detalles de Deudas      \n\n" +
                                         "-----------------------------\n";

                        foreach (var deuda in socio.deudas)
                        {
                            deudasMensaje += $"Tipo de Deuda: {deuda.tipo}\n" +
                                             $"Nombre: {deuda.nombre}\n" +
                                             $"Monto: {deuda.monto}\n" +
                                             $"Fecha: {deuda.fecha}\n" +
                                             "-----------------------------\n";
                        }
                    }

                    string mensaje = $"Socio: {socio.nombres} {socio.apellidos}\n" +
                                     $"{deudasMensaje}";

                    await DisplayAlert("Deudas", mensaje, "Aceptar");
                }
                catch (Exception ex)
                {
                    txtResp.Text = "Error: " + ex.Message;
                }
            }
            
        }

        private async void cmdPayments_Clicked(object sender, EventArgs e)
        {
            Button button = sender as Button;
            int id_cliente = (int)button.CommandParameter;
            Partner selectedPartner = await GetPartnerById(id_cliente);
            var paymentsPage = new Payments
            {
                partner = selectedPartner,
            };

            await Navigation.PushAsync(paymentsPage);
        }

        private async void cmdUpdate_Clicked(object sender, EventArgs e)
        {
            Button button = sender as Button;
            int id_cliente = (int)button.CommandParameter;

            Partner selectedPartner = await GetPartnerById(id_cliente);

            var updatePartner = new NewPartner();
            updatePartner.title = "Actualizar Datos Socio";
            updatePartner.partner = selectedPartner;

            await Navigation.PushAsync(updatePartner);           
        }

        private async Task<Partner> GetPartnerById(int id_cliente)
        {
            try
            {
                using (var webClient = new HttpClient())
                {
                    var token = await SecureStorage.GetAsync("jwt_token");
                    webClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    var url = $"{apiUrl}/client/{id_cliente}";
                    var resp = webClient.GetStringAsync(url);
                    resp.Wait();

                    var json = resp.Result;
                    var lib = Newtonsoft.Json.JsonConvert.DeserializeObject<Usuario>(json);

                    txtResp.Text = lib.message;

                    if (lib.data is List<Partner> listaSocios)
                    {
                        //listViewPartners.ItemsSource = listaSocios;
                        return listaSocios[0];
                    }

                    return null;
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones: Puedes registrar el error o manejarlo según tus necesidades
                Console.WriteLine($"Error en GetPartnerByIdAsync: {ex.Message}");
                return null;
            }
        }

        private async void cmdDelete_Clicked(object sender, EventArgs e) {
            Button button = sender as Button;
            int id_cliente = (int)button.CommandParameter;
            using (var webClient = new HttpClient())
            {
                try
                {
                    var token = await SecureStorage.GetAsync("jwt_token");
                    webClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    var url = $"{apiUrl}/{id_cliente}";
                   
                    var resp = await webClient.DeleteAsync(url);

                    resp.EnsureSuccessStatusCode();

                    var json = await resp.Content.ReadAsStringAsync();
                    var lib = Newtonsoft.Json.JsonConvert.DeserializeObject<Usuario>(json);

                    txtResp.Text = lib.message;

                    if (lib.status.Equals("Ok"))
                    {
                        await DisplayAlert("Acción", "Socio Eliminado", "Aceptar");
                        getPartners();
                    }

                }
                catch (Exception ex)
                {
                    txtResp.Text = "Error: " + ex.Message;
                }
            }
        }

    }
}