using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RusukRumus : MonoBehaviour
{
   public void Geser()
    {
        SceneManager.LoadScene("Rusuk");
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
