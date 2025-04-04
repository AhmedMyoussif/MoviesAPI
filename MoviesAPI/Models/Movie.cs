﻿using System.Globalization;

namespace MoviesAPI.Models
{
    public class Movie
    {
        public int Id { get; set; }
        [MaxLength(250)]
        public string Title { get; set; }
        public int Year { get; set; }
        public double Rate { get; set; }
        public  string Storeline { get; set; }
        public  byte[] poster{ get; set; }
        public byte? GenreId { get; set; }
        public Genre Genre { get; set; }
    }
}
