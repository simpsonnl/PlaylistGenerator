using PlaylistGenerator.Models;
using SpotifyAPI.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlaylistGenerator.ViewModels
{
    public class PlaylistViewModel 
    {
        public Playlist Playlist { get; set; }
        public FullTrack searchedTrack { get; set; }
        public FullArtist searchedArtist { get; set; }
        public bool isTrack { get; set; }
        public string trackArtists { get; set; }
        public string title { get; set; }
        public string imageUrl { get; set; }
    }
}