using System;
using System.Collections;
using UnityEngine;
using Zenject;

namespace WildIsland.Test
{
    public class Spawner : MonoBehaviour
    {
        [Inject] private EntityFactory _factory;
        
        private IEnumerator Start()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                _factory.Create();
            }

        }
    }
}