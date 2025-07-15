using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SoalInputForm : MonoBehaviour
{
    public TMP_InputField soalInput, inputA, inputB, inputC, inputD, jawabanInput;
    public Button saveButton;
    public Button backButton; // Button untuk kembali
    public GameObject notificationPanel; // Panel untuk menampilkan notifikasi
    public TextMeshProUGUI notificationText; // Text untuk pesan notifikasi
    public Button closeNotificationButton; // Button untuk menutup notifikasi
    private System.Action<SoalManager.SoalData> onSaveCallback;

    public void ShowNew(System.Action<SoalManager.SoalData> onSave)
    {
        ClearFields();
        gameObject.SetActive(true);
        onSaveCallback = onSave;
    }

    public void ShowEdit(SoalManager.SoalData data, System.Action<SoalManager.SoalData> onSave)
    {
        soalInput.text = data.soal;
        inputA.text = data.pilihanA;
        inputB.text = data.pilihanB;
        inputC.text = data.pilihanC;
        inputD.text = data.pilihanD;
        jawabanInput.text = data.jawaban;
        gameObject.SetActive(true);
        onSaveCallback = onSave;
    }

    void Start()
    {
         if (saveButton != null)
        {
            saveButton.onClick.AddListener(() =>
            {
                if (ValidateFields())
                {
                    var data = new SoalManager.SoalData
                    {
                        soal = soalInput.text,
                        pilihanA = inputA.text,
                        pilihanB = inputB.text,
                        pilihanC = inputC.text,
                        pilihanD = inputD.text,
                        jawaban = jawabanInput.text.ToUpper()
                    };

                    onSaveCallback?.Invoke(data);
                    gameObject.SetActive(false);
                }
            });
        }

        // Setup close notification button
        if (closeNotificationButton != null)
        {
            closeNotificationButton.onClick.AddListener(CloseNotification);
        }

        // Setup back button
        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackPressed);
        }
    }

    bool ValidateFields()
    {
        string missingFields = "";
        
        if (string.IsNullOrWhiteSpace(soalInput.text))
            missingFields += "- Soal\n";
            
        if (string.IsNullOrWhiteSpace(inputA.text))
            missingFields += "- Pilihan A\n";
            
        if (string.IsNullOrWhiteSpace(inputB.text))
            missingFields += "- Pilihan B\n";
            
        if (string.IsNullOrWhiteSpace(inputC.text))
            missingFields += "- Pilihan C\n";
            
        if (string.IsNullOrWhiteSpace(inputD.text))
            missingFields += "- Pilihan D\n";
            
        if (string.IsNullOrWhiteSpace(jawabanInput.text))
            missingFields += "- Jawaban\n";
        else
        {
            // Validasi jawaban harus A, B, C, atau D
            string jawaban = jawabanInput.text.ToUpper().Trim();
            if (jawaban != "A" && jawaban != "B" && jawaban != "C" && jawaban != "D")
            {
                ShowNotification("Jawaban harus berupa A, B, C, atau D!");
                return false;
            }
        }

        if (!string.IsNullOrEmpty(missingFields))
        {
            ShowNotification("Mohon lengkapi kolom berikut:\n\n" + missingFields);
            return false;
        }

        return true;
    }

    void ShowNotification(string message)
    {
        if (notificationPanel != null && notificationText != null)
        {
            notificationText.text = message;
            notificationPanel.SetActive(true);
        }
        else
        {
            // Fallback menggunakan Debug.LogWarning jika UI notification tidak tersedia
            Debug.LogWarning(message);
        }
    }

    void CloseNotification()
    {
        if (notificationPanel != null)
        {
            notificationPanel.SetActive(false);
        }
    }

    void OnBackPressed()
    {
        // Tutup form tanpa menyimpan
        gameObject.SetActive(false);
    }

    void ClearFields()
    {
        soalInput.text = inputA.text = inputB.text = inputC.text = inputD.text = jawabanInput.text = "";
    }
}