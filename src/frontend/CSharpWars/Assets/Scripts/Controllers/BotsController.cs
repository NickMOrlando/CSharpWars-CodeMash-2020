using Assets.Scripts.Networking;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class BotsController : MonoBehaviour
    {
        public float RefreshTime = 2.0f;
        public float PollRate = 2.0f;
        private Dictionary<Guid, BotController> _bots = new Dictionary<Guid, BotController>();

        public GameObject BotPrefab;

        public void Start()
        {
            InvokeRepeating(nameof(RefreshBots), RefreshTime, PollRate);
        }
        private void RefreshBots()
        {
            var bots = ApiClient.GetRobots();
            bots.ForEach(x =>
            {
                if (_bots.ContainsKey(x.Id))
                {
                    var newBot = Instantiate(BotPrefab);
                    newBot.transform.parent = transform;
                    newBot.name = $"Bot-{x.PlayerName}-{x.Name}";
                    
                }
            });
        }
    }
}