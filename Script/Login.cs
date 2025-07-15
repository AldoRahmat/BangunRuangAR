using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{
    public void Guru(){
        PlayerPrefs.SetString("UserRole", "Guru");
        PlayerPrefs.Save();
        SceneManager.LoadScene("InputPassword");
    }

    public void Murid(){
        PlayerPrefs.SetString("UserRole", "Murid");
        PlayerPrefs.Save();
        SceneManager.LoadScene("MainMenuMurid");
    }
}
