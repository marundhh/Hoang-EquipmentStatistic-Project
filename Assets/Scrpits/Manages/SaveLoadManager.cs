using System.IO;
using UnityEngine;

// Class dữ liệu lưu trữ mỗi vũ khí
[System.Serializable]
public class WeaponSaveData
{
    public string weaponID;
    public int state;
    public int damage;
    public int dispersion;
    public int rateOfFire;
    public int reloadSpeed;
    public int ammunition;
}

// Wrapper lưu danh sách vũ khí (để serialize)
[System.Serializable]
public class WeaponSaveDataList
{
    public WeaponSaveData[] weapons;
}

// Quản lý lưu và tải dữ liệu
public class SaveLoadManager : MonoBehaviour
{
    private string saveFilePath;

    private void Awake()
    {
        // Đường dẫn lưu file trên thiết bị
        saveFilePath = Path.Combine(Application.persistentDataPath, "weaponsave.json");
    }

    // Lưu toàn bộ danh sách vũ khí hiện tại
    public void SaveWeapons(WeaponInfo[] weaponInfos)
    {
        WeaponSaveDataList saveData = new WeaponSaveDataList();
        saveData.weapons = new WeaponSaveData[weaponInfos.Length];

        for (int i = 0; i < weaponInfos.Length; i++)
        {
            WeaponInfo w = weaponInfos[i];
            saveData.weapons[i] = new WeaponSaveData
            {
                weaponID = w.weaponID,
                state = (int)w.state,
                damage = w.damage,
                dispersion = w.dispersion,
                rateOfFire = w.rateOfFire,
                reloadSpeed = w.reloadSpeed,
                ammunition = w.ammunition
            };
        }

        // Serialize ra JSON và ghi file
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Lưu vũ khí thành công tại: " + saveFilePath);
    }

    // Tải dữ liệu vũ khí từ file vào mảng WeaponInfo đã có
    public void LoadWeapons(WeaponInfo[] weaponInfos)
    {
        if (!File.Exists(saveFilePath))
        {
            Debug.LogWarning("Không tìm thấy file save, dùng dữ liệu mặc định.");
            return;
        }

        string json = File.ReadAllText(saveFilePath);
        WeaponSaveDataList loadedData = JsonUtility.FromJson<WeaponSaveDataList>(json);

        if (loadedData == null || loadedData.weapons == null)
        {
            Debug.LogError("Dữ liệu save bị lỗi hoặc rỗng");
            return;
        }

        // Map dữ liệu load vào WeaponInfo tương ứng theo weaponID
        foreach (var saveWeapon in loadedData.weapons)
        {
            foreach (var w in weaponInfos)
            {
                if (w.weaponID == saveWeapon.weaponID)
                {
                    w.state = (WeaponState)saveWeapon.state;
                    w.damage = saveWeapon.damage;
                    w.dispersion = saveWeapon.dispersion;
                    w.rateOfFire = saveWeapon.rateOfFire;
                    w.reloadSpeed = saveWeapon.reloadSpeed;
                    w.ammunition = saveWeapon.ammunition;
                    break;
                }
            }
        }

        Debug.Log("Load vũ khí thành công.");
    }
}
