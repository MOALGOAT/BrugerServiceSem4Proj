using MongoDB.Bson.Serialization.Attributes;


namespace BrugerServiceAPI.Models
{
    public class Bruger
    {
        [BsonId]
        public Guid brugerID { get; set; }
        public string fornavn { get; set; }
        public string efternavn { get; set; }
        public string email { get; set; }
        public string adresse { get; set; }
        public string telefonnummer { get; set; }
        public int rolle { get; set; }
        public string brugernavn { get; set; }
        public string adgangskode { get; set; }
    }
}
