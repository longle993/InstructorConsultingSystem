using KanseiAPI.NewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace KanseiAPI.Controllers
{
    [Route("api/evaluate")]
    [ApiController]
    public class EvaluationController : ControllerBase
    {
        [HttpPost("", Name = "PostNewEvaluate")]
        public async Task<ActionResult<ResponseInfo>> PostNewEvaluation([FromBody] Evaluation evaluation)
        {
            ResponseInfo response = new ResponseInfo();
            try
            {
                var client = new MongoClient("mongodb+srv://kanseidemo123:kanseidemo123@cluster0.eetjn7s.mongodb.net/?retryWrites=true&w=majority");
                var database = client.GetDatabase("Kansei");
                var evaluationTable = database.GetCollection<Evaluation>("EvaluateKansei");
                evaluationTable.InsertOne(evaluation);
                response.statusCode = System.Net.HttpStatusCode.OK;
                response.data = evaluation;
                return await Task.FromResult(response);
            }
            catch (Exception e)
            {
                response.statusCode = System.Net.HttpStatusCode.BadRequest;
                Console.WriteLine(e.ToString());
                return await Task.FromResult(response);
            }
        }
        [HttpGet("get-all-evaluation")]
        public async Task<ActionResult<ResponseInfo>> GetAllEvaluation()
        {
            ResponseInfo response = new ResponseInfo();
            try
            {
                var client = new MongoClient("mongodb+srv://kanseidemo123:kanseidemo123@cluster0.eetjn7s.mongodb.net/?retryWrites=true&w=majority");
                var database = client.GetDatabase("Kansei");
                var evaluationCollection = database.GetCollection<Evaluation>("EvaluateKansei");
                var kanseiCollection = database.GetCollection<Kansei>("KanseiWord");
                var criteriaCollection = database.GetCollection<Criteria>("Criteria");

                List<Evaluation> listEvaluation = evaluationCollection.Find(new BsonDocument()).ToList();
                response.statusCode = System.Net.HttpStatusCode.OK;
                response.data = listEvaluation;
                return await Task.FromResult(response);
            }
            catch (Exception e)
            {
                response.statusCode = System.Net.HttpStatusCode.BadRequest;
                Console.WriteLine(e.ToString());
                return await Task.FromResult(response);
            }
        }
    }
}
