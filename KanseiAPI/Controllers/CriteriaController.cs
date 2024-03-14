using KanseiAPI.NewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace KanseiAPI.Controllers
{
    [Route("api/criteria")]
    [ApiController]
    public class CriteriaController : ControllerBase
    {
        [HttpGet("", Name = "GetAllCriteria")]
        public async Task<ActionResult<ResponseInfo>> getListCriteria()
        {
            ResponseInfo response = new ResponseInfo();
            try
            {
                var client = new MongoClient("mongodb+srv://kanseidemo123:kanseidemo123@cluster0.eetjn7s.mongodb.net/?retryWrites=true&w=majority");
                var database = client.GetDatabase("Kansei");
                var criteriaCollection = database.GetCollection<Criteria>("Criteria");
                var kanseiCollection = database.GetCollection<Kansei>("KanseiWord");
                var criterias = await criteriaCollection.Aggregate()
                    .Lookup(
                        foreignCollection: kanseiCollection,
                        localField: x => x.KanseiList,
                        foreignField: x => x.Id,
                        @as: (Criteria criteria) => criteria.ListKansei
                    )
                    .ToListAsync();

                response.statusCode = System.Net.HttpStatusCode.OK;
                response.data = criterias;
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
