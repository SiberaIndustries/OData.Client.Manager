namespace OData.Client.Manager.Tests
{
    public class Product
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTimeOffset ReleaseDate { get; set; }

        public DateTimeOffset? DiscontinuedDate { get; set; }

        public int Rating { get; set; }

        public double Price { get; set; }
    }
}
