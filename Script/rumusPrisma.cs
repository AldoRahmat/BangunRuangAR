using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class rumusPrismaSegitiga : MonoBehaviour
{
    public TMP_InputField inputAlas;           // Alas segitiga
    public TMP_InputField inputTinggiSegitiga; // Tinggi segitiga
    public TMP_InputField inputTinggiPrisma;   // Tinggi prisma
    public TextMeshProUGUI hasilText;
    public TextMeshProUGUI rumusText;

    public void HitungVolumePrisma()
    {
        float alas, tinggiSegitiga, tinggiPrisma;

        bool sukses1 = float.TryParse(inputAlas.text, out alas);
        bool sukses2 = float.TryParse(inputTinggiSegitiga.text, out tinggiSegitiga);
        bool sukses3 = float.TryParse(inputTinggiPrisma.text, out tinggiPrisma);

        if (sukses1 && sukses2 && sukses3)
        {
            float luasAlas = 0.5f * alas * tinggiSegitiga;
            float volume = luasAlas * tinggiPrisma;

            hasilText.text = volume.ToString("F2") + $" cm³";

            // Tampilkan proses perhitungan
            rumusText.text = $"(½ × a\u25B2 × t\u25B2) × t<br>(½ × {alas} × {tinggiSegitiga}) × {tinggiPrisma}";
        }
        else
        {
            hasilText.text = "Input tidak valid!";
            rumusText.text = "";
        }
    }
}
