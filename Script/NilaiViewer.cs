using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine.SceneManagement;

public class NilaiViewer : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Dropdown guruDropdown;
    public GameObject resultItemPrefab;
    public Transform contentParent;
    public Button refreshButton;
    public Button kembaliButton;
    public Button exportButton;

    [Header("Result Details")]
    public GameObject detailPanel;
    public TMP_Text detailText;
    public Button closeDetailButton;

    [Header("Loading")]
    public GameObject loadingPanel;
    public TMP_Text statusText;

    [Header("Statistics")]
    public TMP_Text statsText;

    private List<string> guruList = new List<string>();
    private List<QuizResult> currentResults = new List<QuizResult>();
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
        if (refreshButton != null)
            refreshButton.onClick.AddListener(OnRefreshClicked);
        
        if (kembaliButton != null)
            kembaliButton.onClick.AddListener(OnKembaliClicked);
        
        if (exportButton != null)
            exportButton.onClick.AddListener(OnExportClicked);
        
        if (closeDetailButton != null)
            closeDetailButton.onClick.AddListener(OnCloseDetailClicked);

        if (guruDropdown != null)
            guruDropdown.onValueChanged.AddListener(OnGuruSelected);
    }

    async void OnRefreshClicked()
    {
        if (!string.IsNullOrEmpty(selectedGuru))
        {
            await LoadResultsForGuru(selectedGuru);
        }
    }

    void OnKembaliClicked()
    {
        SceneManager.LoadScene("BankSoal"); // Ganti dengan nama scene bank soal
    }

    void OnExportClicked()
    {
        if (currentResults.Count > 0)
        {
            ExportResultsToCSV();
        }
        else
        {
            ShowStatus("Tidak ada data untuk diekspor");
        }
    }

    void OnCloseDetailClicked()
    {
        if (detailPanel != null)
            detailPanel.SetActive(false);
    }

    void OnGuruSelected(int index)
    {
        if (index > 0 && index <= guruList.Count)
        {
            selectedGuru = guruList[index - 1];
            LoadResultsForGuru(selectedGuru);
        }
        else
        {
            selectedGuru = "";
            ClearResults();
        }
    }

    async Task LoadGuruList()
    {
        ShowLoading("Memuat daftar tes...");

        try
        {
            // Use PublicDataManager to get guru list - consistent with other files
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
    }

    async Task LoadResultsForGuru(string namaGuru)
    {
        ShowLoading($"Memuat hasil untuk guru {namaGuru}...");

        try
        {
            // Load all keys from cloud save
            var allData = await CloudSaveService.Instance.Data.Player.LoadAllAsync();
            
            currentResults.Clear();

            foreach (var kvp in allData)
            {
                if (kvp.Key.StartsWith($"result_{namaGuru}_"))
                {
                    try
                    {
                        string json = kvp.Value.Value.GetAsString();
                        var result = JsonUtility.FromJson<QuizResult>(json);
                        currentResults.Add(result);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"Error parsing result: {e.Message}");
                    }
                }
            }

            // Sort by timestamp (newest first)
            currentResults = currentResults.OrderByDescending(r => r.timestamp).ToList();

            UpdateResultsUI();
            UpdateStatistics();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Gagal load results: {e.Message}");
            ShowStatus($"Gagal memuat hasil untuk tes {namaGuru}");
        }
        finally
        {
            HideLoading();
        }
    }

    void UpdateResultsUI()
    {
        // Clear existing items
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // Create result items
        foreach (var result in currentResults)
        {
            var item = Instantiate(resultItemPrefab, contentParent);
            
            // Set basic info
            var nameText = item.transform.Find("NameText")?.GetComponent<TMP_Text>();
            if (nameText != null)
                nameText.text = result.studentName;

            var absenText = item.transform.Find("AbsenText")?.GetComponent<TMP_Text>();
            if (absenText != null)
                absenText.text = $"No. {result.noAbsen}";

            var scoreText = item.transform.Find("ScoreText")?.GetComponent<TMP_Text>();
            if (scoreText != null)
                scoreText.text = $"{result.score}/{result.totalQuestions} ({result.percentage:F1}%)";

            var timeText = item.transform.Find("TimeText")?.GetComponent<TMP_Text>();
            if (timeText != null)
                timeText.text = result.timestamp;

            // Set score color
            if (scoreText != null)
            {
                if (result.percentage >= 80)
                    scoreText.color = Color.green;
                else if (result.percentage >= 60)
                    scoreText.color = Color.yellow;
                else
                    scoreText.color = Color.red;
            }

            // Detail button
            var detailButton = item.transform.Find("DetailButton")?.GetComponent<Button>();
            if (detailButton != null)
            {
                detailButton.onClick.AddListener(() => ShowResultDetail(result));
            }
        }
    }

    void ShowResultDetail(QuizResult result)
    {
        if (detailPanel == null || detailText == null) return;

        string detailInfo = $"Detail Hasil Kuis\n\n";
        detailInfo += $"Nama: {result.studentName}\n";
        detailInfo += $"No. Absen: {result.noAbsen}\n";
        detailInfo += $"Tes: {result.guru}\n";
        detailInfo += $"Waktu: {result.timestamp}\n";
        detailInfo += $"Skor: {result.score}/{result.totalQuestions} ({result.percentage:F1}%)\n\n";

        detailInfo += "Jawaban:\n";
        for (int i = 0; i < result.answers.Count && i < result.answerResults.Count; i++)
        {
            string answer = string.IsNullOrEmpty(result.answers[i]) ? "Tidak dijawab" : result.answers[i];
            string status = result.answerResults[i] ? "✓" : "✗";
            detailInfo += $"{i + 1}. {answer} {status}\n";
        }

        detailText.text = detailInfo;
        detailPanel.SetActive(true);
    }

    void UpdateStatistics()
    {
        if (statsText == null || currentResults.Count == 0) return;

        int totalStudents = currentResults.Count;
        float avgScore = currentResults.Average(r => r.percentage);
        int passedStudents = currentResults.Count(r => r.percentage >= 60); // Assuming 60% is passing
        float passRate = (float)passedStudents / totalStudents * 100f;

        var topScore = currentResults.OrderByDescending(r => r.percentage).FirstOrDefault();
        var lowScore = currentResults.OrderBy(r => r.percentage).FirstOrDefault();

        string stats = $"Statistik Kuis - {selectedGuru}\n\n";
        stats += $"Total Siswa: {totalStudents}\n";
        stats += $"Rata-rata Nilai: {avgScore:F1}%\n";
        stats += $"Siswa Lulus: {passedStudents}/{totalStudents} ({passRate:F1}%)\n\n";
        
        if (topScore != null)
            stats += $"Nilai Tertinggi: {topScore.percentage:F1}% ({topScore.studentName})\n";
        
        if (lowScore != null)
            stats += $"Nilai Terendah: {lowScore.percentage:F1}% ({lowScore.studentName})";

        statsText.text = stats;
    }

    void ExportResultsToCSV()
    {
        if (currentResults.Count == 0) return;

        string csv = "Nama,No_Absen,Guru,Skor,Total_Soal,Persentase,Waktu\n";
        
        foreach (var result in currentResults)
        {
            csv += $"{result.studentName},{result.noAbsen},{result.guru},{result.score},{result.totalQuestions},{result.percentage:F1},{result.timestamp}\n";
        }

        // Save to file (you might want to use a file dialog or specific path)
        string fileName = $"hasil_kuis_{selectedGuru}_{System.DateTime.Now:yyyyMMdd_HHmmss}.csv";
        string path = System.IO.Path.Combine(Application.persistentDataPath, fileName);
        
        try
        {
            System.IO.File.WriteAllText(path, csv);
            ShowStatus($"File berhasil diekspor ke: {path}");
        }
        catch (System.Exception e)
        {
            ShowStatus($"Gagal ekspor file: {e.Message}");
        }
    }

    void ClearResults()
    {
        currentResults.Clear();
        UpdateResultsUI();
        
        if (statsText != null)
            statsText.text = "";
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
    public class QuizResult
    {
        public string studentName;
        public string noAbsen;
        public string guru;
        public int score;
        public int totalQuestions;
        public float percentage;
        public List<string> answers;
        public List<bool> answerResults;
        public string timestamp;
    }

    [System.Serializable]
    public class GuruListWrapper
    {
        public List<string> list;
    }
}