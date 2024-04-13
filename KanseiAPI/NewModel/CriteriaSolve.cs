using MongoDB.Bson.Serialization.Attributes;

namespace KanseiAPI.NewModel
{
    public class CriteriaSolve
    {
        [BsonElement("name")]
        private string _name;
        [BsonElement("point")]
        private double _point;

        public CriteriaSolve() { }
        public CriteriaSolve(string name, double point)
        {
            _name = name;
            _point = point;
        }

        public string Name { get => _name; set => _name = value; }
        public double Point { get => _point; set => _point = value; }
    }
}
