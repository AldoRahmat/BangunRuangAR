using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
public class SoalManager : MonoBehaviour
{
    public GameObject soalItemPrefab;
    public Transform contentParent;
    public Button tambahButton, hapusButton, publishButton, nilaiButton;
    public GameObject editPanel;
    public SoalInputForm inputForm;

    [Header("Publish Settings")]
    public TMP_InputField namaGuruInput;
    public GameObject publishPanel;
    public Button confirmPublishButton, cancelPublishButton;
    public TMP_Text publishStatusText;

    
    private List<SoalData> soalList = new();
    private int selectedIndex = -1;

     async void Start()
    {
        if (!UnityServices.State.Equals(ServicesInitializationState.Initialized))
            await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

    
        SetupButtons();
        await LoadSoalList();
        RefreshUI();
    }

    void SetupButtons()
    {
        if (tambahButton != null)
            tambahButton.onClick.AddListener(OnTambahClicked);
        if (hapusButton != null)
            hapusButton.onClick.AddListener(OnHapusClicked);
        if (publishButton != null)
            publishButton.onClick.AddListener(OnPublishClicked);
        if (nilaiButton != null)
            nilaiButton.onClick.AddListener(OnNilaiClicked);
            
        // Publish buttons
        if (confirmPublishButton != null)
            confirmPublishButton.onClick.AddListener(OnConfirmPublish);
        if (cancelPublishButton != null)
            cancelPublishButton.onClick.AddListener(OnCancelPublish);
    }

    void OnTambahClicked()
    {
        if (inputForm == null) return;

        inputForm.ShowNew((newSoal) =>
        {
            soalList.Add(newSoal);
            SaveSoalList();
            RefreshUI();
        });
    }

    void OnHapusClicked()
    {
        if (selectedIndex >= 0 && selectedIndex < soalList.Count)
        {
            soalList.RemoveAt(selectedIndex);
            selectedIndex = -1;
            SaveSoalList();
            RefreshUI();
        }
    }

    void OnNilaiClicked()
    {
        SceneManager.LoadScene("NilaiScene");
    }

    // === PUBLISH SYSTEM ===
    void OnPublishClicked()
    {
        if (soalList.Count == 0)
        {
            ShowPublishStatus("Tidak ada soal untuk dipublikasikan!");
            return;
        }

        if (publishPanel != null)
            publishPanel.SetActive(true);
    }

    async void OnConfirmPublish()
    {
    string namaGuru = namaGuruInput.text.Trim();
    
    if (string.IsNullOrEmpty(namaGuru))
    {
        ShowPublishStatus("Nama Test harus diisi!");
        return;
    }

    try
    {
        ShowPublishStatus("Mempublikasikan soal untuk murid...");
        
        // Prepare data
        var teacherData = await PrepareTeacherData();
        
        if (teacherData.soal == null || teacherData.settings == null)
        {
            ShowPublishStatus("Data soal atau settings tidak lengkap!");
            return;
        }

        // Use PublicDataManager instead of account switching
        bool success = await PublicDataManager.Instance.SavePublicData(
            namaGuru, 
            teacherData.soal, 
            teacherData.settings
        );

        if (success)
        {
            ShowPublishStatus("Soal berhasil dipublikasikan untuk murid!");
            
            if (publishPanel != null)
                publishPanel.SetActive(false);
        }
        else
        {
            ShowPublishStatus("Gagal mempublikasikan soal. Silakan coba lagi.");
        }
    }
    catch (System.Exception e)
    {
        ShowPublishStatus($"Error publish: {e.Message}");
        Debug.LogError($"Publish failed: {e.Message}");
    }
}

    void OnCancelPublish()
    {
        if (publishPanel != null)
            publishPanel.SetActive(false);
    }

    async Task<TeacherBankData> PrepareTeacherData()
    {
        try
        {
            var teacherData = new TeacherBankData();

            // Prepare soal data
            var soalWrapper = new SoalListWrapper { list = soalList };
            teacherData.soal = JsonUtility.ToJson(soalWrapper);

            // Prepare quiz settings
            var quizSettings = await LoadQuizSettings();
            teacherData.settings = JsonUtility.ToJson(quizSettings);

            return teacherData;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to prepare teacher data: {e.Message}");
            throw;
        }
    }


    void ShowPublishStatus(string message)
    {
        if (publishStatusText != null)
            publishStatusText.text = message;
        
        Debug.Log($"Publish Status: {message}");
    }

    async Task<QuizSettings> LoadQuizSettings()
    {
        try
        {
            var result = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { "quiz_settings" });
            if (result.TryGetValue("quiz_settings", out var data))
            {
                return JsonUtility.FromJson<QuizSettings>(data.Value.GetAsString());
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Gagal load quiz settings: {e.Message}");
        }

        return new QuizSettings
        {
            timerDuration = 30f,
            useTimer = true,
            acakSoal = true,
            jumlahSoal = 10
        };
    }

    async Task LoadSoalList()
    {
        try
        {
            var result = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { "soal_list" });
            if (result.TryGetValue("soal_list", out var jsonData))
            {
                string json = jsonData.Value.GetAsString();
                var wrapper = JsonUtility.FromJson<SoalListWrapper>(json);
                soalList = wrapper.list ?? new List<SoalData>();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Gagal load soal: {e.Message}");
        }
    }

    async void SaveSoalList()
    {
        try
        {
            var wrapper = new SoalListWrapper { list = soalList };
            string json = JsonUtility.ToJson(wrapper);
            await CloudSaveService.Instance.Data.Player.SaveAsync(new Dictionary<string, object> { { "soal_list", json } });
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Gagal simpan soal: {e.Message}");
        }
    }

    void RefreshUI()
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        for (int i = 0; i < soalList.Count; i++)
        {
            int index = i;
            var item = Instantiate(soalItemPrefab, contentParent);

            TMP_Text textComponent = item.GetComponentInChildren<TMP_Text>();
            if (textComponent != null)
                textComponent.text = soalList[i].soal;

            Transform editButtonTransform = item.transform.Find("EditButton");
            if (editButtonTransform != null)
            {
                Button editBtn = editButtonTransform.GetComponent<Button>();
                if (editBtn != null)
                {
                    editBtn.onClick.AddListener(() =>
                    {
                        inputForm.ShowEdit(soalList[index], (updated) =>
                        {
                            soalList[index] = updated;
                            SaveSoalList();
                            RefreshUI();
                        });
                    });
                }
            }

            Button bgBtn = item.GetComponent<Button>();
            if (bgBtn != null)
            {
                bgBtn.onClick.AddListener(() =>
                {
                    selectedIndex = index;
                    RefreshUI();
                });
            }

            Image highlight = item.GetComponent<Image>();
            if (highlight != null)
                highlight.color = (index == selectedIndex) ? Color.yellow : Color.white;
        }
    }

    // === DATA CLASSES ===
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

    [System.Serializable]
    public class GuruListWrapper
    {
        public List<string> list;
    }

    [System.Serializable]
    public class QuizSettings
    {
        public float timerDuration;
        public bool useTimer;
        public bool acakSoal;
        public int jumlahSoal;
    }
    
    [System.Serializable]
    public class TeacherBankData
    {
        public string soal;
        public string settings;
    }
}