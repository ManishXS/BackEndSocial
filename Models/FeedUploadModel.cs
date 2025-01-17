﻿using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace BackEnd.Models
{
    public class FeedUploadModel
    {
        [JsonPropertyName("userId")]
        public string UserId { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("fileName")]
        public string FileName { get; set; } = string.Empty; 

        [JsonPropertyName("contentType")]
        public string ContentType { get; set; } = string.Empty;

        [JsonPropertyName("fileSize")]
        public long FileSize { get; set; }

        [JsonPropertyName("file")]
        public IFormFile File { get; set; } = null!; 

        [JsonPropertyName("profilePic")]
        public string ProfilePic { get; set; } = string.Empty;

        [JsonPropertyName("userName")]
        public string UserName { get; set; } = string.Empty;

        [JsonPropertyName("caption")]
        public string Caption { get; set; } = string.Empty;

        // Chunk-specific fields
        [JsonPropertyName("uploadId")]
        public string UploadId { get; set; } = string.Empty;

        [JsonPropertyName("chunkIndex")]
        public int ChunkIndex { get; set; }

        [JsonPropertyName("totalChunks")]
        public int TotalChunks { get; set; }
    }
}
