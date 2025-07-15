using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Rusuk : MonoBehaviour
{
    public void Geser()
    {
        SceneManager.LoadScene("RusukRumus");
    }
    public void Keluar()
    {
        SceneManager.LoadScene("MenuAwal");
    }
    public void Dimensi()
    {
        SceneManager.LoadScene("Dimensi");
    }
    public void Jaring()
    {
        SceneManager.LoadScene("Jaring");
    }  
}

