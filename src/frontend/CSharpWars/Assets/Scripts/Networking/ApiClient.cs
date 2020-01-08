using Assets.Scripts.Model;
using RestSharp;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Networking
{
    public static class ApiClient
    {
        private static string _baseUrl = "http://csharpwars-api.azurefd.net/api/";
        internal static Arena GetArena()
        {
            var client = new RestClient(_baseUrl);
            var request = new RestRequest("arena", Method.GET);
            var response = client.Execute<Arena>(request);
            return response.Data;
        }
        internal static List<Bot> GetRobots()
        {
            var client = new RestClient(_baseUrl);
            var request = new RestRequest("bots", Method.GET);
            var response = client.Execute<List<Bot>>(request);
            return response.Data;
        }
    }
}