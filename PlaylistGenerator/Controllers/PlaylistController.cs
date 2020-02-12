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
        private CredentialsAuth auth = new CredentialsAuth("52c0f5ab6e5f4a2f83da6c5fad1c6bac", "6e2bea1292b845b392725811ae29b026");
        public async Task<ActionResult> CreatePlaylist(string inputId, bool isTrack, int range, int numberOfTracks)
        {
            PlaylistViewModel viewModel = new PlaylistViewModel();
            
            viewModel.isTrack = isTrack;

            Token token = await auth.GetToken();
            _spotify = new SpotifyWebAPI()
            {
                AccessToken = token.AccessToken,
                TokenType = token.TokenType
            };


            

            
            string artistId = inputId;

            if (isTrack)
            {

                viewModel.searchedTrack = _spotify.GetTrack(inputId);
                artistId = viewModel.searchedTrack.Artists[0].Id;
                viewModel.title = viewModel.searchedTrack.Name + " Playlist";
                viewModel.trackArtists = displayArtistsNames(viewModel.searchedTrack);
                viewModel.imageUrl = viewModel.searchedTrack.Album.Images[1].Url;
            }
            else {
                viewModel.searchedArtist = _spotify.GetArtist(artistId);
                viewModel.title = viewModel.searchedArtist.Name + " Playlist";
                viewModel.imageUrl = viewModel.searchedArtist.Images[1].Url;
            }
            

            SeveralArtists relatedArtists = new SeveralArtists();

            
            relatedArtists = _spotify.GetRelatedArtists(artistId);
            relatedArtists.Artists.Insert(0,(_spotify.GetArtist(artistId)));
            

            viewModel.Playlist = getPlaylist(relatedArtists, numberOfTracks, range);

            return View(viewModel);
        }

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

        [HttpPost]
        public ActionResult Create()
        {
            return View();
        }

        private SeveralArtists getRelatedArtists(string id, int scale)
        {
            SeveralArtists relatedArtists = new SeveralArtists();
            relatedArtists = _spotify.GetRelatedArtists(id);
            SeveralArtists returnedArtists = new SeveralArtists
            {
                Artists = new List<FullArtist>
            {
                _spotify.GetArtist(id)
            }
            };
            
            Random rand = new Random();
            switch (scale)
            {
                case 1:
                    
                    for (int i = 0; i < 5; i++)
                    {
                        returnedArtists.Artists.Add(relatedArtists.Artists[i]);
                    }
                    return returnedArtists;
                case 2:
                    for (int i = 0; i < (relatedArtists.Artists.Count * .4); i++)
                    {
                        returnedArtists.Artists.Add(relatedArtists.Artists[i]);
                    }
                    return returnedArtists;
                case 3:
                    for (int i = 0; i < (relatedArtists.Artists.Count * (.6)); i++)
                    {
                        returnedArtists.Artists.Add(relatedArtists.Artists[i]);
                    }
                    return returnedArtists;
                case 4:
                    for (int i = 0; i < (relatedArtists.Artists.Count * .8); i++)
                    {
                        returnedArtists.Artists.Add(relatedArtists.Artists[i]);
                    }
                    return returnedArtists;
                case 5:
                    for (int i = 0; i < relatedArtists.Artists.Count; i++)
                    {
                        returnedArtists.Artists.Add(relatedArtists.Artists[i]);
                    }
                    return returnedArtists;
                default:
                    relatedArtists = _spotify.GetRelatedArtists(id);
                    return null;
            }
        }
        private Playlist getPlaylist(SeveralArtists relatedArtists, int size, int range)
        {
            int relatedArtistsCount = relatedArtists.Artists.Count;
            int artists = (int)((range / 5.0) * relatedArtistsCount);

            var rand = new Random();
            Playlist returnedPlaylist = new Playlist();
            FullTrack track = new FullTrack();

            SeveralTracks tracks = new SeveralTracks
            {
                Tracks = new List<FullTrack>()
            };
            int count = 0;
            for (int i = 0; i < artists; i++)
            {
                tracks.Tracks.AddRange(_spotify.GetArtistsTopTracks(relatedArtists.Artists[i].Id, "US").Tracks);
            }

            do
            {
                //tracks = _spotify.GetArtistsTopTracks(relatedArtists.Artists[rand.Next(0, artists)].Id, "US");

                track = tracks.Tracks[rand.Next(0, tracks.Tracks.Count)];

                if (track != null && !returnedPlaylist.hasTrack(track.Id))
                {
                    returnedPlaylist.TrackList.Add(track);
                    count++;
                }


            } while (count < size);



            return returnedPlaylist;
        }
    }
}