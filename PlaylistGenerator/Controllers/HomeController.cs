using PlaylistGenerator.Models;
using PlaylistGenerator.ViewModels;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PlaylistGenerator.Controllers
{
    public class HomeController : Controller
    {
        private static SpotifyWebAPI _spotify;
        private static FullTrack track = new FullTrack();
        private CredentialsAuth auth = new CredentialsAuth("52c0f5ab6e5f4a2f83da6c5fad1c6bac", "a66d0d708a1f49789372f50a65d2b3cc");
        private List<Search> searchList = new List<Search>();

        public HomeController()
        {
            
        }

        public  ActionResult Index(IndexViewModel viewModel)
        {
            viewModel.profile = (PrivateProfile) TempData["User"];
            viewModel.api = (SpotifyWebAPI)TempData["api"];
           
            
            
            return View(viewModel);
        }
        [HttpPost]
        public async Task<JsonResult> SearchTrack(string query)
        {
            Token token = await auth.GetToken();
            _spotify = new SpotifyWebAPI()
            {
                AccessToken = token.AccessToken,
                TokenType = token.TokenType
            };

            

            var searchItems = _spotify.SearchItems(query, SearchType.Track, 5, 0, "US").Tracks.Items;

            searchList.Clear();
            
            foreach (var item in searchItems)
            {
                var displayArtist = "";

                foreach (var artist in item.Artists)
                {
                    displayArtist += artist.Name + ", ";
                }

                var displayArtistTrimmed = displayArtist.Remove(displayArtist.Length - 2, 2);

                searchList.Add(new Search() {
                    Name = item.Name + " - " + displayArtistTrimmed,
                    Id = item.Id
                }) ;
            }

            var searchItemsJson = Json(searchList, JsonRequestBehavior.AllowGet);

            

            return searchItemsJson ;
        }

        [HttpPost]
        public async Task<JsonResult> SearchArtistTerm(string query)
        {
            Token token = await auth.GetToken();
            _spotify = new SpotifyWebAPI()
            {
                AccessToken = token.AccessToken,
                TokenType = token.TokenType
            };

            var j = Json(_spotify.SearchItems(query, SearchType.Artist, 5, 0, "US").Artists.Items, JsonRequestBehavior.AllowGet);

            return j;
        }

        //[HttpPost]
        //public async Task<ActionResult> Search(string query)
        //{
        //    searchItem.Tracks = new Paging<FullTrack>();
        //    searchItem.Tracks.Items = new List<FullTrack>();
        //    Token token = await auth.GetToken();
        //    _spotify = new SpotifyWebAPI()
        //    {
        //        AccessToken = token.AccessToken,
        //        TokenType = token.TokenType
        //    };


        //    searchItem = _spotify.SearchItems(query, SearchType.Track, 5, 0, "US");
            

        //    return View(searchItem);
        //}

        public ActionResult Playlist()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Playlist(Playlist playlist)
        {


            return View();
        }


    }
}