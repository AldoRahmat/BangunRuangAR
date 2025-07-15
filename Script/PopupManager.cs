using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance;
    
    [Header("Error Popup")]
    [SerializeField] private GameObject errorPopupPanel;
    [SerializeField] private TextMeshProUGUI errorTitleText;
    [SerializeField] private TextMeshProUGUI errorMessageText;
    [SerializeField] private Button errorOkButton;
    
    [Header("Success Popup")]
    [SerializeField] private GameObject successPopupPanel;
    [SerializeField] private TextMeshProUGUI successTitleText;
    [SerializeField] private TextMeshProUGUI successMessageText;
    [SerializeField] private Button successOkButton;
    
    [Header("Loading Popup")]
    [SerializeField] private GameObject loadingPopupPanel;
    [SerializeField] private TextMeshProUGUI loadingText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // Setup button listeners
        if (errorOkButton != null)
            errorOkButton.onClick.AddListener(CloseErrorPopup);
            
        if (successOkButton != null)
            successOkButton.onClick.AddListener(CloseSuccessPopup);
        
        // Hide all popups initially
        HideAllPopups();
    }

    private void OnDestroy()
    {
        // Clear the static instance when this object is destroyed
        if (Instance == this)
        {
            Instance = null;
        }
    }

    // Helper method to check if instance is valid
    private static bool IsInstanceValid()
    {
        return Instance != null && Instance.gameObject != null;
    }

    public void ShowErrorPopup(string title, string message)
    {
        if (!IsInstanceValid()) return;
        
        if (errorPopupPanel != null)
        {
            if (errorTitleText != null)
                errorTitleText.text = title;
                
            if (errorMessageText != null)
                errorMessageText.text = message;
                
            errorPopupPanel.SetActive(true);
            HideLoadingPopup(); // Hide loading if shown
        }
    }

    public void ShowSuccessPopup(string title, string message)
    {
        if (!IsInstanceValid()) return;
        
        if (successPopupPanel != null)
        {
            if (successTitleText != null)
                successTitleText.text = title;
                
            if (successMessageText != null)
                successMessageText.text = message;
                
            successPopupPanel.SetActive(true);
            HideLoadingPopup(); // Hide loading if shown
        }
    }

    public void ShowLoadingPopup(string message = "Loading...")
    {
        if (!IsInstanceValid()) return;
        
        if (loadingPopupPanel != null)
        {
            if (loadingText != null)
                loadingText.text = message;
                
            loadingPopupPanel.SetActive(true);
        }
    }

    public void CloseErrorPopup()
    {
        if (!IsInstanceValid()) return;
        
        if (errorPopupPanel != null)
            errorPopupPanel.SetActive(false);
    }

    public void CloseSuccessPopup()
    {
        if (!IsInstanceValid()) return;
        
        if (successPopupPanel != null)
            successPopupPanel.SetActive(false);
    }

    public void HideLoadingPopup()
    {
        if (!IsInstanceValid()) return;
        
        if (loadingPopupPanel != null)
            loadingPopupPanel.SetActive(false);
    }

    public void HideAllPopups()
    {
        if (!IsInstanceValid()) return;
        
        CloseErrorPopup();
        CloseSuccessPopup();
        HideLoadingPopup();
    }

    // Auto close popup after certain seconds
    public void ShowErrorPopupWithAutoClose(string title, string message, float seconds = 3f)
    {
        if (!IsInstanceValid()) return;
        
        ShowErrorPopup(title, message);
        StartCoroutine(AutoCloseErrorPopup(seconds));
    }

    public void ShowSuccessPopupWithAutoClose(string title, string message, float seconds = 2f)
    {
        if (!IsInstanceValid()) return;
        
        ShowSuccessPopup(title, message);
        StartCoroutine(AutoCloseSuccessPopup(seconds));
    }

    private IEnumerator AutoCloseErrorPopup(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        
        // Check again if instance is still valid before closing
        if (IsInstanceValid())
        {
            CloseErrorPopup();
        }
    }

    private IEnumerator AutoCloseSuccessPopup(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        
        // Check again if instance is still valid before closing
        if (IsInstanceValid())
        {
            CloseSuccessPopup();
        }
    }

    // Static methods untuk akses yang lebih aman dari script lain
    public static void ShowError(string title, string message)
    {
        if (IsInstanceValid())
        {
            Instance.ShowErrorPopup(title, message);
        }
    }

    public static void ShowSuccess(string title, string message)
    {
        if (IsInstanceValid())
        {
            Instance.ShowSuccessPopup(title, message);
        }
    }

    public static void ShowSuccessWithAutoClose(string title, string message, float seconds = 2f)
    {
        if (IsInstanceValid())
        {
            Instance.ShowSuccessPopupWithAutoClose(title, message, seconds);
        }
    }

    public static void ShowLoading(string message = "Loading...")
    {
        if (IsInstanceValid())
        {
            Instance.ShowLoadingPopup(message);
        }
    }

    public static void HideLoading()
    {
        if (IsInstanceValid())
        {
            Instance.HideLoadingPopup();
        }
    }
}