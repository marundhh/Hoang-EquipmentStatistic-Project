using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

public class WeaponInfoDisplay : MonoBehaviour
{
    public static WeaponInfoDisplay Instance { get; private set; }

    private WeaponInfo currentSelectedWeapon;

    [Header("Gun Information Display")]
    public TextMeshProUGUI gunNameText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI dispersionText;
    public TextMeshProUGUI rateOfFireText;
    public TextMeshProUGUI reloadSpeedText;
    public TextMeshProUGUI ammunitionText;

    [Header("Upgrade Buttons")]
    public Button upgradeBndButton;
    public Button upgradeEwarButton;

    private string saveFilePath;
    private List<WeaponUpgradeData> upgradeDataList = new List<WeaponUpgradeData>();

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Đường dẫn file lưu dữ liệu nâng cấp (JSON)
        saveFilePath = Path.Combine(Application.persistentDataPath, "weaponUpgrades.json");
        LoadUpgradeData();
    }

    private void Start()
    {
        // 2 nút nâng cấp gọi chung một hàm
        upgradeBndButton.onClick.AddListener(() => UpgradeCurrentWeapon());
        upgradeEwarButton.onClick.AddListener(() => UpgradeCurrentWeapon());
    }

    // Hiển thị thông tin vũ khí lên UI
    public void DisplayWeaponInfo(WeaponInfo info)
    {
        if (info == null) return;

        currentSelectedWeapon = info;

        // Áp dụng dữ liệu nâng cấp nếu có
        ApplyUpgradeDataToWeapon(info);

        // Cập nhật text UI
        gunNameText.text = info.gunName;
        damageText.text = info.damage.ToString();
        dispersionText.text = info.dispersion.ToString();
        rateOfFireText.text = info.rateOfFire + " RPM";
        reloadSpeedText.text = info.reloadSpeed + "%";
        ammunitionText.text = info.ammunition + "/100";
    }

    // Nâng cấp vũ khí hiện tại (tăng tất cả chỉ số)
    private void UpgradeCurrentWeapon()
    {
        if (currentSelectedWeapon == null) return;

        currentSelectedWeapon.damage += 1;
        currentSelectedWeapon.dispersion += 1;
        currentSelectedWeapon.rateOfFire += 1;
        currentSelectedWeapon.reloadSpeed += 1;
        currentSelectedWeapon.ammunition += 1;

        // Lưu dữ liệu nâng cấp
        SaveUpgradeDataForWeapon(currentSelectedWeapon);

        // Cập nhật hiển thị mới
        DisplayWeaponInfo(currentSelectedWeapon);

        // Cập nhật UI WeaponSelect nếu cần
        WeaponUIManager.Instance.UpdateAllWeaponUI();
    }

    // Load dữ liệu nâng cấp từ file JSON
    private void LoadUpgradeData()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            WeaponUpgradeDataArray loadedData = JsonUtility.FromJson<WeaponUpgradeDataArray>(json);
            if (loadedData != null && loadedData.weaponUpgrades != null)
                upgradeDataList = new List<WeaponUpgradeData>(loadedData.weaponUpgrades);
        }
        else
        {
            Debug.Log("Không tìm thấy file nâng cấp, sử dụng dữ liệu mặc định.");
            upgradeDataList = new List<WeaponUpgradeData>();
        }
    }

    // Lưu dữ liệu nâng cấp ra file JSON
    private void SaveUpgradeData()
    {
        WeaponUpgradeDataArray dataArray = new WeaponUpgradeDataArray();
        dataArray.weaponUpgrades = upgradeDataList.ToArray();

        string json = JsonUtility.ToJson(dataArray, true);
        File.WriteAllText(saveFilePath, json);
    }

    // Lưu dữ liệu nâng cấp cho một vũ khí cụ thể
    private void SaveUpgradeDataForWeapon(WeaponInfo weapon)
    {
        WeaponUpgradeData data = upgradeDataList.Find(w => w.weaponID == weapon.weaponID);
        if (data == null)
        {
            data = new WeaponUpgradeData();
            data.weaponID = weapon.weaponID;
            upgradeDataList.Add(data);
        }

        data.damage = weapon.damage;
        data.dispersion = weapon.dispersion;
        data.rateOfFire = weapon.rateOfFire;
        data.reloadSpeed = weapon.reloadSpeed;
        data.ammunition = weapon.ammunition;

        SaveUpgradeData();
    }

    // Áp dụng dữ liệu nâng cấp đã lưu vào WeaponInfo
    private void ApplyUpgradeDataToWeapon(WeaponInfo weapon)
    {
        WeaponUpgradeData data = upgradeDataList.Find(w => w.weaponID == weapon.weaponID);
        if (data != null)
        {
            weapon.damage = data.damage;
            weapon.dispersion = data.dispersion;
            weapon.rateOfFire = data.rateOfFire;
            weapon.reloadSpeed = data.reloadSpeed;
            weapon.ammunition = data.ammunition;
        }
    }

    [System.Serializable]
    private class WeaponUpgradeDataArray
    {
        public WeaponUpgradeData[] weaponUpgrades;
    }
}

[System.Serializable]
public class WeaponUpgradeData
{
    public string weaponID;
    public int damage;
    public int dispersion;
    public int rateOfFire;
    public int reloadSpeed;
    public int ammunition;
}
