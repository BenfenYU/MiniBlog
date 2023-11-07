using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace MiniBlog.Model
{
    public class Article
    {
        public Article()
        {
        }

        public Article(string userName, string title, string content)
        {
            Id = Guid.NewGuid().ToString();
            UserName = userName;
            Title = title;
            Content = content;
        }

        public static string CollectionName { get; set; } = "Article";

        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? Id { get; set; }
        public string UserName { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string? UserId { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is Article article &&
                   Id == article.Id &&
                   UserName == article.UserName &&
                   Title == article.Title &&
                   Content == article.Content &&
                   UserId == article.UserId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, UserName, Title, Content, UserId);
        }
    }
}