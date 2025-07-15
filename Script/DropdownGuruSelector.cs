using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using Unity.Services.Authentication;

public class DropdownGuruSelector : MonoBehaviour
{
    public TMP_Dropdown guruDropdown;
    public Button startQuizButton;
    public TMP_InputField namaInput, absenInput;

    private List<string> guruIds = new();

    [System.Serializable]
    public class GuruListWrapper
    {
        public List<string> guruIds;
    }

    async void Start()
    {
        await UnityServices.InitializeAsync();

        // Cek apakah sudah login, jika belum login sebagai BankSoal
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            try
            {
                await AuthenticationService.Instance.SignInWithUsernamePasswordAsync("BankSoal", "BankSoal#1234");
                Debug.Log("Login BankSoal berhasil.");
            }
            catch (System.Exception e)
            {
                Debug.LogError("Gagal login sebagai BankSoal: " + e.Message);
                return;
            }
        }
        else
        {
            Debug.Log("Sudah terlogin sebagai BankSoal.");
        }

        // Ambil guru_list dari Cloud Save
        try
        {
            var keys = new HashSet<string> { "guru_list" };
            var result = await CloudSaveService.Instance.Data.Player.LoadAsync(keys);

            if (result.TryGetValue("guru_list", out var item))
            {
                string json = item.Value.GetAs<string>();
                guruIds = JsonUtility.FromJson<GuruListWrapper>(json).guruIds;

                guruDropdown.ClearOptions();
                guruDropdown.AddOptions(guruIds);
            }
            else
            {
                Debug.LogWarning("Data guru_list tidak ditemukan di Cloud Save.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Gagal memuat guru_list: " + e.Message);
        }

        // Saat tombol Start ditekan
        startQuizButton.onClick.AddListener(() =>
        {
            // Validasi input
            if (string.IsNullOrEmpty(namaInput.text) || string.IsNullOrEmpty(absenInput.text))
            {
                Debug.LogWarning("Nama dan Absen tidak boleh kosong.");
                return;
            }

            if (guruDropdown.value < 0 || guruDropdown.value >= guruIds.Count)
            {
                Debug.LogWarning("Pilih guru dari dropdown.");
                return;
            }

            PlayerPrefs.SetString("SelectedGuruId", guruDropdown.options[guruDropdown.value].text);
            PlayerPrefs.SetString("NamaSiswa", namaInput.text);
            PlayerPrefs.SetString("AbsenSiswa", absenInput.text);

            UnityEngine.SceneManagement.SceneManager.LoadScene("MulaiKuis");
        });
    }
}