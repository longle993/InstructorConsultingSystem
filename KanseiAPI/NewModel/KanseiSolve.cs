using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace KanseiAPI.NewModel
{
    public class KanseiSolve
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        private string _id;

        [BsonElement("name")]
        private string _name;

        [BsonElement("type")]
        [BsonRepresentation(BsonType.ObjectId)]
        private string _type;

        [BsonElement("point")]
        private double _point;

        public KanseiSolve(string id, string name, string type, double point)
        {
            _id = id;
            _name = name;
            Type = type;
            _point = point;
        }
        public KanseiSolve() { }
        public string Id { get => _id; set => _id = value; }
        public string Name { get => _name; set => _name = value; }
        public double Point { get => _point; set => _point = value; }
        public string Type { get => _type; set => _type = value; }
        public List<Criteria>? Criterias { get; set; }


    }

}
