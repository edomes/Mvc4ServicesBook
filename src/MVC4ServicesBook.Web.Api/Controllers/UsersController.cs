﻿using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MVC4ServicesBook.Data;
using MVC4ServicesBook.Web.Api.HttpFetchers;
using MVC4ServicesBook.Web.Api.Models;
using MVC4ServicesBook.Web.Api.TypeMappers;
using MVC4ServicesBook.Web.Common;

namespace MVC4ServicesBook.Web.Api.Controllers
{
    public class UsersController : ApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserManager _userManager;
        private readonly IUserMapper _userMapper;
        private readonly IHttpUserFetcher _userFetcher;

        public UsersController(
            IUserRepository userRepository, 
            IUserManager userManager, 
            IUserMapper userMapper,
            IHttpUserFetcher userFetcher)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _userMapper = userMapper;
            _userFetcher = userFetcher;
        }

        [Queryable]
        public IQueryable<Data.Model.User> Get()
        {
            return _userRepository.AllUsers();
        }

        [LoggingNHibernateSessions]
        public User Get(Guid id)
        {
            var user = _userFetcher.GetUser(id);
            return _userMapper.CreateUser(user);
        }

        [LoggingNHibernateSessions]
        public HttpResponseMessage Post(HttpRequestMessage request, User user)
        {
            var newUser = _userManager.CreateUser(user.Username, user.Password, user.Firstname, user.Lastname, user.Email);

            var href = newUser.Links.First(x => x.Rel == "self").Href;

            var response = request.CreateResponse(HttpStatusCode.Created, newUser);
            response.Headers.Add("Location", href);

            return response;
        }
    }
}
