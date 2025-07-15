using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class rumusLuasPrisma : MonoBehaviour
{
    public TMP_InputField inputAlas;           // Alas segitiga
    public TMP_InputField inputTinggiSegitiga; // Tinggi segitiga
    public TMP_InputField inputTinggiPrisma;   // Tinggi prisma
    public TextMeshProUGUI hasilText;
    public TextMeshProUGUI rumusText;

    public void HitungLuasPermukaanPrisma()
    {
        float alas, tinggiSegitiga, tinggiPrisma;

        bool sukses1 = float.TryParse(inputAlas.text, out alas);
        bool sukses2 = float.TryParse(inputTinggiSegitiga.text, out tinggiSegitiga);
        bool sukses3 = float.TryParse(inputTinggiPrisma.text, out tinggiPrisma);

        if (sukses1 && sukses2 && sukses3)
        {
            // Hitung sisi miring segitiga
            float sisiMiring = Mathf.Sqrt(Mathf.Pow(alas / 2f, 2f) + Mathf.Pow(tinggiSegitiga, 2f));

            // Keliling alas segitiga
            float kelilingAlas = alas + 2 * sisiMiring;

            // Luas alas segitiga
            float luasAlas = 0.5f * alas * tinggiSegitiga;

            // Luas permukaan prisma
            float luasPermukaan = 2 * luasAlas + kelilingAlas * tinggiPrisma;

            // Tampilkan hasil
            hasilText.text = luasPermukaan.ToString("F2")  + $" cm²";

            // Format untuk rumusText
            string sisiMiringStr = sisiMiring.ToString("F2");
            string kelilingStr = kelilingAlas.ToString("F2");
            string luasAlasStr = luasAlas.ToString("F2");

            rumusText.text = 
                $"2 × (½ × a\u25B2 × t\u25B2) + (k\u25B2 × t)\n" +
                $"2 × (½ × {alas} × {tinggiSegitiga}) + ({alas} + (2 × {sisiMiringStr}) × {tinggiPrisma})\n" +
                $"2 × ({luasAlasStr}) + ({kelilingStr} × {tinggiPrisma})";
        }
        else
        {
            hasilText.text = "Input tidak valid!";
            rumusText.text = "";
        }
    }
}