using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WeaponUIManager : MonoBehaviour
{
    public static WeaponUIManager Instance { get; private set; }

    [Header("Primary Weapon Display")]
    public Image primaryWeaponImage;              // Ảnh hiện vũ khí chính đang chọn
    public Button useButton;                      // Nút sử dụng vũ khí
    public Button rentOutButton;                  // Nút thuê vũ khí

    [Header("Notification UI")]
    public TextMeshProUGUI notificationText;     // Text hiển thị thông báo trạng thái

    [Header("Weapon Content Container")]
    public Transform weaponContent;               // Container chứa các UI chọn vũ khí
    public GameObject weaponSelectPrefab;        // Prefab item UI chọn vũ khí

    [Header("Button Text and Colors")]
    public TextMeshProUGUI useButtonText;        // Text trên nút dùng vũ khí
    public TextMeshProUGUI rentOutButtonText;    // Text trên nút thuê vũ khí
    public Color useTextColor = new Color32(0, 255, 72, 255);    // Màu chữ xanh cho nút dùng
    public Color rentTextColor = new Color32(255, 135, 38, 255); // Màu chữ cam cho nút thuê
    public Color disableTextColor = Color.gray;                    // Màu chữ xám khi vô hiệu

    public SaveLoadManager saveLoadManager;      // Tham chiếu đến SaveLoadManager để lưu/load dữ liệu
    public WeaponInfo[] allWeapons;               // Mảng chứa tất cả vũ khí

    private WeaponInfo currentSelectedWeapon;    // Vũ khí đang được chọn
    private Coroutine notificationCoroutine;     // Coroutine dùng để hiện thông báo

    private void Awake()
    {
        // Thiết lập Singleton, đảm bảo chỉ có 1 instance
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        notificationText.gameObject.SetActive(false); // Ẩn thông báo lúc bắt đầu
    }

    private void Start()
    {
        // Load dữ liệu vũ khí từ file hoặc bộ nhớ
        saveLoadManager.LoadWeapons(allWeapons);

        // Tạo UI các vũ khí trong danh sách
        LoadWeaponsToUI();

        // Gán sự kiện cho các nút chức năng
        useButton.onClick.AddListener(OnClickUseWeaponButton);
        rentOutButton.onClick.AddListener(OnClickRentOutWeaponButton);
    }

    /// <summary>
    /// Tạo lại danh sách UI chọn vũ khí theo mảng allWeapons
    /// </summary>
    public void LoadWeaponsToUI()
    {
        // Xóa hết UI con cũ trong weaponContent trước
        foreach (Transform child in weaponContent)
            Destroy(child.gameObject);

        // Tạo UI cho từng vũ khí
        foreach (var w in allWeapons)
        {
            GameObject go = Instantiate(weaponSelectPrefab, weaponContent);
            WeaponSelect ws = go.GetComponent<WeaponSelect>();

            ws.weaponInfo = w;
            ws.gunImage.sprite = w.gunImage;
            ws.UpdateWeaponStatusUI();
        }
    }

    /// <summary>
    /// Cập nhật UI cho vũ khí được chọn
    /// </summary>
    public void SetCurrentWeapon(WeaponInfo info)
    {
        if (info == null) return;

        currentSelectedWeapon = info;
        primaryWeaponImage.sprite = info.gunImage;

        // Hiển thị thông tin chi tiết vũ khí
        WeaponInfoDisplay.Instance.DisplayWeaponInfo(info);

        UpdateButtonTexts();

        // Cập nhật màu sắc nút theo trạng thái vũ khí
        switch (info.state)
        {
            case WeaponState.Used:
                SetUseButtonActive(true);
                break;
            case WeaponState.RentedOut:
                SetRentOutButtonActive(true);
                break;
            default:
                ResetButtonTextColors();
                break;
        }
    }

    /// <summary>
    /// Xử lý khi nhấn nút "Use"
    /// </summary>
    private void OnClickUseWeaponButton()
    {
        if (currentSelectedWeapon == null) return;
        if (currentSelectedWeapon.state == WeaponState.RentedOut) return; // Không thể dùng vũ khí đang thuê

        if (currentSelectedWeapon.state == WeaponState.Used)
        {
            // Bỏ dùng vũ khí
            currentSelectedWeapon.state = WeaponState.Locked;
            ShowNotification($"Bạn đã bỏ sử dụng: {currentSelectedWeapon.gunName}");
            ResetButtonTextColors();
        }
        else
        {
            // Bỏ dùng tất cả vũ khí khác
            foreach (var w in allWeapons)
            {
                if (w.state == WeaponState.Used)
                    w.state = WeaponState.Locked;
            }

            // Đặt vũ khí hiện tại thành đang dùng
            currentSelectedWeapon.state = WeaponState.Used;
            ShowNotification($"Bạn đang sử dụng: {currentSelectedWeapon.gunName}");
            SetUseButtonActive(true);
        }

        // Cập nhật UI và lưu trạng thái
        UpdateAllWeaponUI();
        UpdateButtonTexts();
        saveLoadManager.SaveWeapons(allWeapons);
    }

    /// <summary>
    /// Xử lý khi nhấn nút "Rent Out"
    /// </summary>
    private void OnClickRentOutWeaponButton()
    {
        if (currentSelectedWeapon == null) return;
        if (currentSelectedWeapon.state == WeaponState.Used) return; // Không thể thuê vũ khí đang dùng

        if (currentSelectedWeapon.state == WeaponState.RentedOut)
        {
            // Bỏ thuê vũ khí
            currentSelectedWeapon.state = WeaponState.Locked;
            ShowNotification($"Bạn đã bỏ thuê: {currentSelectedWeapon.gunName}");
            ResetButtonTextColors();
        }
        else
        {
            // Thuê vũ khí
            currentSelectedWeapon.state = WeaponState.RentedOut;
            ShowNotification($"Bạn đã thuê: {currentSelectedWeapon.gunName}");
            SetRentOutButtonActive(true);
        }

        // Cập nhật UI và lưu trạng thái
        UpdateAllWeaponUI();
        UpdateButtonTexts();
        saveLoadManager.SaveWeapons(allWeapons);
    }

    /// <summary>
    /// Cập nhật trạng thái UI tất cả weapon select item trong container
    /// </summary>
    public void UpdateAllWeaponUI()
    {
        foreach (Transform child in weaponContent)
        {
            WeaponSelect ws = child.GetComponent<WeaponSelect>();
            if (ws != null)
                ws.UpdateWeaponStatusUI();
        }
    }

    /// <summary>
    /// Hiện thông báo cho người dùng trong 1 giây
    /// </summary>
    public void ShowNotification(string msg)
    {
        if (notificationCoroutine != null)
            StopCoroutine(notificationCoroutine);

        notificationCoroutine = StartCoroutine(ShowNotificationRoutine(msg));
    }

    private IEnumerator ShowNotificationRoutine(string msg)
    {
        notificationText.text = msg;
        notificationText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        notificationText.gameObject.SetActive(false);
    }

    // Các hàm phụ trợ để thay đổi màu sắc nút tương ứng trạng thái

    private void SetUseButtonActive(bool isActive)
    {
        useButtonText.color = useTextColor;
        rentOutButtonText.color = disableTextColor;
    }

    private void SetRentOutButtonActive(bool isActive)
    {
        useButtonText.color = disableTextColor;
        rentOutButtonText.color = rentTextColor;
    }

    private void ResetButtonTextColors()
    {
        useButtonText.color = useTextColor;
        rentOutButtonText.color = rentTextColor;
    }

    /// <summary>
    /// Cập nhật text trên 2 nút dựa trên trạng thái vũ khí hiện tại
    /// </summary>
    private void UpdateButtonTexts()
    {
        if (currentSelectedWeapon == null)
        {
            useButtonText.text = "Use";
            rentOutButtonText.text = "Rent Out";
            return;
        }

        useButtonText.text = (currentSelectedWeapon.state == WeaponState.Used) ? "Un-Use" : "Use";
        rentOutButtonText.text = (currentSelectedWeapon.state == WeaponState.RentedOut) ? "Un-Rent" : "Rent Out";
    }
}
