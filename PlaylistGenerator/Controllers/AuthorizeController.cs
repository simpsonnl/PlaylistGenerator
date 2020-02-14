using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PlaylistGenerator.Controllers
{
    public class AuthorizeController : Controller
    {
        class SpotifyAuthentication
        {
            public string clientID = "52c0f5ab6e5f4a2f83da6c5fad1c6bac";
            public string clientSecret = "6e2bea1292b845b392725811ae29b026";
            public string redirectURL = "https://localhost:44385/Authorize/Callback/";
        }

        SpotifyAuthentication sAuth = new SpotifyAuthentication();

        [HttpPost]
        public async Task<ActionResult> Auth()
        {
            TokenSwapWebAPIFactory webApiFactory;
            SpotifyWebAPI spotify;

            // You should store a reference to WebAPIFactory if you are using AutoRefresh or want to manually refresh it later on. New WebAPIFactory objects cannot refresh SpotifyWebAPI object that they did not give to you.
            webApiFactory = new TokenSwapWebAPIFactory("https://frozen-chamber-35968.herokuapp.com/swap")
            {
                Scope = Scope.UserReadPrivate | Scope.UserReadEmail | Scope.PlaylistReadPrivate,
                AutoRefresh = true

            };
            // You may want to react to being able to use the Spotify service.
            // webApiFactory.OnAuthSuccess += (sender, e) => authorized = true;
            // You may want to react to your user's access expiring.
            // webApiFactory.OnAccessTokenExpired += (sender, e) => authorized = false;

            try
            {
                spotify = await webApiFactory.GetWebApiAsync();
                PrivateProfile profile = spotify.GetPrivateProfile();
                // Synchronous way:
                // spotify = webApiFactory.GetWebApiAsync().Result;
            }
            catch (Exception ex)
            {
                // Example way to handle error reporting gracefully with your SpotifyWebAPI wrapper
                // UpdateStatus($"Spotify failed to load: {ex.Message}");
            }
            return null;

            //AuthorizationCodeAuth auth = new AuthorizationCodeAuth(
            // "52c0f5ab6e5f4a2f83da6c5fad1c6bac",
            // "6e2bea1292b845b392725811ae29b026",
            // "https://localhost:44385/",
            // "https://localhost:44385",
            // Scope.PlaylistReadPrivate | Scope.PlaylistReadCollaborative
            //  );

            //auth.AuthReceived += async (sender, payload) =>
            //{
            //    auth.Stop();
            //    Token token = await auth.ExchangeCode(payload.Code);
            //    SpotifyWebAPI api = new SpotifyWebAPI()
            //    {
            //        TokenType = token.TokenType,
            //        AccessToken = token.AccessToken
            //    };
            //    // Do requests with API client
            //    Console.WriteLine(api.AccessToken);
            //};
            //auth.Start(); // Starts an internal HTTP Server
            //auth.OpenBrowser();




            //var my_client_id = "52c0f5ab6e5f4a2f83da6c5fad1c6bac";
            //var scopes = "user-read-private user-read-email";
            //return Redirect("https://accounts.spotify.com/authorize" +
            //          "?response_type=code" +
            //          "&client_id=" + my_client_id +
            //          "&scope=" + scopes +
            //          "&redirect_uri=" + "https://localhost:44385/Authorize/Callback/");

        }

        public ActionResult Callback(string code)
        {
            string responseString = "";

            if (code.Length > 0)
            {
                using (HttpClient client = new HttpClient())
                {
                    Console.WriteLine(Environment.NewLine + "Your basic bearer: " + Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(sAuth.clientID + ":" + sAuth.clientSecret)));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(sAuth.clientID + ":" + sAuth.clientSecret)));

                    FormUrlEncodedContent formContent = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("code", code),
                        new KeyValuePair<string, string>("redirect_uri", sAuth.redirectURL),
                        new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    });

                    var response = client.PostAsync("https://accounts.spotify.com/api/token", formContent).Result;

                    

                    var responseContent = response.Content;
                    responseString = responseContent.ReadAsStringAsync().Result;
                    
                }
            }

            var j = Json(responseString, JsonRequestBehavior.AllowGet);
            
            return RedirectToAction("Index", "Home");
            //return new JsonResult
            //{
            //    ContentType = "application/json",
            //    Data = responseString,
            //    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                
            //};
            
        }
    }
}