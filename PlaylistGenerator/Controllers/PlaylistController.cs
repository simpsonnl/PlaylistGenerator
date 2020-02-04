using PlaylistGenerator.Models;
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
        private CredentialsAuth auth = new CredentialsAuth("52c0f5ab6e5f4a2f83da6c5fad1c6bac", "6e2bea1292b845b392725811ae29b026");
        public async Task<ActionResult> Create(string id)
        {
            Token token = await auth.GetToken();
            _spotify = new SpotifyWebAPI()
            {
                AccessToken = token.AccessToken,
                TokenType = token.TokenType
            };
            FullTrack t = new FullTrack();
            t.Artists = new List<SimpleArtist>();
            t = _spotify.GetTrack(id);

            SeveralArtists relatedArtists = new SeveralArtists();
            Playlist playlist = new Playlist();
            
            relatedArtists = _spotify.GetRelatedArtists(id);

            var rand = new Random();

            foreach (var artist in relatedArtists.Artists)
            {

                SeveralTracks tracks = _spotify.GetArtistsTopTracks(artist.Id, "US");



                FullTrack track = tracks.Tracks[rand.Next(0,tracks.Tracks.Count)];
                playlist.TrackList.Add(track);
            }

            return View(playlist);
        }

        [HttpPost]
        public ActionResult Create()
        {
            return View();
        }
    }
}