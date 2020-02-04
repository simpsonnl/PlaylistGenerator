using PlaylistGenerator.Models;
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
        private CredentialsAuth auth = new CredentialsAuth("52c0f5ab6e5f4a2f83da6c5fad1c6bac", "6e2bea1292b845b392725811ae29b026");
        private SearchItem searchItem = new SearchItem();

        public HomeController()
        {
            
        }

        public async Task<ActionResult> Index()
        {
            searchItem.Tracks = new Paging<FullTrack>();
            searchItem.Tracks.Items = new List<FullTrack>();

            Token token = await auth.GetToken();
            _spotify = new SpotifyWebAPI()
            {
                AccessToken = token.AccessToken,
                TokenType = token.TokenType
            };
            FullTrack fulltrack = _spotify.GetTrack("4Bh5r6syfTaPkGZFRD2ZFj");
            List<SimpleArtist> artists = _spotify.GetTrack("4Bh5r6syfTaPkGZFRD2ZFj").Artists;

            track = fulltrack;
         


            return View();
        }
        
        public ActionResult Search()
        {
            searchItem.Tracks = new Paging<FullTrack>();
            searchItem.Tracks.Items = new List<FullTrack>();
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Search(string query)
        {
            searchItem.Tracks = new Paging<FullTrack>();
            searchItem.Tracks.Items = new List<FullTrack>();
            Token token = await auth.GetToken();
            _spotify = new SpotifyWebAPI()
            {
                AccessToken = token.AccessToken,
                TokenType = token.TokenType
            };


            searchItem = _spotify.SearchItems(query, SearchType.Track, 20, 0, "US");
            

            return View(searchItem);
        }

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