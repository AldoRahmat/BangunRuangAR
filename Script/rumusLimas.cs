using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class rumusLimas : MonoBehaviour
{
    public TMP_InputField input1; // Luas alas
    public TMP_InputField input2; // Tinggi limas
    public TextMeshProUGUI hasilText;
    public TextMeshProUGUI rumusText;

    public void HitungVolumeLimas()
    {
        float luas, tinggi;

        bool sukses1 = float.TryParse(input1.text, out luas);
        bool sukses2 = float.TryParse(input2.text, out tinggi);

        if (sukses1 && sukses2)
        {
            float volume = (1f / 3f) * luas * luas * tinggi;
            hasilText.text = volume.ToString() + $" cm³";

            // Menampilkan rumus
            rumusText.text = $"1/3 × s² × t<br>1/3 × {luas}² × {tinggi}";
        }
        else
        {
            hasilText.text = "Input tidak valid!";
            rumusText.text = "";
        }
    }
}
