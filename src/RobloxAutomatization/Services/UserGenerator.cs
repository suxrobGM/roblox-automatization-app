using System;
using System.Linq;
using RobloxAutomatization.Models;

namespace RobloxAutomatization.Services
{
    public static class UserGenerator
    {
        public static RobloxUser GenerateUser()
        {
            var rnd = new Random();

            var user = new RobloxUser
            {
                Username = new string(Guid.NewGuid().ToString().Replace("-", "").Take(15).ToArray()),
                Password = Guid.NewGuid().ToString().Replace("-", ""),
                Gender = Gender.Male,
                Birthday = new DateTime(rnd.Next(1970, 2010), rnd.Next(1, 12), rnd.Next(1, 27))
            };

            return user;
        }

        public static RobloxUser GenerateUser(string username)
        {
            var rnd = new Random();

            var user = new RobloxUser
            {
                Username = $"{username}{rnd.Next(1000, 9999)}",
                Password = Guid.NewGuid().ToString().Replace("-", ""),
                Gender = Gender.Male,
                Birthday = new DateTime(rnd.Next(1970, 2010), rnd.Next(1, 12), rnd.Next(1, 27))
            };

            return user;
        }

        public static string GenerateOnlyUsername()
        {
            return new string(Guid.NewGuid().ToString().Replace("-", "").Take(15).ToArray());
        }

        public static string AttachRandomNumber(string username)
        {
            username = username.Substring(0, username.Length - 4);
            username = string.Concat(username, new Random().Next(1000, 9999));
            return username;
        }
    }
}
