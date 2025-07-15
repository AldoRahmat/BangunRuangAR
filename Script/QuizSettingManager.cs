using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using Unity.Services.CloudSave.Models;

public class QuizSettingManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Toggle useTimerToggle;
    public TMP_Dropdown timerDropdown;
    public TMP_InputField jumlahSoalInput;
    public Toggle acakToggle;
    public Button saveButton;
    public Button backButton;
    
    [Header("Pop-up Elements")]
    public GameObject popupPanel;
    public TextMeshProUGUI popupText;
    public Button popupOkButton;

    async void Awake()
    {
        // Memastikan UnityServices diinisialisasi
        if (!UnityServices.State.Equals(ServicesInitializationState.Initialized))
            await UnityServices.InitializeAsync();

        // Cek apakah user sudah login
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            Debug.LogError("User belum login. Arahkan ke halaman login.");
            return;
        }

        // Menambahkan listener untuk tombol simpan
        if (saveButton != null)
            saveButton.onClick.AddListener(() => ValidateAndSaveSettings());
            
        if (backButton != null)
            backButton.onClick.AddListener(() => GoBack());

        // Menambahkan listener untuk tombol OK pada pop-up
        if (popupOkButton != null)
            popupOkButton.onClick.AddListener(() => ClosePopup());

        // Mengisi dropdown timer (5 - 60 menit per 5)
        timerDropdown.ClearOptions();
        List<string> timerOptions = new List<string>();
        for (int i = 5; i <= 60; i += 5)
            timerOptions.Add(i.ToString());
        timerDropdown.AddOptions(timerOptions);

        // Atur dropdown aktif/tidak sesuai toggle
        useTimerToggle.onValueChanged.AddListener(OnToggleTimerChanged);
        OnToggleTimerChanged(useTimerToggle.isOn);
        
        // Pastikan pop-up tersembunyi di awal
        if (popupPanel != null)
            popupPanel.SetActive(false);
    }

    void OnToggleTimerChanged(bool isOn)
    {
        // Menampilkan atau menyembunyikan dropdown timer
        timerDropdown.gameObject.SetActive(isOn);
    }

    public void ValidateAndSaveSettings()
    {
        // Validasi input jumlah soal
        if (string.IsNullOrEmpty(jumlahSoalInput.text))
        {
            ShowPopup("Harap isi jumlah soal yang diinginkan!");
            return;
        }

        if (!int.TryParse(jumlahSoalInput.text, out int jumlahSoal) || jumlahSoal <= 0)
        {
            ShowPopup("Jumlah soal harus berupa angka positif!");
            return;
        }

        if (jumlahSoal > 100)
        {
            ShowPopup("Jumlah soal tidak boleh lebih dari 100!");
            return;
        }

        // Validasi timer jika timer diaktifkan
        if (useTimerToggle.isOn)
        {
            if (timerDropdown.options.Count == 0 || timerDropdown.value < 0)
            {
                ShowPopup("Harap pilih durasi timer yang valid!");
                return;
            }
        }

        // Jika semua validasi lolos, simpan pengaturan
        SaveSettingsAndReturn();
    }

    public async void SaveSettingsAndReturn()
    {
        // Mengambil nilai jumlah soal
        int jumlahSoal = 10;
        if (!int.TryParse(jumlahSoalInput.text, out jumlahSoal))
            jumlahSoal = 10;

        // Mengambil durasi timer jika timer diaktifkan
        int timerDurasi = 0;
        if (useTimerToggle.isOn && timerDropdown.options.Count > 0)
            int.TryParse(timerDropdown.options[timerDropdown.value].text, out timerDurasi);

        // Mencoba menyimpan pengaturan ke Unity Cloud Save
        try
        {
            var quizSettings = new QuizSettings
            {
                useTimer = useTimerToggle.isOn,
                timerDuration = timerDurasi,
                jumlahSoal = jumlahSoal,
                acakSoal = acakToggle.isOn
            };

            string settingsJson = JsonUtility.ToJson(quizSettings);

            // Menyimpan data ke Unity Cloud Save
            await CloudSaveService.Instance.Data.Player.SaveAsync(new Dictionary<string, object>
            {
                { "quiz_settings", settingsJson } // Pengaturan kuis
            });

            Debug.Log("Pengaturan kuis berhasil disimpan.");
            ShowPopup("Pengaturan berhasil disimpan!", true);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Gagal menyimpan pengaturan: " + ex.Message);
            ShowPopup("Gagal menyimpan pengaturan. Silakan coba lagi.");
        }
    }

    public void GoBack()
    {
        // Kembali ke scene sebelumnya atau scene utama
        // Anda bisa mengganti "MainMenu" dengan nama scene yang sesuai
        SceneManager.LoadScene("BankSoal");
    }

    private void ShowPopup(string message, bool loadSceneAfter = false)
    {
        if (popupPanel != null && popupText != null)
        {
            popupText.text = message;
            popupPanel.SetActive(true);
            
            // Jika perlu load scene setelah menutup popup
            if (loadSceneAfter)
            {
                // Tambahkan delay sebelum load scene
                StartCoroutine(LoadSceneAfterDelay(2f));
            }
        }
        else
        {
            Debug.LogWarning("Pop-up elements tidak ditemukan!");
            Debug.Log(message);
            
            // Jika popup tidak tersedia tapi perlu load scene
            if (loadSceneAfter)
            {
                SceneManager.LoadScene("BankSoal");
            }
        }
    }

    private void ClosePopup()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false);
    }

    private System.Collections.IEnumerator LoadSceneAfterDelay(float delay)
    {
        yield return new UnityEngine.WaitForSeconds(delay);
        SceneManager.LoadScene("BankSoal");
    }

    [System.Serializable]
    public class QuizSettings
    {
        public bool useTimer;
        public int timerDuration;
        public int jumlahSoal;
        public bool acakSoal;
    }
}