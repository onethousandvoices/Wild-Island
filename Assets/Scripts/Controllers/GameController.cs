using System;
using UnityEngine;
using Zenject;

namespace WildIsland.Controllers
{
    public class GameController : IInitializable
    {
        [Inject] private IConsoleHandler _consoleHandler;
        
        public void Initialize()
        {
            _consoleHandler.SubscribeToLog();
            Debug.Log($"Started at {DateTime.Now}");
        }
    }
}