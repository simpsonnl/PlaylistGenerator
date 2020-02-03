using SpotifyAPI.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlaylistGenerator.Models
{
    public class Track
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string TrackToken { get; set; }

        public List<SimpleArtist> Artists { get; set; }
    }
}