using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace KanseiAPI.NewModel
{
    public class Subject
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        private string _id;

        [BsonElement("subject_name")]
        private string _name;

        public string Id { get => _id; set => _id = value; }
        public string Name { get => _name; set => _name = value; }

        public Subject(string id, string name)
        {
            _id = id;
            _name = name;
        }
        public Subject() { }
    }
}
