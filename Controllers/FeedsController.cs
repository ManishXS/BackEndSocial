﻿using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using StackExchange.Redis;
using BackEnd.Entities;
using BackEnd.Models;
using BackEnd.ViewModels;

namespace BackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedsController : ControllerBase
    {
        private readonly CosmosDbContext _dbContext;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _feedContainer = "media";
        private readonly string _cdnBaseUrl = "https://xxxxxxxxxxxxx./media/";
        private readonly IDatabase _redis;
        private readonly ILogger<FeedsController> _logger;

        public FeedsController(CosmosDbContext dbContext, BlobServiceClient blobServiceClient,  ILogger<FeedsController> logger)
        {
            _dbContext = dbContext;
            _blobServiceClient = blobServiceClient;
            _logger = logger;
        }

        [HttpPost("uploadFeed")]
        public async Task<IActionResult> UploadFeed([FromForm] FeedUploadModel model)
        {
            try
            {

                if (model.File == null || string.IsNullOrEmpty(model.UserId) || string.IsNullOrEmpty(model.FileName))
                {
                    return BadRequest("Missing required fields.");
                }

                var containerClient = _blobServiceClient.GetBlobContainerClient(_feedContainer);

                var blobName = $"{ShortGuidGenerator.Generate()}{model.File.FileName}";
                var blobClient = containerClient.GetBlobClient(blobName);

                await using (var stream = model.File.OpenReadStream())
                await using (var blobStream = await blobClient.OpenWriteAsync(overwrite: true))
                {
                    await stream.CopyToAsync(blobStream); // Stream file data in chunks
                }

                var blobUrl = blobClient.Uri.ToString();

                var userPost = new UserPost
                {
                    PostId = Guid.NewGuid().ToString(),
                    Title = model.ProfilePic,
                    Content = _cdnBaseUrl + "" + blobName,
                    Caption = model.Caption,
                    AuthorId = model.UserId,
                    AuthorUsername = model.UserName,
                    DateCreated = DateTime.UtcNow,
                };
                await _dbContext.PostsContainer.UpsertItemAsync<UserPost>(userPost, new PartitionKey(userPost.PostId));


                return Ok(new { Message = "Feed uploaded successfully.", FeedId = userPost.PostId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error uploading feed: {ex.Message}");
            }
        }

        [HttpGet("getUserFeeds")]
        public async Task<IActionResult> GetUserFeeds(string? userId = null, int pageNumber = 1, int pageSize = 2) // Default pageSize set to 2
        {
            try
            {
                var userPosts = new List<UserPost>();
                var queryString = $"SELECT * FROM f WHERE f.type='post' ORDER BY f.dateCreated DESC OFFSET {(pageNumber - 1) * pageSize} LIMIT {pageSize}"; // Pagination logic


                    var queryFromPostsContainer = _dbContext.PostsContainer.GetItemQueryIterator<UserPost>(new QueryDefinition(queryString));
                    while (queryFromPostsContainer.HasMoreResults)
                    {
                        var response = await queryFromPostsContainer.ReadNextAsync();
                        userPosts.AddRange(response.ToList());
                    }

                if (!string.IsNullOrEmpty(userId))
                {
                    var userLikes = new List<UserPost>();
                    foreach (var item in userPosts)
                    {
                        userLikes = new List<UserPost>();

                       var queryString_likes = $"SELECT  * FROM f WHERE f.type='like' and f.postId='pid' and f.userId='uid'";
                        queryString_likes = queryString_likes.Replace("uid", userId);
                        queryString_likes = queryString_likes.Replace("pid", item.PostId);
                        var queryFromPostsContainter_likes = _dbContext.PostsContainer.GetItemQueryIterator<UserPost>(new QueryDefinition(queryString_likes));
                        while (queryFromPostsContainter_likes.HasMoreResults)
                        {
                            var response = await queryFromPostsContainter_likes.ReadNextAsync();
                            var ru = response.RequestCharge;
                            userLikes.AddRange(response.ToList());
                        }
                        if (userLikes.Count > 0)
                        {
                            item.LikeFlag = 1;
                        }
                    }
                 
                }

                return Ok(new { BlogPostsMostRecent = userPosts }); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving feeds: {ex.Message}");
            }
        }

    }
}
