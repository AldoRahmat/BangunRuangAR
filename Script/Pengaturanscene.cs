using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Threading.Tasks;
using UnityEngine.UI;
using TMPro;

public class Pengaturanscene : MonoBehaviour
{
     [Header("Pop-up Components")]
    public GameObject popupPanel; // Panel untuk pop-up
    public TMP_Text popupText; // Text untuk pesan pop-up
    public Button popupOKButton; // Tombol OK pada pop-up

    void Start()
    {
        // Setup pop-up button listener
        if (popupOKButton != null)
            popupOKButton.onClick.AddListener(ClosePopup);
        
        // Hide pop-up at start
        if (popupPanel != null)
            popupPanel.SetActive(false);
    }

    public void Setting()
    {
        SceneManager.LoadScene("PengaturanKuis");
    }

    public async void MulaiKuis()
    {
        // Cek apakah ada soal sebelum pindah scene
        bool hasSoal = await CheckIfSoalExists();
        
        if (hasSoal)
        {
            SceneManager.LoadScene("MulaiKuis");
        }
        else
        {
            ShowPopup("Tambahkan Minimal 1 Soal");
        }
    }

    public void BalikKuis()
    {
        SceneManager.LoadScene("MainMenuMurid");
    }

    private async Task<bool> CheckIfSoalExists()
    {
        try
        {
            // Initialize Unity Services jika belum
            if (!UnityServices.State.Equals(ServicesInitializationState.Initialized))
                await UnityServices.InitializeAsync();

            // Sign in jika belum
            if (!AuthenticationService.Instance.IsSignedIn)
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

            // Load data soal dari cloud save
            var result = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { "soal_list" });
            
            if (result.TryGetValue("soal_list", out var jsonData))
            {
                string json = jsonData;
                var wrapper = JsonUtility.FromJson<SoalListWrapper>(json);
                
                // Cek apakah list tidak null dan memiliki minimal 1 soal
                return wrapper.list != null && wrapper.list.Count > 0;
            }
            
            return false; // Tidak ada data soal
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error checking soal: {e.Message}");
            return false; // Anggap tidak ada soal jika terjadi error
        }
    }

    private void ShowPopup(string message)
    {
        if (popupPanel != null && popupText != null)
        {
            popupText.text = message;
            popupPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("Pop-up components not assigned!");
            // Fallback menggunakan Debug.Log jika komponen pop-up tidak di-assign
            Debug.LogWarning(message);
        }
    }

    private void ClosePopup()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false);
    }

    // Class helper untuk deserialize data soal
    [System.Serializable]
    public class SoalData
    {
        public string soal;
        public string pilihanA, pilihanB, pilihanC, pilihanD;
        public string jawaban;
    }

    [System.Serializable]
    public class SoalListWrapper
    {
        public List<SoalData> list;
    }
}