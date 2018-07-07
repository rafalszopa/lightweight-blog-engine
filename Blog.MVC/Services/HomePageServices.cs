﻿using Blog.Core;
using Blog.Core.Models;
using Blog.Persistance;
using System.Collections.Generic;
using System.Linq;

namespace Blog.MVC.Services
{
    public class HomePageServices : IHomePageServices
    {
        IUnityOfWork unitOfWork;

        public HomePageServices()
        {
            /*his.unitOfWork = new UnityOfWork("connectionString");*/
        }

        public Post Foo()
        {
            using (var unitOfWork = new UnityOfWork("connectionString"))
            {
                var post = unitOfWork.PostRepository.FindById(1);
                post.Tags = unitOfWork.TagRepository.GetTagsByPostId(1).ToList();
                post.Details = unitOfWork.PostDetailsRepository.GetById(1);

                return post;
            }
        }

        public IEnumerable<Post> GetPosts()
        {
            //using (var unitOfWork = new UnityOfWork("connectionString"))
            //{
            //    var posts = unitOfWork.PostRepository.GetAll();

            //    foreach(var post in posts)
            //    {
            //        post.Tags = unitOfWork.TagRepository.GetTagsByPostId(post.Id).ToList();
            //    }

            //    return posts;
            //}
            return null;
        }
    }

    public class AppSettings
    {
        public int NumberOfHomePosts = 5;
    }
}
