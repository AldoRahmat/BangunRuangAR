using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
public class StudentLogin : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField namaInput;
    public TMP_InputField noAbsenInput;
    public TMP_Dropdown guruDropdown;
    public Button mulaiKuisButton;
    public Button refreshButton;

    [Header("Loading")]
    public GameObject loadingPanel;
    public TMP_Text statusText;


    private List<string> guruList = new List<string>();
    private string selectedGuru = "";

    async void Start()
    {
        if (!UnityServices.State.Equals(ServicesInitializationState.Initialized))
            await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();


        SetupUI();
        await LoadGuruList();
    }

    void SetupUI()
    {
        if (mulaiKuisButton != null)
            mulaiKuisButton.onClick.AddListener(OnMulaiKuisClicked);
        
        if (refreshButton != null)
            refreshButton.onClick.AddListener(OnRefreshClicked);

        if (guruDropdown != null)
            guruDropdown.onValueChanged.AddListener(OnGuruSelected);
    }

    async void OnRefreshClicked()
    {
        await LoadGuruList();
    }

    void OnGuruSelected(int index)
    {
        if (index > 0 && index <= guruList.Count)
        {
            selectedGuru = guruList[index - 1];
        }
        else
        {
            selectedGuru = "";
        }
    }

    async void OnMulaiKuisClicked()
    {
        if (!ValidateInput())
            return;

        ShowLoading("Memuat data kuis...");

        try
        {
            // Simpan data siswa untuk digunakan di quiz
            var studentData = new StudentData
            {
                nama = namaInput.text.Trim(),
                noAbsen = noAbsenInput.text.Trim(),
                guru = selectedGuru
            };

            PlayerPrefs.SetString("student_data", JsonUtility.ToJson(studentData));

            // Load data kuis dari akun publik
            bool dataLoaded = await LoadQuizDataFromPublicAccount(selectedGuru);
            
            if (dataLoaded)
            {
                SceneManager.LoadScene("MulaiKuis"); // Ganti dengan nama scene quiz
            }
            else
            {
                ShowStatus("Gagal memuat data kuis. Silakan coba lagi.");
            }
        }
        catch (System.Exception e)
        {
            ShowStatus($"Error: {e.Message}");
        }
        finally
        {
            HideLoading();
        }
    }

    bool ValidateInput()
    {
        if (string.IsNullOrEmpty(namaInput.text.Trim()))
        {
            ShowStatus("Nama harus diisi!");
            return false;
        }

        if (string.IsNullOrEmpty(noAbsenInput.text.Trim()))
        {
            ShowStatus("No. Absen harus diisi!");
            return false;
        }

        if (string.IsNullOrEmpty(selectedGuru))
        {
            ShowStatus("Pilih tes terlebih dahulu!");
            return false;
        }

        return true;
    }

async Task<bool> LoadQuizDataFromPublicAccount(string namaGuru)
{
    try
    {
        // Use PublicDataManager instead of account switching
        bool success = await PublicDataManager.Instance.LoadPublicData(namaGuru, 
            (soalData, settingsData) => 
            {
                PlayerPrefs.SetString("quiz_soal", soalData);
                PlayerPrefs.SetString("quiz_settings", settingsData);
            });

        return success;
    }
    catch (System.Exception e)
    {
        Debug.LogError($"Gagal load data dari public account: {e.Message}");
        return false;
    }
}

    async Task LoadGuruList()
    {
    ShowLoading("Memuat daftar tes...");

    try
    {
        // Use PublicDataManager instead of account switching
        guruList = await PublicDataManager.Instance.GetGuruList();
        UpdateGuruDropdown();
    }
    catch (System.Exception e)
    {
        Debug.LogError($"Gagal load guru list: {e.Message}");
        ShowStatus("Gagal memuat daftar tes");
    }
    finally
    {
        HideLoading();
    }
}


    void UpdateGuruDropdown()
    {
        if (guruDropdown == null) return;

        guruDropdown.ClearOptions();
        
        List<string> options = new List<string> { "-- Pilih Tes --" };
        options.AddRange(guruList);
        
        guruDropdown.AddOptions(options);
        guruDropdown.value = 0;
        selectedGuru = "";

        // Show message if no teachers available
        if (guruList.Count == 0)
        {
            ShowStatus("Belum ada guru yang mempublikasikan soal");
        }
        else
        {
            ShowStatus($"Ditemukan {guruList.Count} guru dengan soal tersedia");
        }
    }

    void ShowLoading(string message)
    {
        if (loadingPanel != null)
            loadingPanel.SetActive(true);
        
        ShowStatus(message);
    }

    void HideLoading()
    {
        if (loadingPanel != null)
            loadingPanel.SetActive(false);
    }

    void ShowStatus(string message)
    {
        if (statusText != null)
            statusText.text = message;
        
        Debug.Log(message);
    }

    [System.Serializable]
    public class StudentData
    {
        public string nama;
        public string noAbsen;
        public string guru;
    }

    [System.Serializable]
    public class GuruListWrapper
    {
        public List<string> list;
    }
}