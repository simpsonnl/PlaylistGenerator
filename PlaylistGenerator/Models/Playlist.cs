using SpotifyAPI.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlaylistGenerator.Models
{
    public class Playlist : FullPlaylist
    {
        public Playlist()
        {
            TrackList = new List<FullTrack>();
        }
        public List<FullTrack> TrackList { get; set; }

        
        public bool hasTrack(string id)
        {
            foreach (var t in TrackList)
            {
                if (id.Equals(t.Id)) return true;
            }
            return false;
        }

    }
}