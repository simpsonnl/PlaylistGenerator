using PlaylistGenerator.Models;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlaylistGenerator.ViewModels
{
    public class IndexViewModel
    {
        public IndexViewModel()
        {
            recentTracks = new Playlist();
            topArtists = new List<FullArtist>();
        }
        public Search search { get; set; }

        public FullTrack searchTrack { get; set; }
        public PrivateProfile profile { get; set; }
        public SpotifyWebAPI api { get; set; }
        public Playlist recentTracks { get; set; }
        public List<FullArtist> topArtists { get; set; }
        public Playlist Playlist { get; set; }
        public FullTrack searchedTrack { get; set; }
        public FullArtist searchedArtist { get; set; }
        public bool isTrack { get; set; }
        public string trackArtists { get; set; }
        public string title { get; set; }
        public string imageUrl { get; set; }
        public bool isFromIndex { get; set; }
        public Dictionary<string,string> trackLengths { get; set; }

    }
}