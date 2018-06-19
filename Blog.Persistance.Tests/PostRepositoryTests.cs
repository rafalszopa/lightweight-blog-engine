﻿using Blog.Core.Models;
using Blog.Core.Repository;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Blog.Persistance.Tests
{
    [TestFixture]
    class PostRepositoryTests
    {
        private string populateTestData;

        private IPostRepository postRepository;

        private TagRepository tagRepository;

        private User teslaUser;

        private User einsteinUser;

        private Post correctPost;

        public PostRepositoryTests()
        {
            this.populateTestData = File.ReadAllText(@"E:\Projects\lightweight-blog-engine\Blog.Persistance.Tests\SqlScripts\PopulateTestData.sql");

            this.postRepository = new PostRepository();

            this.tagRepository = new TagRepository();

            this.teslaUser = new User
                (1, "Nikola", "Tesla", DateTime.Parse("2000-01-01"), "nikola@tesla.com", "Electrical engineer and inventor", UserType.Author, true);

            this.einsteinUser = new User
                (2, "Albert", "Einstein", DateTime.Parse("2001-01-01"), "albert@einstein.com", "German-born theoretical physicist", UserType.Author, true);

            this.correctPost = new Post(
                "Vary clever post",
                "Post description",
                PostStatus.Live,
                DateTime.Parse("2000-01-01"),
                DateTime.Parse("2000-01-01"),
                "image.jpeg",
                new List<Tag>()
                {
                    new Tag("programming"),
                    new Tag("C#")
                },
                new PostDetails()
                {
                    Content = "<h1>Post headline!</h1>"
                },
                this.teslaUser);
        }

        [SetUp]
        public void Init()
        {
            DatabaseHandler.ClearDatabase();
            DatabaseHandler.ExecuteSqlScript(this.populateTestData);
        }

        [Test]
        public void InsertAndThenSelectPost_CorrectPost_ShouldPopulatePostInDatabaseAndThenReturn()
        {
            // Arrange
            var postId = this.postRepository.Add(this.correctPost);

            // Act
            var resultPost = this.postRepository.GetFullPostById(postId);

            // Assert
            Assert.AreEqual(postId, resultPost.Id);
            Assert.AreEqual(this.correctPost.Title, resultPost.Title);
            Assert.AreEqual(this.correctPost.Description, resultPost.Description);
            Assert.AreEqual(this.correctPost.Status, resultPost.Status);
            Assert.AreEqual(this.correctPost.CreateDate, resultPost.CreateDate);
            Assert.AreEqual(this.correctPost.PublishDate, resultPost.PublishDate);
            Assert.AreEqual(this.correctPost.PhotoUrl, resultPost.PhotoUrl);
            Assert.AreEqual(this.correctPost.Details.Content, resultPost.Details.Content);
            Assert.AreEqual(this.correctPost.Tags[0].Name, resultPost.Tags[0].Name);
            Assert.AreEqual(this.correctPost.Tags[0].Count, resultPost.Tags[0].Count);
            Assert.AreEqual(this.correctPost.Tags[1].Name, resultPost.Tags[1].Name);
            Assert.AreEqual(this.correctPost.Tags[1].Count, resultPost.Tags[1].Count);
            Assert.AreEqual(this.correctPost.Details.Content, resultPost.Details.Content);
            Assert.AreEqual(this.correctPost.Details.Content, resultPost.Details.Content);
            Assert.AreEqual(this.correctPost.Author.FirstName, resultPost.Author.FirstName);
            Assert.AreEqual(this.correctPost.Author.LastName, resultPost.Author.LastName);
            Assert.AreEqual(this.correctPost.Author.Email, resultPost.Author.Email);
            Assert.AreEqual(this.correctPost.Author.Bio, resultPost.Author.Bio);
            Assert.AreEqual(this.correctPost.Author.Type, resultPost.Author.Type);
        }

        [Test]
        public void InsertPost_InsertIncorrectPost_ShouldThrowException()
        {

        }

        [Test]
        public void InsertPost_ShouldIncreaseTagsCount()
        {
            // Arrange
            var tags = this.tagRepository.GetAll();
            Assert.AreEqual(4, tags.Count());
            Assert.AreEqual(1, tags.Where(o => o.Name == "agile").Select(o => o.Count));
            Assert.AreEqual(1, tags.Where(o => o.Name == "javascript").Select(o => o.Count));
            Assert.AreEqual(1, tags.Where(o => o.Name == "programming").Select(o => o.Count));
            Assert.AreEqual(1, tags.Where(o => o.Name == "vue.js").Select(o => o.Count));

            // Act
            this.postRepository.Add(this.correctPost);
            tags = this.tagRepository.GetAll();

            // Assert
            Assert.AreEqual(5, tags.Count());
            Assert.AreEqual(2, tags.Where(o => o.Name == "programming").Select(o => o.Count));
            Assert.AreEqual(1, tags.Where(o => o.Name == "C#").Select(o => o.Count));
        }

        [Test]
        public void UpdatePost_ShouldDecreaseTagsCount()
        {
            // Arrange
            var post = this.postRepository.GetById(1);
            var tags = this.tagRepository.GetAll();
            Assert.AreEqual(4, tags.Count());
            Assert.AreEqual(1, tags.Where(o => o.Name == "agile").Select(o => o.Count));
            Assert.AreEqual(1, tags.Where(o => o.Name == "javascript").Select(o => o.Count));
            Assert.AreEqual(1, tags.Where(o => o.Name == "programming").Select(o => o.Count));
            Assert.AreEqual(1, tags.Where(o => o.Name == "vue.js").Select(o => o.Count));

            // Act
            post.Tags.RemoveAt(0);
            this.postRepository.Update(post);
            tags = this.tagRepository.GetAll();

            Assert.AreEqual(3, tags.Count());
            Assert.AreEqual(null, tags.Where(o => o.Name == "agile"));
        }

        [Test]
        public void UpdatePost_RemoveAllTags_ShouldRemove()
        {
            // Arrange
            var post = this.postRepository.GetFullPostById(1);
            Assert.AreEqual(4, post.Tags.Count);
            post.Tags = new List<Tag>();

            // Act
            this.postRepository.Update(post);
            var result = this.postRepository.GetFullPostById(1);

            // Assert
            Assert.AreEqual(0, result.Tags.Count);
        }

        [Test]
        public void UpdatePost_ShouldUpdatePost()
        {
            // Arrange
            const string updatedValue = "updated value";
            const PostStatus updatedStatus = PostStatus.Draft;
            this.correctPost.Id = 1;
            this.correctPost.Title = updatedValue;
            this.correctPost.PhotoUrl = updatedValue;
            this.correctPost.Status = updatedStatus;

            // Act
            this.postRepository.Update(this.correctPost);
            var result = this.postRepository.GetFullPostById(1);

            // Assert
            Assert.AreEqual(updatedValue, result.Title);
            Assert.AreEqual(updatedValue, result.PhotoUrl);
            // Author must not be updated
            Assert.AreNotEqual(this.correctPost.Author.FirstName, result.Author.FirstName);
        }
    }
}
