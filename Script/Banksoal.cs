using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;

public class Banksoal : MonoBehaviour
{
     async void Start()
    {
        await UnityServices.InitializeAsync();

        try
        {
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync("BankSoal", "BankSoal#1234");
            Debug.Log("Akun BankSoal berhasil dibuat.");
        }
        catch (AuthenticationException ex)
        {
            Debug.LogWarning("BankSoal mungkin sudah terdaftar: " + ex.Message);
        }
    }
}