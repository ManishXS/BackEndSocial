using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using System.Net;
using BackEnd.ViewModels;
using BackEnd.Entities;
using Azure.Storage.Blobs;

namespace BackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly CosmosDbContext _dbContext;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _feedContainer = "media";  // Blob container for storing feeds

        public AccountController(CosmosDbContext dbContext, BlobServiceClient blobServiceClient)
        {
            _dbContext = dbContext;
            _blobServiceClient = blobServiceClient;
        }


        [Route("Register")]
        [HttpPost]
        public async Task<IActionResult> Register(AccountRegisterViewModel m)
        {

            var username = m.Username.Trim().ToLower();

            var user = new BlogUser
            {
                UserId = Guid.NewGuid().ToString(),
                Username = username
            };

            try
            {
                var uniqueUsername = new UniqueUsername { Username = user.Username };

                await _dbContext.UsersContainer.CreateItemAsync<UniqueUsername>(uniqueUsername, new PartitionKey(uniqueUsername.UserId));
                user.Action = "Create";

                await _dbContext.UsersContainer.CreateItemAsync<BlogUser>(user, new PartitionKey(user.UserId));

                m.Message = $"User has been created.";
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
            {

                throw ex;
            }

            return Ok(m);
        }


        [Route("Login")]
        [HttpPost]
        public async Task<IActionResult> Login(AccountLoginViewModel m)
        {
            var username = m.Username.Trim().ToLower();


            var queryDefinition = new QueryDefinition("SELECT * FROM u WHERE u.type = 'user' AND u.username = @username").WithParameter("@username", username);

            var query = _dbContext.UsersContainer.GetItemQueryIterator<BlogUser>(queryDefinition);

            List<BlogUser> results = new List<BlogUser>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();

                results.AddRange(response.ToList());
            }

            if (results.Count > 1)
            {
                throw new Exception($"More than one user found for username '{username}'");
            }

            var u = results.SingleOrDefault();

            return Ok(u);
        }
    }
}