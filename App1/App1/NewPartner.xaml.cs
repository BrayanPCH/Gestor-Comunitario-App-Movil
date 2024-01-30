using Newtonsoft.Json;
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
    public partial class NewPartner : ContentPage
    {
        public string title { get; set; }
        public Partner partner { get; set; }
        public bool mostrar { get; set; }

        string apiUrl = "https://gestor-comunitario-backend.vercel.app/api/v1/community/clients";
        
        public NewPartner()
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

            lblTexto.Text = title ?? string.Empty;

            txtNombres.Text = partner.nombres ?? string.Empty;
            txtApellidos.Text = partner.apellidos ?? string.Empty;
            txtCorreo.Text = partner?.correo ?? string.Empty;
            txtTelefono.Text = partner?.telefono ?? string.Empty;

            if (partner.id_cliente != 0)
            {
                txtCedula.IsVisible = false;
            }
        }

        public async void cmdAccept_Clicked(object sender, EventArgs e)
        {
            if (partner.id_cliente != 0)
            {
                bool result = await DisplayAlert("¿Actualizar Socio?", null, "Aceptar", "Cancelar");

                if (result)
                {
                    // El usuario hizo clic en "Aceptar", ahora puedes acceder a los valores ingresados
                    partner.nombres = txtNombres.Text;
                    partner.apellidos = txtApellidos.Text;
                    partner.correo = txtCorreo.Text;
                    partner.telefono = txtTelefono.Text;

                    UpdatePartner(partner);

                }
            }
            else
            {
                partner.cedula = txtCedula.Text;
                partner.nombres = txtNombres.Text;
                partner.apellidos = txtApellidos.Text;
                partner.correo = txtCorreo.Text;
                partner.telefono = txtTelefono.Text;

                RegisterPartner(partner);
            }
        }

        public void cmdCancel_Clicked(object sender, EventArgs e)
        {
            Regresar();
        }

        public async void Regresar()
        {
            var previusPage = new Home();
            await Navigation.PushAsync(previusPage);
        }

        private async void RegisterPartner(Partner partner)
        {
            try {
                using (var webClient = new HttpClient())
                {
                    var token = await SecureStorage.GetAsync("jwt_token");
                    webClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    webClient.BaseAddress = new Uri(apiUrl);
                    webClient
                        .DefaultRequestHeaders
                        .Accept
                        .Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(partner);

                    var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                    var resp = await webClient.SendAsync(request);
                    resp.EnsureSuccessStatusCode();

                    var result = await resp.Content.ReadAsStringAsync();
                    var lib = JsonConvert.DeserializeObject<Usuario>(result);

                    if (lib.status.Equals("Ok"))
                    {
                        await DisplayAlert("Registro exitoso", "El socio ha sido registrado", "Aceptar");
                        Regresar();
                    }
                    else
                    {
                        lblResp.Text = lib.message;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en RegisterPartner: {ex.Message}");
            }
        }

        private async void UpdatePartner(Partner partner)
        {
            try
            {
                using (var webClient = new HttpClient())
                {
                    var token = await SecureStorage.GetAsync("jwt_token");
                    webClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    webClient.BaseAddress = new Uri(apiUrl);
                    webClient
                        .DefaultRequestHeaders
                        .Accept
                        .Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                    var json = JsonConvert.SerializeObject(partner);

                    var rqst = new HttpRequestMessage(HttpMethod.Put, apiUrl);
                    rqst.Content = new StringContent(json, Encoding.UTF8, "application/json");

                    var resp = await webClient.SendAsync(rqst);
                    resp.EnsureSuccessStatusCode();

                    var result = await resp.Content.ReadAsStringAsync();
                    var lib = Newtonsoft.Json.JsonConvert.DeserializeObject<Usuario>(result);

                    if (lib.status.Equals("Ok"))
                    {
                        await DisplayAlert("Actualización exitosa", "El socio ha sido actualizado", "Aceptar");

                        Regresar();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en UpdatePartner: {ex.Message}");
            }
        }
    }
}