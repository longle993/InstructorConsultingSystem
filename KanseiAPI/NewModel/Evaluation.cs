using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace KanseiAPI.NewModel
{
    public class Evaluation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        private string _id;

        [BsonElement("student_name")]
        private string _studentName;

        [BsonElement("id_teacher")]
        private string _teacherId;

        [BsonElement("listPoint")]
        private List<Kansei> _listKansei;

        public List<double> Standardized = new List<double>();
        public double mCC;

        public string Id { get => _id; set => _id = value; }
        public string StudentName { get => _studentName; set => _studentName = value; }
        public string TeacherId { get => _teacherId; set => _teacherId = value; }
        public List<Kansei> ListKansei { get => _listKansei; set => _listKansei = value; }

        public Evaluation(string id, string studentName, string teacherId, List<Kansei> listKansei)
        {
            _id = id;
            _studentName = studentName;
            _teacherId = teacherId;
            _listKansei = listKansei;
        }
        public Evaluation() { }
    }
}
