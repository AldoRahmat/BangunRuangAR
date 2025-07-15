using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class rumusLuasLimas : MonoBehaviour
{
    public TMP_InputField inputSisi;        // Panjang sisi alas persegi
    public TMP_InputField inputTinggiLimas; // Tinggi limas
    public TextMeshProUGUI hasilText;
    public TextMeshProUGUI rumusText;

    public void HitungLuasPermukaanLimas()
    {
        float sisi, tinggiLimas;

        bool sukses1 = float.TryParse(inputSisi.text, out sisi);
        bool sukses2 = float.TryParse(inputTinggiLimas.text, out tinggiLimas);

        if (sukses1 && sukses2)
        {
            // Hitung tinggi segitiga tegak
            float tinggiSegitiga = Mathf.Sqrt(Mathf.Pow(sisi / 2f, 2f) + Mathf.Pow(tinggiLimas, 2f));
            string tinggiSegitigaStr = tinggiSegitiga.ToString("F2");

            // Hitung luas permukaan limas persegi
            float luasAlas = sisi * sisi;
            float luasSelubung = 2 * sisi * tinggiSegitiga;
            float luasPermukaan = luasAlas + luasSelubung;

            hasilText.text = luasPermukaan.ToString("F2") + $" cm²";

            // Tampilkan rumus secara deskriptif
            rumusText.text = $"(s × s) + (4 × (½ × s × t\u25B2))\n" +
            $"({sisi}  × {sisi}) + (4 × (½ × {sisi} × {tinggiSegitigaStr}))\n" +
                             $"{luasAlas:F2} + {luasSelubung:F2}";
        }
        else
        {
            hasilText.text = "Input tidak valid!";
            rumusText.text = "";
        }
    }
}