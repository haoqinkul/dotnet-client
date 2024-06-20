using Newtonsoft.Json;
using System.Collections.Generic;

namespace clientSocket
{
    public class ElaboratedImageCoordinates : IElaboratedImageCoordinates
    {
        public double CoordinateX { get; set; }
        public double CoordinateY { get; set; }
    }

    public class Fastener : IElaboratedImageCoordinates
    {
        [JsonProperty("x_top_left")]
        public double CoordinateX { get; set; }

        [JsonProperty("y_top_left")]
        public double CoordinateY { get; set; }

        // Other properties
        public string Uuid { get; set; }
        public string FastenerType { get; set; }
        // Add other properties as needed
    }

    public class Product
    {
        public string Uuid { get; set; }
        public string ProductClass { get; set; }
        // Add other properties as needed
    }

    public class Infer
    {
        public Product Product { get; set; }
        public List<Fastener> Fasteners { get; set; }
        // Add other properties as needed
    }

    public class Response
    {
        public Infer Infer { get; set; }
    }
}
