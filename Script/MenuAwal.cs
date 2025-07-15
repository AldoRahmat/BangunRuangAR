using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuAwal : MonoBehaviour
{
    [SerializeField] GameObject exitPanel;
    void Update ()
    {
        if (Input.GetKeyDown (KeyCode.Escape)){
            if (SceneManager.GetActiveScene ().buildIndex != 0){
                SceneManager.LoadScene (0);
            } else{
                if (exitPanel) {
                    exitPanel.SetActive (true);
                }
            }
        }
    }
    public void onUserClickYesNo(int choice){
        if (choice == 1){
            Application.Quit ();
        }
        exitPanel.SetActive (false);
    }
    public void Mulai(){
        SceneManager.LoadScene("Login");
    }

     public void Masuk()
    {
        SceneManager.LoadScene("MenuAwal");
    }

     public void Login()
    {
        SceneManager.LoadScene("Login");
    }
}
