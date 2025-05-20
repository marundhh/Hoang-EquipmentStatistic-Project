using UnityEngine;

public class WeaponSpawner : MonoBehaviour
{
    public GameObject weaponUIPrefab;  // Prefab UI WeaponSelect
    public Transform parentLayout;     // Nơi chứa UI WeaponSelect
    public WeaponInfo[] weaponInfos;   // Danh sách weapon data

    private void Start()
    {
        WeaponSelect firstUsed = null;

        // Tạo UI WeaponSelect từ dữ liệu weaponInfos
        foreach (var info in weaponInfos)
        {
            GameObject go = Instantiate(weaponUIPrefab, parentLayout);
            WeaponSelect ws = go.GetComponent<WeaponSelect>();
            ws.weaponInfo = info;
            ws.UpdateWeaponStatusUI();

            // Lưu weapon Used đầu tiên để chọn sau cùng
            if (firstUsed == null && info.state == WeaponState.Used)
                firstUsed = ws;
        }

        // Chọn weapon Used đầu tiên (nếu có)
        firstUsed?.SelectWeapon();
    }
}
