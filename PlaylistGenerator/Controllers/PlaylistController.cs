using PlaylistGenerator.Models;
using PlaylistGenerator.ViewModels;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PlaylistGenerator.Controllers
{
    public class PlaylistController : Controller
    {
        private static SpotifyWebAPI _spotify;
        private CredentialsAuth auth = new CredentialsAuth("52c0f5ab6e5f4a2f83da6c5fad1c6bac", "a66d0d708a1f49789372f50a65d2b3cc");
        private List<string> playlistURIs = new List<string>();

        //returns you to the create playlist view
        public async Task<ActionResult> Create() 
        {
            if ((Playlist)TempData["Playlist"] == null)
            {
                return RedirectToAction("NoItemSelected", "Error");
            }

            IndexViewModel viewModel = (IndexViewModel)TempData["ViewModel"];
            Token userToken = (Token)TempData["Token"];
            AuthorizationCodeAuth userAuth = (AuthorizationCodeAuth)TempData["Auth"];
            viewModel.profile = (PrivateProfile)TempData["User"];
            viewModel.api = (SpotifyWebAPI)TempData["Api"];
            viewModel.Playlist = (Playlist)TempData["Playlist"];
            viewModel.PlaylistURIs = (List<string>)TempData["PlaylistURIs"];

            Console.WriteLine(viewModel.PlaylistURIs);

            if (viewModel.profile != null)
            {
                if (userToken.IsExpired())
                {

                    Token newToken = await userAuth.RefreshToken(userToken.RefreshToken);
                    viewModel.api.AccessToken = newToken.AccessToken;
                    viewModel.api.TokenType = newToken.TokenType;
                }
            }

            TempData["PlaylistURIs"] = (List<string>)TempData["PlaylistURIs"];
            TempData["Api"] = viewModel.api;
            TempData["User"] = viewModel.profile;
            TempData["Token"] = userToken;
            TempData["Auth"] = userAuth;
            TempData["ViewModel"] = viewModel;
            TempData["Playlist"] = viewModel.Playlist;
            TempData["isFromIndex"] = false;
            return View(viewModel);
        }
        public async Task<ActionResult> CreatePlaylist(string inputId, bool isTrack, int range, int numberOfTracks)
        {

            IndexViewModel viewModel = new IndexViewModel();

            viewModel.profile = (PrivateProfile)TempData["User"];
            viewModel.api = (SpotifyWebAPI)TempData["Api"];
            Token userToken = (Token)TempData["Token"];
            AuthorizationCodeAuth userAuth = (AuthorizationCodeAuth)TempData["Auth"];

            if(userToken != null)
            {
                if (userToken.IsExpired())
                {

                    Token newToken = await userAuth.RefreshToken(userToken.RefreshToken);
                    viewModel.api.AccessToken = newToken.AccessToken;
                    viewModel.api.TokenType = newToken.TokenType;
                }
            }

            viewModel.isTrack = isTrack;

            Token token = await auth.GetToken();
            _spotify = new SpotifyWebAPI()
            {
                AccessToken = token.AccessToken,
                TokenType = token.TokenType
            };

            string artistId = inputId;
            List<string> trackArtistIds = new List<string>();
            SeveralArtists relatedArtists = new SeveralArtists();

            List<SeveralArtists> relatedArtistsLists = new List<SeveralArtists>();

            //if the search item was a track
            //get all the artists from that track 
            //then get all the related artists for those artists
            //then create the playlsit
            if (isTrack)
            {

                viewModel.searchedTrack = _spotify.GetTrack(inputId);
                artistId = viewModel.searchedTrack.Artists[0].Id;
                viewModel.title = viewModel.searchedTrack.Name + " Playlist";
                viewModel.trackArtists = displayArtistsNames(viewModel.searchedTrack);
                viewModel.imageUrl = viewModel.searchedTrack.Album.Images[1].Url;

                relatedArtists.Artists = new List<FullArtist>();
                int i = 0;
                foreach (var artist in viewModel.searchedTrack.Artists)
                {
                    relatedArtistsLists.Add(_spotify.GetRelatedArtists(artist.Id));
                    relatedArtistsLists[i].Artists.Insert(0, _spotify.GetArtist(artist.Id));
                    i++;
                }

                viewModel.Playlist = getPlaylistFromTrack(relatedArtistsLists, numberOfTracks, range, viewModel.searchedTrack.Id);
                viewModel.trackLengths = (Dictionary<string,string>)TempData["TrackLengths"];
                
            }
            //if the search item was an artist, get the related artists, and create a playlist
            else
            {
                viewModel.searchedArtist = _spotify.GetArtist(artistId);
                viewModel.title = viewModel.searchedArtist.Name + " Playlist";
                viewModel.imageUrl = viewModel.searchedArtist.Images[1].Url;

                relatedArtists = _spotify.GetRelatedArtists(artistId);
                relatedArtists.Artists.Insert(0, (_spotify.GetArtist(artistId)));
                viewModel.Playlist = getPlaylistFromArtist(relatedArtists, numberOfTracks, range);
                viewModel.trackLengths = (Dictionary<string, string>)TempData["TrackLengths"];
            }

            TempData["PlaylistURIs"] = (List<string>)TempData["PlaylistURIs"];

            TempData["Playlist"] = viewModel.Playlist;
            TempData["Token"] = userToken;
            TempData["Auth"] = userAuth;
            TempData["ViewModel"] = viewModel;
            return RedirectToAction("Create");
        }
        
        public ActionResult CreateFromCard(string inputId, int range, int numberOfTracks, bool isTrack) 
        {
            IndexViewModel viewModel = new IndexViewModel();

            viewModel.profile = (PrivateProfile)TempData["User"];
            viewModel.api = (SpotifyWebAPI)TempData["Api"];
            Token userToken = (Token)TempData["Token"];
            AuthorizationCodeAuth userAuth = (AuthorizationCodeAuth)TempData["Auth"];

            TempData["Api"] = viewModel.api;
            TempData["User"] = viewModel.profile;
            TempData["Token"] = userToken;
            TempData["Auth"] = userAuth;
            TempData["Playlist"] = viewModel.Playlist;

            return RedirectToAction("CreatePlaylist", new { inputId, isTrack, range, numberOfTracks });
        }


        

        //creates the playlist from the selected artist
        private Playlist getPlaylistFromArtist(SeveralArtists relatedArtists, int size, int range)
        {
            List<string> genres = new List<string>();
            int relatedArtistsCount = relatedArtists.Artists.Count;
            int artists = (int)((range / 5.0) * relatedArtistsCount);
            var rand = new Random();
            Playlist returnedPlaylist = new Playlist();
            FullTrack track = new FullTrack();
            Dictionary<string, string> trackLengths = new Dictionary<string, string>();
            List<string> playlistURIs = new List<string>();

            SeveralTracks tracks = new SeveralTracks
            {
                Tracks = new List<FullTrack>()
            };

            for (int i = 0; i < artists; i++)
            {
                tracks.Tracks.AddRange(_spotify.GetArtistsTopTracks(relatedArtists.Artists[i].Id, "US").Tracks);

            }


            //shuffles the list of tracks
            int n = tracks.Tracks.Count;
            while (n > 1)
            {
                n--;
                int k = rand.Next(n + 1);
                FullTrack temp = tracks.Tracks[k];
                tracks.Tracks[k] = tracks.Tracks[n];
                tracks.Tracks[n] = temp;
            }

            int count = 0;
            int iterations = 0;
            

            if (tracks.Tracks.Count!=0)
            {
                do
                {
                    track = tracks.Tracks[iterations];
                    if (track != null && !returnedPlaylist.hasTrack(track.Id))
                    {
                        trackLengths.Add(track.Id, getTrackLengthString(track.DurationMs));
                        returnedPlaylist.TrackList.Add(track);
                        playlistURIs.Add(track.Uri);
                        count++;
                    }
                    iterations++;
                } while (count < size && iterations < tracks.Tracks.Count);
            }

            TempData["TrackLengths"] = trackLengths;
            TempData["PlaylistURIs"] = playlistURIs;
            return returnedPlaylist;
        }

        //creates a playlist from the selected track 
        private Playlist getPlaylistFromTrack(List<SeveralArtists> relatedArtistsList, int size, int range, string searchTrackId)
        {
            int relatedArtistsCount = 0;
            List<string> genres = new List<string>();
            SeveralArtists reOrderedArtists = new SeveralArtists();
            reOrderedArtists.Artists = new List<FullArtist>();

            foreach (var artistList in relatedArtistsList)
            {
                relatedArtistsCount += artistList.Artists.Count;
            }
            
            int artistsAmount = (int)((range / 5.0) * relatedArtistsCount);

            //in the case that the song has multiple artists,
            //i don't want the list of related artists to be: related artists from the 1st, then 2nd then 3rd artist etc...
            //i want them to be spread out evenly to give a better playlist based on the songs artists
            for (int i = 0; i < relatedArtistsCount; i++)
            {
                for (int j = 0; j < relatedArtistsList.Count; j++)
                {
                    if (i<relatedArtistsList[j].Artists.Count)
                    {
                        reOrderedArtists.Artists.Add(relatedArtistsList[j].Artists[i]);
                    }
                }
            }

            var rand = new Random();
            Playlist returnedPlaylist = new Playlist();
            FullTrack track = new FullTrack();

            SeveralTracks tracks = new SeveralTracks
            {
                Tracks = new List<FullTrack>()
            };
            
            for (int i = 0; i < artistsAmount; i++)
            {
                tracks.Tracks.AddRange(_spotify.GetArtistsTopTracks(reOrderedArtists.Artists[i].Id, "US").Tracks);
            }

            //shuffles the list of tracks
            int n = tracks.Tracks.Count;
            while (n > 1)
            {
                n--;
                int k = rand.Next(n + 1);
                FullTrack temp = tracks.Tracks[k];
                tracks.Tracks[k] = tracks.Tracks[n];
                tracks.Tracks[n] = temp;
            }
            Dictionary<string, string> trackLengths = new Dictionary<string, string>();
            int count = 0;
            int iterations = 0;
            if (tracks.Tracks.Count!=0)
            {
                do
                {
                    track = tracks.Tracks[iterations];

                    if (track != null && !returnedPlaylist.hasTrack(track.Id))
                    {
                            trackLengths.Add(track.Id, getTrackLengthString(track.DurationMs));
                            returnedPlaylist.TrackList.Add(track);
                            playlistURIs.Add(track.Uri);
                            count++;
                    }

                    iterations++;
                } while (count < size && iterations < tracks.Tracks.Count);
            }

            TempData["PlaylistURIs"] = playlistURIs;
            TempData["TrackLengths"] = trackLengths;
            return returnedPlaylist;
        }
        
        //saves the playlist to th users account and sends you back to the home page
        public async Task<ActionResult> Save(string playlistName, bool isPublic=false) {
            string name = playlistName;
            if (name == "")
            {
                name = ((IndexViewModel)TempData["ViewModel"]).title;
            }

            Token token = (Token)TempData["Token"];
            AuthorizationCodeAuth auth = (AuthorizationCodeAuth)TempData["Auth"];
            SpotifyWebAPI api = (SpotifyWebAPI)TempData["Api"];
            Playlist currentPlaylist = (Playlist)TempData["Playlist"];
            IndexViewModel viewModel = (IndexViewModel)TempData["ViewModel"];
            viewModel.PlaylistURIs = (List<string>)TempData["PlaylistURIs"];
            if (token.IsExpired())
            {

                Token newToken = await auth.RefreshToken(token.RefreshToken);
                api.AccessToken = newToken.AccessToken;
                api.TokenType = newToken.TokenType;
            }

            string userId = api.GetPrivateProfile().Id;

            FullPlaylist playlist = api.CreatePlaylist(userId, name, isPublic);

            ErrorResponse response = new ErrorResponse();

            response = api.AddPlaylistTracks(playlist.Id, viewModel.PlaylistURIs);
            
            TempData["FlashMessage"] = name + " was successfully created and saved to your Spotify account!";
            TempData["isError"] = false;

            //checks for error from laylist save and changes the flass message appropriately.
            if (response.HasError())
            {
                TempData["FlashMessage"] = "Something went wrong while saving the playlist.";
                TempData["isError"] = true;
            }


            
            return RedirectToAction("Index", "Home");
        }

        //adds all the artists to be shown in the search bar
        private string displayArtistsNames(FullTrack searchedTrack)
        {
            var displayArtist = "";

            foreach (var artist in searchedTrack.Artists)
            {
                displayArtist += artist.Name + ", ";
            }

            var displayArtistTrimmed = displayArtist.Remove(displayArtist.Length - 2, 2);

            return displayArtist;
        }

        //converts the track length from ms to M:SS
        private string getTrackLengthString(int ms) 
        {
            

            TimeSpan t = TimeSpan.FromMilliseconds(ms);

            string length = string.Format("{0:D1}:{1:D2}",
                        t.Minutes,
                        t.Seconds);

            return length;
        }
    }
}