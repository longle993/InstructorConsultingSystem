using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace KanseiAPI.NewModel
{
    public class EvaluationInput
    {

        public string student_name { get; set; }

        public string id_teacher { get; set; }

        public List<Point> listPoint { get; set; }
        public List<PointCriteria> listPointCriteria { get; set; }

        public string id_subject { get; set; }


        public class PointCriteria
        {
            public string name { get; set; }
            public double point { get; set; }
        }

        public class Point
        {
            public string name { get; set; }
            public string type { get; set; }
            public int point { get; set; }
        }

    }
}
