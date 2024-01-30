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
    public partial class Login : ContentPage
    {
        string apiUrl = "https://gestor-comunitario-backend.vercel.app/api/v1/login";
        public Login()
        {
            InitializeComponent();
        }

        private async void cmdLogin_Clicked(object sender, EventArgs e)
        {
            using (var webClient = new HttpClient())
            {
                webClient.BaseAddress = new Uri(apiUrl);
                webClient
                    .DefaultRequestHeaders
                    .Accept
                    .Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(new Usuario
                {
                    usuario = txtUser.Text,
                    password = txtPassword.Text
                });

                var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                var resp = webClient.SendAsync(request);
                resp.Wait();

                if (resp.Result.IsSuccessStatusCode)
                {
                    json = resp.Result.Content.ReadAsStringAsync().Result;
                    var usuario = JsonConvert.DeserializeObject<Usuario>(json);

                    if (usuario.token != null)
                    {
                        await SecureStorage.SetAsync("jwt_token", usuario.token);

                        var homePage = new Home();
                        await Navigation.PushAsync(homePage);
                    }
                    else
                    {

                        lblTexto.Text = usuario.message;
                    }
                }
                else
                {
                    lblTexto.Text = "Error en el inicio de sesión";
                }
            }
        }

        private async void cmdRegesar_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}