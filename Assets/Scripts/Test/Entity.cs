using System;
using UnityEngine;
using Zenject;

namespace WildIsland.Test
{
    public class Entity : MonoBehaviour
    {
        private Injected _lol;

        [Inject]
        public void Create(Injected lol)
        {
            _lol = lol;
        }

        private void Update()
        {
            Debug.LogError(name + " " + _lol.Name);
        }
    }

    public class EntityFactory : PlaceholderFactory<Entity> { }
}