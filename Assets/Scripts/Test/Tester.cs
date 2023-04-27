using System;
using System.Collections.Generic;
using TMPro;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

namespace WildIsland.Test
{
    public class Tester : MonoBehaviour
    {
        [SerializeField] private bool _useJobs;
        [SerializeField] private bool _isParenting;
        [SerializeField] private Transform _entity;
        [SerializeField] private Transform _pool;
        [SerializeField] private TextMeshProUGUI _count;

        private readonly List<Entity> _entitiesList = new List<Entity>();

        private class Entity
        {
            public Transform transform;
            public float moveY;
        }

        public void UnityEvent_Create10()
            => Create(10);

        public void UnityEvent_Create100()
            => Create(100);

        public void UnityEvent_Create1K()
            => Create(1000);

        public void UnityEvent_Clear()
        {
            for (int i = 0; i < _entitiesList.Count; i++)
            {
                Entity entity = _entitiesList[i];

                try
                {
                    Destroy(entity.transform.gameObject);
                    DestroyImmediate(entity.transform.gameObject);
                }
                catch (Exception e)
                {
                    // ignored
                }
            }

            _entitiesList.Clear();
            _count.text = 0.ToString();
        }

        public void UnityEvent_ChangeParentingState(bool state)
            => _isParenting = state;

        private void Create(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                Transform entity = Instantiate(_entity, new Vector3(UnityEngine.Random.Range(-8f, 8f), UnityEngine.Random.Range(-5f, 5f)), Quaternion.identity);
                if (_isParenting)
                    entity.parent = _pool;
                _entitiesList.Add(new Entity { transform = entity, moveY = UnityEngine.Random.Range(1f, 2f) });
            }

            _count.text = _entitiesList.Count.ToString();
        }

        private void Update()
        {
            NativeArray<float> moveYArray = new NativeArray<float>(_entitiesList.Count, Allocator.TempJob);
            TransformAccessArray transformAccessArray = new TransformAccessArray(_entitiesList.Count);

            for (int i = 0; i < _entitiesList.Count; i++)
            {
                //positionArray[i] = zombieList[i].transform.position;
                moveYArray[i] = _entitiesList[i].moveY;
                transformAccessArray.Add(_entitiesList[i].transform);
            }

            /*
            ReallyToughParallelJob reallyToughParallelJob = new ReallyToughParallelJob {
                deltaTime = Time.deltaTime,
                positionArray = positionArray,
                moveYArray = moveYArray,
            };

            JobHandle jobHandle = reallyToughParallelJob.Schedule(zombieList.Count, 100);
            jobHandle.Complete();
            */
            ReallyToughParallelJobTransforms reallyToughParallelJobTransforms = new ReallyToughParallelJobTransforms { deltaTime = Time.deltaTime, moveYArray = moveYArray, };

            JobHandle jobHandle = reallyToughParallelJobTransforms.Schedule(transformAccessArray);
            jobHandle.Complete();

            for (int i = 0; i < _entitiesList.Count; i++)
            {
                //zombieList[i].transform.position = positionArray[i];
                _entitiesList[i].moveY = moveYArray[i];
            }

            //positionArray.Dispose();
            moveYArray.Dispose();
            transformAccessArray.Dispose();
        }
    }

    [BurstCompile]
    public struct ReallyToughParallelJobTransforms : IJobParallelForTransform
    {
        public NativeArray<float> moveYArray;
        [ReadOnly] public float deltaTime;

        public void Execute(int index, TransformAccess transform)
        {
            transform.position += new Vector3(0, moveYArray[index] * deltaTime, 0f);
            moveYArray[index] = transform.position.y switch

                                {
                                    > 5f  => -math.abs(moveYArray[index]),
                                    < -5f => +math.abs(moveYArray[index]),
                                    _     => moveYArray[index]
                                };
        }
    }
}