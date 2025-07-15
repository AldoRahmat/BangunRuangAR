using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class LoginPage : MonoBehaviour
{
    [SerializeField] private TMP_InputField NamaField, passwordField;
    [SerializeField] private Button signUpBtn, loginBtn;
    [SerializeField] private Button togglePasswordBtn; // Button untuk toggle password
    [SerializeField] private GameObject signUpPage;
    [SerializeField] private Image togglePasswordIcon; // Image component dari button
    [SerializeField] private Sprite eyeOpenSprite; // Icon mata terbuka (password terlihat)
    [SerializeField] private Sprite eyeClosedSprite; // Icon mata tertutup (password tersembunyi)
    
    private bool isPasswordVisible = false;
    void Start()
    {
        signUpBtn.onClick.AddListener(() =>
        {
            signUpPage.SetActive(true);
            gameObject.SetActive(false);
        });

        loginBtn.onClick.AddListener(() =>
        {
            AuthenticationManager.Instance.LoginWithUsernameAndPassword(NamaField.text, passwordField.text);
            
        });

        // Toggle password visibility (hanya jika button sudah di-assign)
        if (togglePasswordBtn != null)
        {
            togglePasswordBtn.onClick.AddListener(TogglePasswordVisibility);
        }
        
        // Set initial state
        SetPasswordVisibility(false);
    }

    private void TogglePasswordVisibility()
    {
        isPasswordVisible = !isPasswordVisible;
        SetPasswordVisibility(isPasswordVisible);
    }

    private void SetPasswordVisibility(bool visible)
    {
        if (visible)
        {
            // Password terlihat (tidak bintang)
            passwordField.contentType = TMP_InputField.ContentType.Standard;
            
            // Ganti icon ke mata terbuka
            if (togglePasswordIcon != null && eyeOpenSprite != null)
            {
                togglePasswordIcon.sprite = eyeOpenSprite;
            }
        }
        else
        {
            // Password tersembunyi (bintang)
            passwordField.contentType = TMP_InputField.ContentType.Password;
            
            // Ganti icon ke mata tertutup
            if (togglePasswordIcon != null && eyeClosedSprite != null)
            {
                togglePasswordIcon.sprite = eyeClosedSprite;
            }
        }
        
        // Refresh input field untuk menerapkan perubahan
        passwordField.ForceLabelUpdate();
    }
}