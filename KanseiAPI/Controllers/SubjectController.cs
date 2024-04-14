using KanseiAPI.NewModel;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace KanseiAPI.Controllers
{
    [Route("api/subject")]
    [ApiController]
    public class SubjectController
    {
        private List<Subject> listSubject;
        public SubjectController()
        {
            listSubject = new List<Subject>();
        }

        [HttpGet("get-list-subject")]
        public async Task<ActionResult<ResponseInfo>> GetListGV()
        {
            ResponseInfo response = new ResponseInfo();
            try
            {
                var client = new MongoClient("mongodb+srv://kanseidemo123:kanseidemo123@cluster0.eetjn7s.mongodb.net/?retryWrites=true&w=majority");
                var database = client.GetDatabase("Kansei");
                var subjectTable = database.GetCollection<Subject>("Subject");
                List<Subject> subjects = subjectTable.Find(new BsonDocument()).ToList();

                response.statusCode = System.Net.HttpStatusCode.OK;
                response.data = subjects;
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
