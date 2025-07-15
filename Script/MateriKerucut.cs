using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MateriKerucut : MonoBehaviour
{
    public void Kubus()
    {
        SceneManager.LoadScene ("MateriKubus");
    }

    public void Balok()
    {
        SceneManager.LoadScene ("MateriBalok");
    }
    public void Limas()
    {
        SceneManager.LoadScene ("MateriLimas");
    }

    public void Bola()
    {
        SceneManager.LoadScene ("MateriBola");
    }
    public void Prisma()
    {
        SceneManager.LoadScene ("MateriPrisma");
    }

    public void Kerucut()
    {
        SceneManager.LoadScene ("MateriKerucut");
    }
    public void Tabung()
    {
        SceneManager.LoadScene ("MateriTabung");
    }

    public void Keluar()
    {
        SceneManager.LoadScene ("MenuAwal");
    }
    public void Suara()
    {
        SceneManager.LoadScene ("MenuAwal");
    }
}
