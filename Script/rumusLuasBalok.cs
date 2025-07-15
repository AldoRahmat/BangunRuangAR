using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class rumusLuasBalok : MonoBehaviour
{
    public TMP_InputField inputPanjang; // Panjang
    public TMP_InputField inputLebar;   // Lebar
    public TMP_InputField inputTinggi;  // Tinggi
    public TextMeshProUGUI hasilText;
    public TextMeshProUGUI rumusText;

    public void HitungLuasPermukaanBalok()
    {
        float panjang, lebar, tinggi;

        bool sukses1 = float.TryParse(inputPanjang.text, out panjang);
        bool sukses2 = float.TryParse(inputLebar.text, out lebar);
        bool sukses3 = float.TryParse(inputTinggi.text, out tinggi);

        if (sukses1 && sukses2 && sukses3)
        {
            float luas = 2 * (panjang * lebar + panjang * tinggi + lebar * tinggi);
            hasilText.text = luas.ToString("F2") + $" cm²";

            // Tampilkan rumus lengkap
            rumusText.text = $"2 × ((p × l) + (p × t) + (l × t))<br>2 × (({panjang}×{lebar}) + ({panjang}×{tinggi}) + ({lebar}×{tinggi}))";
        }
        else
        {
            hasilText.text = "Input tidak valid!";
            rumusText.text = "";
        }
    }
}
