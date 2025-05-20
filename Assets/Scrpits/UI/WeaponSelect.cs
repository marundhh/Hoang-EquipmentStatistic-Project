using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class WeaponSelect : MonoBehaviour
{
    [Header("UI Components")]
    public Button selectButton;
    public Image gunImage;
    public GameObject background;
    public GameObject backgroundOutline;
    public GameObject usedIcon;
    public GameObject rentedOutIcon;

    [Header("Weapon Info")]
    public WeaponInfo weaponInfo;

    private bool isSelected = false;

    // Danh sách WeaponSelect đang tồn tại để quản lý chọn/deselect
    private static List<WeaponSelect> allWeapons = new List<WeaponSelect>();

    private void Awake()
    {
        allWeapons.Add(this);
    }

    private void OnDestroy()
    {
        allWeapons.Remove(this);
    }

    private void Start()
    {
        // Gán ảnh vũ khí nếu có
        if (weaponInfo != null && gunImage != null)
            gunImage.sprite = weaponInfo.gunImage;

        // Đăng ký sự kiện click cho nút chọn vũ khí
        selectButton.onClick.AddListener(SelectWeapon);

        // Cập nhật icon trạng thái (Used / RentedOut)
        UpdateWeaponStatusUI();

        // Nếu đây là weapon Used đầu tiên thì chọn luôn, ngược lại deselect
        if (IsFirstUsedWeapon())
            SelectWeapon();
        else
            Deselect();
    }

    // Kiểm tra xem có phải weapon Used đầu tiên không
    private bool IsFirstUsedWeapon()
    {
        if (weaponInfo == null || weaponInfo.state != WeaponState.Used)
            return false;

        foreach (var ws in allWeapons)
        {
            if (ws.weaponInfo != null && ws.weaponInfo.state == WeaponState.Used)
                return ws == this;
        }
        return false;
    }

    // Chọn vũ khí này
    public void SelectWeapon()
    {
        if (isSelected) return;

        // Bỏ chọn các weapon khác
        foreach (var ws in allWeapons)
        {
            if (ws != this)
                ws.Deselect();
        }

        isSelected = true;
        background.SetActive(false);
        backgroundOutline.SetActive(true);

        // Cập nhật UI weapon được chọn
        if (weaponInfo != null)
        {
            WeaponUIManager.Instance.SetCurrentWeapon(weaponInfo);
        }
    }

    // Bỏ chọn vũ khí này
    public void Deselect()
    {
        if (!isSelected) return;

        isSelected = false;
        background.SetActive(true);
        backgroundOutline.SetActive(false);
    }

    // Cập nhật icon trạng thái theo state của weapon
    public void UpdateWeaponStatusUI()
    {
        if (weaponInfo == null) return;

        usedIcon.SetActive(weaponInfo.state == WeaponState.Used);
        rentedOutIcon.SetActive(weaponInfo.state == WeaponState.RentedOut);
    }
}
