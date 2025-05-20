using UnityEngine;

// Các trạng thái của vũ khí
public enum WeaponState
{
    Locked,     // Khóa
    RentedOut,  // Đang thuê
    Used        // Đang sử dụng
}

[CreateAssetMenu(fileName = "NewGunInfo", menuName = "GunInfo/GunInfo")]
public class WeaponInfo : ScriptableObject
{
    [Header("ID và Tên")]
    public string weaponID;
    public string gunName;

    [Header("Trạng thái và Ảnh")]
    public WeaponState state = WeaponState.Locked;
    public Sprite gunImage;

    [Header("Chỉ số chiến đấu")]
    public int damage;
    public int dispersion;
    public int rateOfFire;
    public int reloadSpeed;
    public int ammunition;
}
