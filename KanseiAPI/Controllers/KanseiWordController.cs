using KanseiAPI.Interface;
using KanseiAPI.NewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace KanseiAPI.Controllers
{
    [Route("api/kansei-word")]
    [ApiController]
    public class KanseiWordController
    {
        private IKanseiWordRepository kanseiWordRepository;
        private List<Kansei> kanseiWords;
        private List<Kansei> kanseiPreprocess;
        public KanseiWordController(IKanseiWordRepository kanseiWordRepository)
        {
            this.kanseiWordRepository = kanseiWordRepository;
            kanseiWords = new List<Kansei>();
            kanseiPreprocess = new List<Kansei>();
        }

        [HttpPost("get-advise")]
        public async Task<ActionResult<ResponseInfo>> AddPointAdvise(List<KanseiSolve> listKansei,string idSubject)
        {
            ResponseInfo response = new ResponseInfo();
            try
            {
                var client = new MongoClient("mongodb+srv://kanseidemo123:kanseidemo123@cluster0.eetjn7s.mongodb.net/?retryWrites=true&w=majority");
                var database = client.GetDatabase("Kansei");
                var teacherTable = database.GetCollection<Teacher>("Teacher");
                List<Teacher> teachers = teacherTable.Find(new BsonDocument()).ToList();

                var criteriaTable = database.GetCollection<Criteria>("Criteria");
                List<Criteria> criterias = criteriaTable.Find(new BsonDocument()).ToList();
                var evaluateKansei = database.GetCollection<Evaluation>("EvaluateKansei");
                List<Evaluation> evaluations = evaluateKansei.Find(new BsonDocument()).ToList();
                List<Evaluation> evalSort = evaluations.Where(p => p.Id_subject.Equals(idSubject)).ToList();

                Algorithm algorithm = new Algorithm(evalSort, criterias,teachers,listKansei);
                algorithm.execute();

                response.statusCode = System.Net.HttpStatusCode.OK;
                response.data = algorithm.ListFinal;
                return await Task.FromResult(response);
            }
            catch (Exception e)
            {
                response.statusCode = System.Net.HttpStatusCode.BadRequest;
                response.message  = e.Message;
                Console.WriteLine(e.ToString());
                return await Task.FromResult(response);
            }
        }

        [HttpGet("", Name = "GetListKansei")]
        public async Task<ActionResult<ResponseInfo>> GetListKansei()
        {
            ResponseInfo response = new ResponseInfo();
            try
            {
                var client = new MongoClient("mongodb+srv://kanseidemo123:kanseidemo123@cluster0.eetjn7s.mongodb.net/?retryWrites=true&w=majority");
                var database = client.GetDatabase("Kansei");
                var kanseiCollection = database.GetCollection<KanseiSolve>("KanseiWord");
                var criteriaCollection = database.GetCollection<Criteria>("Criteria");
                //List<Kansei> listKansei = kanseiCollection.Find(new BsonDocument()).ToList();
                var listKansei = await kanseiCollection.Aggregate()
                 .Lookup(
                foreignCollection: criteriaCollection,
                localField: x => x.Type,
                foreignField: x => x.Id,
                @as: (Kansei kansei) => kansei.Criterias
                )
                .ToListAsync();
                response.statusCode = System.Net.HttpStatusCode.OK;
                response.data = listKansei;
                return await Task.FromResult(response);
            }
            catch (Exception e)
            {
                response.statusCode = System.Net.HttpStatusCode.BadRequest;
                response.message = e.Message;
                Console.WriteLine(e.ToString());
                return await Task.FromResult(response);
            }
        }

    }
}
