using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace KanseiAPI.NewModel
{
    public class Kansei
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        private string _id;

        [BsonElement("name")]
        private string _name;

        [BsonElement("type")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Type { get; set; }

        [BsonElement("point")]
        private double _point;

        public Kansei(string id, string name, string type, double point)
        {
            _id = id;
            _name = name;
            Type = type;
            _point = point;
        }
        public Kansei() { }
        public string Id { get => _id; set => _id = value; }
        public string Name { get => _name; set => _name = value; }
        public double Point { get => _point; set => _point = value; }
        public List<Criteria> Criterias { get; set; }

    }
}
