using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class rumusLuasBola : MonoBehaviour
{
    public TMP_InputField inputJariJari; // r
    public TextMeshProUGUI hasilText;
    public TextMeshProUGUI rumusText;

    public void HitungLuasPermukaan()
    {
        float r;

        bool sukses = float.TryParse(inputJariJari.text, out r);

        if (sukses)
        {
            float luasPermukaan = 4f * Mathf.PI * r * r;

            hasilText.text = $"{luasPermukaan:F2}" + $" cm²";
            rumusText.text = $"4 × π × r²<br>4 × 3.14 × {r}²";
        }
        else
        {
            hasilText.text = "Input tidak valid!";
            rumusText.text = "";
        }
    }
}
