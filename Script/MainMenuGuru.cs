using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Services.Authentication;

public class MainMenuGuru : MonoBehaviour
{
    public void Keluar(){
        // Sign out dari Unity Authentication Service
        if (AuthenticationService.Instance.IsSignedIn)
        {
            AuthenticationService.Instance.SignOut();
            Debug.Log("User berhasil sign out");
        }
        else
        {
            Debug.Log("User sudah tidak login");
        }
        
        // Kembali ke menu awal
        SceneManager.LoadScene("MenuAwal");
    }

    public void Materi()
    {
        SceneManager.LoadScene("MateriNew");
    }

    public void Dimensi()
    {
        SceneManager.LoadScene("Dimensi");
    }

    public void Kalkulator()
    {
        SceneManager.LoadScene("KalkulatorKubus");
    }

    public void Kuis()
    {
        SceneManager.LoadScene("BankSoal");
    }
}
