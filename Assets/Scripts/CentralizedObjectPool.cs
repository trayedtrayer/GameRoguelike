using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;

[System.Serializable]
public class PoolItem
{
    [Tooltip("Префаб объекта для пула")]
    public GameObject prefabPool;
    [Tooltip("Размер пула для префаба")]
    public int poolSize;
    [Tooltip("Размер максимального пула для префаба")]
    public int maxPoolSize;
}

public class CentralizedObjectPool : MonoBehaviour
{
    public static CentralizedObjectPool instancePool;
    [SerializeField]
    [Tooltip("Объекты которые будут пулиться")]
    private List<PoolItem> itemsPool;
    private Dictionary<GameObject, ObjectPool<GameObject>> pools;
    private void Awake()
    {
        instancePool = this;
        InitializePools();
        print(pools.Count);
    }

   
    private void InitializePools()
    {
        pools = new Dictionary<GameObject, ObjectPool<GameObject>>();
        foreach (var item in itemsPool)
        {
            print(item.prefabPool.name);
            var pool = new ObjectPool<GameObject>(
                createFunc: () =>
                {
                    var instance = Instantiate(item.prefabPool);
                    instance.transform.SetParent(this.transform);
                    return instance;
                },
                actionOnGet: (obj) =>
                {
                    obj.SetActive(true);
                    obj.transform.SetParent(null);
                },
                actionOnRelease: (obj) =>
                {
                    obj.SetActive(false);
                    obj.transform.SetParent(this.transform, false);
                },
                actionOnDestroy: (obj) => Destroy(obj),
                collectionCheck: false,
                defaultCapacity: item.poolSize,
                maxSize: item.maxPoolSize
                );
            pools.Add(item.prefabPool, pool);
        }
    }
    public GameObject GetObject(GameObject prefabObj)
    {
        Debug.Log(1);
        if (pools.TryGetValue(prefabObj, out var pool))
        {
            return pool.Get();
        }
        Debug.LogError($"не найдено пула для {prefabObj.name}");
        return null;
    }
    public void ReturnObject(GameObject prefab, GameObject obj)
    {
        if(pools.TryGetValue(prefab, out var pool))
        {
            pool.Release(obj);
        }
        else
        {
            Debug.LogError($"не найдено пула для {prefab.name}");
        }
    }
}
