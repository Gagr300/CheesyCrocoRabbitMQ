﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Rabbit.Models.Collections
{
    public class Test
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        public string name { get; set; } = "";

        public int rateSum { get; set; } = 0;

        public int rateNum { get; set; } = 0;
        public int questionsNum { get; set; } = 0;

        public int passCounter { get; set; } = 0;

        public string imagePath { get; set; } = "";
    }
}