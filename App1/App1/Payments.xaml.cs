using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App1
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Payments : ContentPage
    {
        public Partner partner { get; set; }
        
        string apiUrl = "https://gestor-comunitario-backend.vercel.app/api/v1/community";
        public Payments()
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

            getPayments(partner.id_cliente);
        }

        private async void getPayments(int id_cliente)
        {
            using (var webClient = new HttpClient())
            {
                try
                {
                    var token = await SecureStorage.GetAsync("jwt_token");
                    webClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    var url = $"{apiUrl}/clients/{id_cliente}/receipts";
                    var resp = webClient.GetStringAsync(url);
                    resp.Wait();

                    var json = resp.Result;
                    var lib = Newtonsoft.Json.JsonConvert.DeserializeObject<Sensor>(json);

                    txtResp.Text = lib.message;

                    if (lib.data is List<Payment> listaPagos)
                    {
                        listViewPayments.ItemsSource = listaPagos;
                    }
                    else
                    {
                        listViewPayments.ItemsSource = null;
                    }
                }
                catch (Exception ex)
                {
                    txtResp.Text = "Error: " + ex.Message;
                }


            }
        }

        private async void cmdView_Clicked(object sender, EventArgs e)
        {
            Button button = sender as Button;
            int id_cobr = (int)button.CommandParameter;
            using (var webClient = new HttpClient())
            {
                try
                {
                    Payment pago = await GetPaymentById(id_cobr);
                    if (pago != null)
                    {
                        string deudasMensaje = "";

                        if (pago.detalles.Count > 0)
                        {
                            deudasMensaje += "      Detalles del Pago      \n\n" +
                                             "-----------------------------\n";

                            foreach (var detail in pago.detalles)
                            {
                                deudasMensaje += $"Detalle: {detail.detalle}\n" +
                                                 $"Monto: {detail.monto}\n" +
                                                 "-----------------------------\n";
                            }

                            deudasMensaje += $"Total Pago: {pago.total}\n";
                        }

                        string mensaje = $"Número de Factura: {pago.numero_factura}\n" +
                                         $"Concepto: {pago.concepto}\n" +
                                         $"Fecha de Pago: {pago.fecha_cobro}\n\n" +
                                         $"{deudasMensaje}";

                        await DisplayAlert("Detalle Pago", mensaje, "Aceptar");
                    }
                    else
                    {
                        // Handle the case where GetPaymentById returns null
                        txtResp.Text = "Error: Pago no encontrado";
                    }
                    }
                catch (Exception ex)
                {
                    txtResp.Text = "Error: " + ex.Message;
                }
            }

        }

        private async Task<Payment> GetPaymentById(int id_cobro)
        {
            try
            {
                using (var webClient = new HttpClient())
                {
                    var token = await SecureStorage.GetAsync("jwt_token");
                    webClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    var url = $"{apiUrl}/receipts/{id_cobro}";
                    var resp = webClient.GetStringAsync(url);
                    resp.Wait();

                    var json = resp.Result;
                    var lib = Newtonsoft.Json.JsonConvert.DeserializeObject<Sensor>(json);

                    //if (lib.status.Equals("Error"))
                    //{
                    //    txtResp.Text = lib.message;
                    //}

                    if (lib.data is List<Payment> pago)
                    {
                        return pago[0];
                    }

                    return null;
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones: Puedes registrar el error o manejarlo según tus necesidades
                Console.WriteLine($"Error en GetPaymentById: {ex.Message}");
                return null;
            }
        }
    }
}