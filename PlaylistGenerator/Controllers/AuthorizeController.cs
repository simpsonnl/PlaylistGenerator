using PlaylistGenerator.Models;
using PlaylistGenerator.ViewModels;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace PlaylistGenerator.Controllers
{
    public class AuthorizeController : Controller
    {
        private string _clientId = "52c0f5ab6e5f4a2f83da6c5fad1c6bac";
        private string _secretId = "a66d0d708a1f49789372f50a65d2b3cc";
        //private string _redirectURL = "https://localhost:44385/Authorize/Callback/";
        //private string _redirectURLFromCreate = "https://localhost:44385/Authorize/CallbackFromCreate/";
        //private string _serverURI = "https://localhost:44385/";
        private string _redirectURL = "https://spotifyplaylistgenerator.azurewebsites.net/Authorize/Callback/";
        private string _redirectURLFromCreate = "https://spotifyplaylistgenerator.azurewebsites.net/Authorize/CallbackFromCreate/";
        private string _serverURI = "https://spotifyplaylistgenerator.azurewebsites.net/";
        private string _authorizeRedirect = "https://accounts.spotify.com/authorize?response_type=code&redirect_uri=";


        public async Task<ActionResult> Callback(string code)
        {
        //    string responseString = "";
            AuthorizationCodeAuth auth = new AuthorizationCodeAuth(
            _clientId,
            _secretId,
            _redirectURL,
            _serverURI,
            Scope.PlaylistReadPrivate | Scope.PlaylistReadCollaborative
            );


            Token token = await auth.ExchangeCode(code);

            SpotifyWebAPI api = new SpotifyWebAPI()
            {
                TokenType = token.TokenType,
                AccessToken = token.AccessToken
            };

            PrivateProfile prof = api.GetPrivateProfile();
            var tracks = await api.GetSavedTracksAsync();

            if (token.IsExpired())
            {
                Token newToken = await auth.RefreshToken(token.RefreshToken);
                api.AccessToken = newToken.AccessToken;
                api.TokenType = newToken.TokenType;
            }
            PrivateProfile profile = await api.GetPrivateProfileAsync();
            IndexViewModel viewModel = new IndexViewModel();

            viewModel.profile = profile;
           

            TempData["Api"] = api;
            TempData["User"] = viewModel.profile;
            TempData["Token"] = token;
            TempData["Auth"] = auth;
            return RedirectToAction("Index", "Home");
            //return new JsonResult
            //{
            //    ContentType = "application/json",
            //    Data = responseString,
            //    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                
            //};
            
        }
        public async Task<ActionResult> CallbackFromCreate(string code)
        {
            AuthorizationCodeAuth auth = new AuthorizationCodeAuth(
            _clientId,
            _secretId,
            _redirectURLFromCreate,
            _serverURI,
            Scope.PlaylistReadPrivate | Scope.PlaylistReadCollaborative
            );


            Token token = await auth.ExchangeCode(code);

            SpotifyWebAPI api = new SpotifyWebAPI()
            {
                TokenType = token.TokenType,
                AccessToken = token.AccessToken
            };

            PrivateProfile prof = api.GetPrivateProfile();
            var tracks = await api.GetSavedTracksAsync();

            if (token.IsExpired())
            {
                Token newToken = await auth.RefreshToken(token.RefreshToken);
                api.AccessToken = newToken.AccessToken;
                api.TokenType = newToken.TokenType;
            }
            PrivateProfile profile = await api.GetPrivateProfileAsync();
            IndexViewModel viewModel = (IndexViewModel) TempData["ViewModel"];
            viewModel.Playlist = (Playlist)TempData["Playlist"];
            viewModel.profile = profile;
            

            TempData["Api"] = api;
            TempData["User"] = viewModel.profile;
            TempData["Token"] = token;
            TempData["Auth"] = auth;
            TempData["ViewModel"] = viewModel;
            TempData["Playlist"] = (Playlist)TempData["Playlist"];
            return RedirectToAction("Create", "Playlist");

        }


        public ActionResult Login()
        {
            bool isFromIndex = (bool)TempData["isFromIndex"];
            if (isFromIndex)
            {
                return RedirectToAction("LoginFromIndex");
            }
            else
            {

                TempData["ViewModel"] = (IndexViewModel)TempData["ViewModel"];
                TempData["Playlist"] = (Playlist)TempData["Playlist"];
                return RedirectToAction("LoginFromCreate");
            }
        }

        public ActionResult LoginFromIndex()
        {
            var my_client_id = "52c0f5ab6e5f4a2f83da6c5fad1c6bac";
            var scopes = "user-read-private user-read-email user-read-recently-played user-top-read playlist-modify-public playlist-modify-private streaming";
            var redirect_uri = _redirectURL;
            

            return Redirect(_authorizeRedirect + redirect_uri + "&client_id=" + my_client_id + "&scope=" + scopes + "&show_dialog=true");
        }

        public ActionResult LoginFromCreate()
        {
            IndexViewModel viewModel = (IndexViewModel)TempData["ViewModel"];
            TempData["ViewModel"] = viewModel;
            TempData["Playlist"] = (Playlist)TempData["Playlist"];

            var my_client_id = "52c0f5ab6e5f4a2f83da6c5fad1c6bac";
            var scopes = "user-read-private user-read-email user-read-recently-played user-top-read playlist-modify-public playlist-modify-private streaming";
            var redirect_uri = _redirectURLFromCreate;


            return Redirect(_authorizeRedirect + redirect_uri + "&client_id=" + my_client_id + "&scope=" + scopes + "&show_dialog=true");

            
        }


        public ActionResult Logout()
        {
            bool isFromIndex = (bool)TempData["isFromIndex"];
            if (isFromIndex)
            {
                return RedirectToAction("LogoutFromIndex");
            }
            else
            {

                TempData["ViewModel"] = (IndexViewModel)TempData["ViewModel"];
                TempData["Playlist"] = (Playlist)TempData["Playlist"];
                return RedirectToAction("LogoutFromCreate");
            }
        }

        public ActionResult LogoutFromIndex()
        {
            TempData["User"] = null;
            TempData["Api"] = null;
            TempData["Token"] = null;
            TempData["Auth"] = null;

            return RedirectToAction("Index", "Home");
        }

        public ActionResult LogoutFromCreate()
        {
            IndexViewModel viewModel = (IndexViewModel)TempData["ViewModel"];
            viewModel.profile = null;
            viewModel.recentTracks = null;
            viewModel.topArtists = null;

            TempData["User"] = null;
            TempData["Api"] = null;
            TempData["Token"] = null;
            TempData["Auth"] = null;
            TempData["ViewModel"] = viewModel;
            TempData["Playlist"] = (Playlist)TempData["Playlist"];
            return RedirectToAction("Create", "Playlist");
        }
    }
}