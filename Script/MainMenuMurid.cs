using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuMurid : MonoBehaviour
{
    public void Materi(){
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
        SceneManager.LoadScene("LoginMurid");
    }

    public void Keluar()
    {
        SceneManager.LoadScene("MenuAwal");
    }
    }
