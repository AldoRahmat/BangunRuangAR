using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using Newtonsoft.Json;
using System.Collections;
using System;

public class QuizRunner : MonoBehaviour
{
     [Header("UI References")]
    public TextMeshProUGUI soalText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI studentInfoText;
    public GameObject panelSoal;
    public GameObject panelHasil;
    public TextMeshProUGUI hasilText;
    public GameObject popupBenar;
    public GameObject popupSalah;

    public Button tombolA;
    public Button tombolB;
    public Button tombolC;
    public Button tombolD;
    public Button kembaliButton;

    [Header("Popup Duration")]
    public float popupDuration = 1.2f;

    private List<SoalData> soalList = new List<SoalData>();
    private QuizSettings quizSettings;
    private StudentData studentData;
    private int currentIndex = 0;
    private int skor = 0;
    private float timer;
    private bool isTimerRunning = false;
    private List<string> studentAnswers = new List<string>();
    private List<bool> answerResults = new List<bool>();

    async void Start()
    {
        await UnityServices.InitializeAsync();
        LoadDataFromPlayerPrefs();
        
        if (quizSettings != null && soalList != null && soalList.Count > 0 && studentData != null)
        {
            SetupQuiz();
            ShowSoal();
        }
        else
        {
            Debug.LogError("Data kuis tidak lengkap!");
            hasilText.text = "Error: Data kuis tidak lengkap!";
            panelHasil.SetActive(true);
        }
    }

    void LoadDataFromPlayerPrefs()
    {
        try
        {
            // Load student data
            string studentJson = PlayerPrefs.GetString("student_data", "");
            if (!string.IsNullOrEmpty(studentJson))
            {
                studentData = JsonUtility.FromJson<StudentData>(studentJson);
                Debug.Log($"Student data loaded: {studentData.nama}");
            }

            // Load quiz settings
            string settingsJson = PlayerPrefs.GetString("quiz_settings", "");
            if (!string.IsNullOrEmpty(settingsJson))
            {
                quizSettings = JsonUtility.FromJson<QuizSettings>(settingsJson);
                Debug.Log("Quiz settings loaded");
            }

            // Load soal list
            string soalJson = PlayerPrefs.GetString("quiz_soal", "");
            if (!string.IsNullOrEmpty(soalJson))
            {
                SoalListWrapper wrapper = JsonUtility.FromJson<SoalListWrapper>(soalJson);
                soalList = wrapper.list ?? new List<SoalData>();
                Debug.Log($"Soal list loaded, count: {soalList.Count}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error loading data: {e.Message}");
        }
    }

    void SetupQuiz()
    {
        // Setup student info display
        if (studentInfoText != null && studentData != null)
        {
            studentInfoText.text = $"Nama: {studentData.nama} | No. Absen: {studentData.noAbsen} | Guru: {studentData.guru}";
        }

        // Setup quiz
        if (quizSettings.acakSoal)
            soalList.Shuffle();

        if (quizSettings.jumlahSoal < soalList.Count)
            soalList = soalList.GetRange(0, quizSettings.jumlahSoal);

        timer = quizSettings.timerDuration;
        isTimerRunning = quizSettings.useTimer;

        // Initialize answer tracking
        studentAnswers = new List<string>();
        answerResults = new List<bool>();

        // Setup buttons
        tombolA.onClick.AddListener(() => Jawab("A"));
        tombolB.onClick.AddListener(() => Jawab("B"));
        tombolC.onClick.AddListener(() => Jawab("C"));
        tombolD.onClick.AddListener(() => Jawab("D"));
        
        if (kembaliButton != null)
            kembaliButton.onClick.AddListener(KembaliKeLogin);
    }

    void Update()
    {
        if (isTimerRunning)
        {
            timer -= Time.deltaTime;
            timerText.text = "Waktu: " + Mathf.Ceil(timer).ToString();

            if (timer <= 0)
            {
                timer = quizSettings.timerDuration;
                
                // Auto answer dengan jawaban kosong jika waktu habis
                studentAnswers.Add("");
                answerResults.Add(false);

                currentIndex++;

                if (currentIndex >= soalList.Count)
                {
                    TampilkanHasil();
                }
                else
                {
                    ShowSoal();
                }
            }
        }
    }

    void ShowSoal()
    {
        if (currentIndex >= soalList.Count)
        {
            TampilkanHasil();
            return;
        }

        SoalData s = soalList[currentIndex];

        soalText.text = $"Soal {currentIndex + 1}: {s.soal}";
        tombolA.GetComponentInChildren<TextMeshProUGUI>().text = "A. " + s.pilihanA;
        tombolB.GetComponentInChildren<TextMeshProUGUI>().text = "B. " + s.pilihanB;
        tombolC.GetComponentInChildren<TextMeshProUGUI>().text = "C. " + s.pilihanC;
        tombolD.GetComponentInChildren<TextMeshProUGUI>().text = "D. " + s.pilihanD;

        timer = quizSettings.timerDuration;
    }

    void Jawab(string pilihan)
    {
        string benar = soalList[currentIndex].jawaban;
        bool isCorrect = pilihan == benar;

        // Record answer
        studentAnswers.Add(pilihan);
        answerResults.Add(isCorrect);

        if (isCorrect)
        {
            skor++;
            StartCoroutine(TampilkanPopupDanLanjut(popupBenar));
        }
        else
        {
            StartCoroutine(TampilkanPopupDanLanjut(popupSalah));
        }
    }

    IEnumerator TampilkanPopupDanLanjut(GameObject popup)
    {
        popup.SetActive(true);
        yield return new WaitForSeconds(popupDuration);
        popup.SetActive(false);

        currentIndex++;
        
        if (currentIndex >= soalList.Count)
        {
            TampilkanHasil();
        }
        else
        {
            ShowSoal();
        }
    }

    async void TampilkanHasil()
    {
        panelSoal.SetActive(false);
        panelHasil.SetActive(true);
        
        float percentage = (float)skor / soalList.Count * 100f;
        hasilText.text = $"Hasil Kuis\n\nNama: {studentData.nama}\nNo. Absen: {studentData.noAbsen}\nKuis : {studentData.guru}\n\nSkor: {skor} dari {soalList.Count}\nPersentase: {percentage:F1}%";

        // Save result to cloud
        await SaveResultToCloud();
    }

    async Task SaveResultToCloud()
    {
        try
        {
            var result = new QuizResult
            {
                studentName = studentData.nama,
                noAbsen = studentData.noAbsen,
                guru = studentData.guru,
                score = skor,
                totalQuestions = soalList.Count,
                percentage = (float)skor / soalList.Count * 100f,
                answers = studentAnswers,
                answerResults = answerResults,
                timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            string resultJson = JsonUtility.ToJson(result);
            string resultKey = $"result_{studentData.guru}_{studentData.noAbsen}_{System.DateTime.Now.Ticks}";

            await CloudSaveService.Instance.Data.Player.SaveAsync(new Dictionary<string, object> 
            { 
                { resultKey, resultJson } 
            });

            Debug.Log("Result saved to cloud");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save result: {e.Message}");
        }
    }

    void KembaliKeLogin()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenuMurid"); // Ganti dengan nama scene login
    }

    [System.Serializable]
    public class StudentData
    {
        public string nama;
        public string noAbsen;
        public string guru;
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
}
