﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Entities
{
    public class UserPost
    {

        [JsonProperty(PropertyName = "id")]
        public string Id
        {
            get
            {
                return PostId;
            }
        }

        [JsonProperty(PropertyName = "postId")]
        public string PostId { get; set; }


        [JsonProperty(PropertyName = "type")]
        public string Type
        {
            get
            {
                return "post";
            }
        }

        [JsonProperty(PropertyName = "userId")]
        public string AuthorId { get; set; }

        [JsonProperty(PropertyName = "userUsername")]
        public string AuthorUsername { get; set; }


        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "caption")]
        public string Caption { get; set; }


        [JsonProperty(PropertyName = "content")]
        public string Content { get; set; }


        [JsonProperty(PropertyName = "commentCount")]
        public int CommentCount { get; set; }

        [JsonProperty(PropertyName = "likeCount")]
        public int LikeCount { get; set; }


        [JsonProperty(PropertyName = "dateCreated")]
        public DateTime DateCreated { get; set; }

        [JsonProperty(PropertyName = "likeFlag")]
        public int LikeFlag { get; set; }


        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }


    }
}
