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
        [HandleError]
        public ActionResult Error() 
        {
            return View();
        }

        public async Task<ActionResult> Index(IndexViewModel viewModel)
        {
            TempData["Playlist"] = null;
            viewModel.isFromIndex = true;
            viewModel.profile = (PrivateProfile) TempData["User"];
            viewModel.api = (SpotifyWebAPI)TempData["Api"];
            Token token = (Token)TempData["Token"];
            AuthorizationCodeAuth auth = (AuthorizationCodeAuth)TempData["Auth"];
            if (token!=null) 
            {
                
                

                if (token.IsExpired())
                {

                    Token newToken = await auth.RefreshToken(token.RefreshToken);
                    viewModel.api.AccessToken = newToken.AccessToken;
                    viewModel.api.TokenType = newToken.TokenType;
                }

                CursorPaging<PlayHistory> histories = viewModel.api.GetUsersRecentlyPlayedTracks(20);
                Paging<FullArtist> artists = viewModel.api.GetUsersTopArtists(TimeRangeType.ShortTerm, 20);

                //creates a random list from 0 to the number of recent tracks and 0 to recent artists
                //then shuffled to be used to display random recent artists and tracks 
                //did the same thing for both for the case that there are not 20 recent artists or tracks to get
                //the lists might then be different sizes and could throw an index out of bounds error
                Random rand = new Random();
                List<int> randArtistNumbs = new List<int>();
                List<int> randTrackNumbs = new List<int>();

                for (int i = 0; i < artists.Items.Count; i++)
                {
                    randArtistNumbs.Add(i);
                }
                for (int i = 0; i < histories.Items.Count; i++)
                {
                    randTrackNumbs.Add(i);
                }

                //shuffles the list of artist numbers
                int n = randArtistNumbs.Count;
                while (n > 1)
                {
                    n--;
                    int k = rand.Next(n + 1);
                    int temp = randTrackNumbs[k];
                    randTrackNumbs[k] = randTrackNumbs[n];
                    randTrackNumbs[n] = temp;
                }

                //shuffle list of track numbers
                int m = randTrackNumbs.Count;
                while (m > 1)
                {
                    m--;
                    int k = rand.Next(m + 1);
                    int temp = randTrackNumbs[k];
                    randTrackNumbs[k] = randTrackNumbs[m];
                    randTrackNumbs[m] = temp;
                }

                //adds up to 5 recent artists to the top artists view property
                for (int i = 0; i < 5; i++)
                {
                    if (artists.Items[randArtistNumbs[i]] != null)
                    {
                        viewModel.topArtists.Add(artists.Items[randArtistNumbs[i]]);
                    }
                }
                //adds non duplicate tracks to the recent tracks property
                viewModel.recentTracks = new Playlist();
                int count = 0;
                while(viewModel.recentTracks.TrackList.Count < 5 && count < randTrackNumbs.Count)
                {
                    string trackId = histories.Items[randTrackNumbs[count]].Track.Id;
                    if (!viewModel.recentTracks.hasTrack(trackId))
                    {
                        viewModel.recentTracks.TrackList.Add(viewModel.api.GetTrack(trackId));
                    }
                    count++;
                }
            }

            ViewBag.FlashMessage = (string)TempData["FlashMessage"];

            if (TempData["isError"] != null)
            {
                ViewBag.isError = (bool)TempData["isError"];
            }
            
            TempData["User"] = viewModel.profile;
            TempData["Api"] = viewModel.api;
            TempData["Token"] = token;
            TempData["Auth"] = auth;
            TempData["isFromIndex"] = true;
            TempData["ViewModel"] = viewModel;
            return View(viewModel);
        }

        //called by the search bar to display send back a json result of the top 5 track results from the search item in the US
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

        //called by the search bar to display send back a json result of the top 5 artist results from the search item in the US
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
    }
}