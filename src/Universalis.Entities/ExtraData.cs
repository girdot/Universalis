﻿using MongoDB.Bson.Serialization.Attributes;

namespace Universalis.Entities
{
    public class ExtraData
    {
        [BsonElement("setName")]
        public string SetName { get; }

        public ExtraData(string setName)
        {
            SetName = setName;
        }
    }
}