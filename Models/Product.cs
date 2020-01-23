using System;

namespace ProductElasticSearch.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Ean { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Brand { get; set; }
        public string Category { get; set; }
        public string Price { get; set; }
        public int Quantity { get; set; }
        public float Rating { get; set; }
        public DateTime ReleaseDate { get; set; }
    }

}