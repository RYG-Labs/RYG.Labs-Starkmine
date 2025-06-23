using UnityEngine;
using System.Collections.Generic;

namespace _Project._Scripts.Game.Managers
{
    public class ObjectPool : MonoBehaviour
    {
        public static ObjectPool Instance;
        private Dictionary<string, Queue<GameObject>> poolDictionary = new();

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void CreatePool(GameObject prefab, int poolSize, Transform parent = null)
        {
            string key = prefab.name;
            if (!poolDictionary.ContainsKey(key))
            {
                poolDictionary[key] = new Queue<GameObject>();
                for (int i = 0; i < poolSize; i++)
                {
                    GameObject obj = Instantiate(prefab, transform);
                    obj.SetActive(false);
                    poolDictionary[key].Enqueue(obj);
                }
            }
        }

        public GameObject GetObject(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            string key = prefab.name;
            if (!poolDictionary.ContainsKey(key))
            {
                CreatePool(prefab, 10);
            }

            GameObject obj;
            if (poolDictionary[key].Count > 0)
            {
                obj = poolDictionary[key].Dequeue();
                obj.transform.position = position;
                obj.transform.rotation = rotation;
                obj.SetActive(true);
            }
            else
            {
                obj = Instantiate(prefab, position, rotation);
            }

            return obj;
        }

        public void ReturnObject(GameObject obj)
        {
            string key = obj.name.Replace("(Clone)", "");
            if (poolDictionary.ContainsKey(key))
            {
                obj.SetActive(false);
                poolDictionary[key].Enqueue(obj);
            }
            else
            {
                Destroy(obj);
            }
        }
    }
}