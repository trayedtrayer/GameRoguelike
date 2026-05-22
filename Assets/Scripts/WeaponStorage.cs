using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;

public class WeaponStorage : MonoBehaviour
{
    public static WeaponStorage InstanceWeaponStorage { get; private set; }

    [System.Serializable]
    public class StorageData
    {
        public List<string> weaponNames = new List<string>();
    }

    private StorageData data = new StorageData();
    private string filePath;

    public int Count => data.weaponNames.Count;
    public IReadOnlyList<string> WeaponNames => data.weaponNames;

    private void Awake()
    {
        if (InstanceWeaponStorage != null && InstanceWeaponStorage != this) { Destroy(gameObject); return; }
        InstanceWeaponStorage = this;
        DontDestroyOnLoad(gameObject);
        filePath = System.IO.Path.Combine(Application.persistentDataPath, "weapon_storage.xml"); ;
        Load();
    }

    public bool AddWeapon(string weaponName)
    {
        data.weaponNames.Add(weaponName);
        Save();
        Debug.Log($"[WeaponStorage] Добавлено: {weaponName}. Всего: {Count}");
        return true;
    }

    public bool TakeWeapon(string weaponName)
    {
        if (data.weaponNames.Remove(weaponName))
        {
            Save();
            Debug.Log($"[WeaponStorage] Взято: {weaponName}");
            return true;
        }
        return false;
    }

    public bool HasWeapon(string weaponName) => data.weaponNames.Contains(weaponName);

    public void Save()
    {
        try
        {
            var serializer = new XmlSerializer(typeof(StorageData));
            using (var writer = new StreamWriter(filePath))
                serializer.Serialize(writer, data);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[WeaponStorage] Ошибка сохранения: {e.Message}");
        }
    }

    public void Load()
    {
        if (!File.Exists(filePath)) { data = new StorageData(); return; }
        try
        {
            var serializer = new XmlSerializer(typeof(StorageData));
            using (var stream = new FileStream(filePath, FileMode.Open))
                data = serializer.Deserialize(stream) as StorageData ?? new StorageData();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[WeaponStorage] Ошибка загрузки: {e.Message}");
            data = new StorageData();
        }
    }
}