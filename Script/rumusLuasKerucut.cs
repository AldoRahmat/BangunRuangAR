using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class rumusLuasKerucut : MonoBehaviour
{
    public TMP_InputField inputJariJari; // r
    public TMP_InputField inputTinggi;   // t
    public TextMeshProUGUI hasilText;
    public TextMeshProUGUI rumusText;

    public void HitungLuasPermukaanKerucut()
    {
        float r, t;

        bool sukses1 = float.TryParse(inputJariJari.text, out r);
        bool sukses2 = float.TryParse(inputTinggi.text, out t);

        if (sukses1 && sukses2)
        {
            // Hitung garis pelukis s = √(r² + t²)
            float s = Mathf.Sqrt(r * r + t * t);

            // Hitung luas permukaan
            float luasPermukaan = Mathf.PI * r * (r + s);

            hasilText.text = luasPermukaan.ToString("F2") + " cm²";

            // Tampilkan detail rumus
            rumusText.text = 
                $"π × r × (r + s)<br>" +
                $"3.14 × {r} × ({r} + {s:F2})";
        }
        else
        {
            hasilText.text = "Input tidak valid!";
            rumusText.text = "";
        }
    }
}