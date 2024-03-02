using KanseiAPI.Interface;
using KanseiAPI.NewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
namespace KanseiAPI.Controllers
{
    [Route("api/teacher")]
    [ApiController]
    public class TeacherController
    {
        private List<Teacher> listTeacher;
        public TeacherController() {
            listTeacher = new List<Teacher>();
        }

        [HttpGet("", Name = "GetListGV")]
        public async Task<ActionResult<ResponseInfo>> GetListGV()
        {
            ResponseInfo response = new ResponseInfo();
            try
            {
                var client = new MongoClient("mongodb+srv://kanseidemo123:kanseidemo123@cluster0.eetjn7s.mongodb.net/?retryWrites=true&w=majority");
                var database = client.GetDatabase("Kansei");
                var teacherTable = database.GetCollection<Teacher>("Teacher");
                List<Teacher> teachers = teacherTable.Find(new BsonDocument()).ToList();

                response.statusCode = System.Net.HttpStatusCode.OK;
                response.data = teachers;
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
