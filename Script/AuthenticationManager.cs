using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;

public class AuthenticationManager : MonoBehaviour
{    
    public static AuthenticationManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeUnity();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private async void InitializeUnity()
    {
        try
        {
            await UnityServices.InitializeAsync();

            // Hanya tambahkan handler jika sudah inisialisasi
            AuthenticationService.Instance.SignedIn += OnSignedIn;
        }
        catch (System.Exception e)
        {
            Debug.LogError("UnityServices init failed: " + e.Message);
        }
    }

    private void OnDestroy()
    {
        // Clear static instance when destroyed
        if (Instance == this)
        {
            Instance = null;
        }

        // Remove event handler if Unity Services is initialized
        if (UnityServices.State == ServicesInitializationState.Initialized)
        {
            AuthenticationService.Instance.SignedIn -= OnSignedIn;
        }
    }

    private void OnSignedIn()
    {
        Debug.Log("Login/Signup successful.");
        Debug.Log("Username: " + AuthenticationService.Instance.PlayerInfo.Username);
        Debug.Log("Player ID: " + AuthenticationService.Instance.PlayerId);

        // Show success popup using the safer static method
        PopupManager.ShowSuccessWithAutoClose("Success", "Login successful!", 1.5f);
        
        // Delay scene change to show success message
        Invoke(nameof(LoadMainScene), 1.5f);
    }

    private void LoadMainScene()
    {        
        SceneManager.LoadScene("MainMenuGuru");
    }

    public async void SignUpWithUsernameAndPassword(string username, string password)
    {
        // Validation
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            PopupManager.ShowError("Input Tidak Valid", "Username dan password tidak boleh kosong.");
            return;
        }

        if (username.Length < 3)
        {
            PopupManager.ShowError("Username Tidak Valid", "Username minimal 3 karakter.");
            return;
        }

        if (password.Length < 6)
        {
            PopupManager.ShowError("Password Tidak Valid", "Password minimal 6 karakter.");
            return;
        }

        PopupManager.ShowLoading("Membuat akun...");

        try
        {
            PlayerPrefs.SetString("LastPassword", password);

            if (AuthenticationService.Instance.IsSignedIn)
                AuthenticationService.Instance.SignOut();

            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);

            await AddGuruToBankSoalList(username, password);
        }
        catch (AuthenticationException authEx)
        {
            Debug.LogError("Sign up failed (auth): " + authEx.Message);
            HandleAuthenticationError(authEx, "Sign Up Failed");
        }
        catch (RequestFailedException reqEx)
        {
            Debug.LogError("Sign up failed (request): " + reqEx.Message);
            HandleRequestError(reqEx, "Sign Up Failed");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Sign up failed: " + e.Message);
            PopupManager.ShowError("Pendaftaran Gagal", "Terjadi kesalahan tak terduga. Silakan coba lagi.");
        }
        finally
        {
            PopupManager.HideLoading();
        }
    }

    public async void LoginWithUsernameAndPassword(string username, string password)
    {
        // Validation
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            PopupManager.ShowError("Input Tidak Valid", "Username dan password tidak boleh kosong.");
            return;
        }

        PopupManager.ShowLoading("Masuk...");

        try
        {
            PlayerPrefs.SetString("LastPassword", password);

            if (AuthenticationService.Instance.IsSignedIn)
                AuthenticationService.Instance.SignOut();

            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
        }
        catch (AuthenticationException authEx)
        {
            Debug.LogError("Login failed (auth): " + authEx.Message);
            HandleAuthenticationError(authEx, "Login Failed");
        }
        catch (RequestFailedException reqEx)
        {
            Debug.LogError("Login failed (request): " + reqEx.Message);
            HandleRequestError(reqEx, "Login Failed");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Login failed: " + e.Message);
            PopupManager.ShowError("Login Gagal", "Terjadi kesalahan tak terduga. Silakan coba lagi.");
        }
        finally
        {
            PopupManager.HideLoading();
        }
    }

    private void HandleAuthenticationError(AuthenticationException authEx, string title)
    {
        string message;
        
        // Using only the error codes that actually exist in Unity Authentication
        if (authEx.ErrorCode == AuthenticationErrorCodes.AccountAlreadyLinked)
            message = "Akun ini sudah terhubung dengan pengguna lain.";
        else if (authEx.ErrorCode == AuthenticationErrorCodes.AccountLinkLimitExceeded)
            message = "Batas tautan akun terlampaui.";
        else if (authEx.ErrorCode == AuthenticationErrorCodes.InvalidParameters)
            message = "Format username atau password tidak valid.";
        else if (authEx.ErrorCode == AuthenticationErrorCodes.InvalidSessionToken)
            message = "Sesi berakhir. Silakan coba lagi.";
        else if (authEx.ErrorCode == AuthenticationErrorCodes.ClientInvalidUserState)
            message = "Status pengguna tidak valid. Silakan coba masuk lagi.";
        else
        {
            // Handle common authentication scenarios based on error message
            string errorMsg = authEx.Message.ToLower();
            if (errorMsg.Contains("network") || errorMsg.Contains("connection"))
                message = "Kesalahan jaringan. Periksa koneksi internet Anda.";
            else if (errorMsg.Contains("unauthorized") || errorMsg.Contains("invalid credentials") || errorMsg.Contains("invalid username or password"))
                message = "Username atau password salah.";
            else if (errorMsg.Contains("user not found") || errorMsg.Contains("username"))
                message = "Pengguna tidak ditemukan. Periksa username Anda.";
            else if (errorMsg.Contains("password"))
                message = "Password salah. Silakan coba lagi.";
            else if (errorMsg.Contains("too many") || errorMsg.Contains("rate limit"))
                message = "Terlalu banyak percobaan. Tunggu sebentar dan coba lagi.";
            else if (errorMsg.Contains("already exists") || errorMsg.Contains("username taken"))
                message = "Username sudah digunakan. Pilih username lain.";
            else
                message = $"Kesalahan autentikasi: {authEx.Message}";
        }

        PopupManager.ShowError(title, message);
    }

    private void HandleRequestError(RequestFailedException reqEx, string title)
    {
        string message;

        // Handle HTTP error codes
        if (reqEx.ErrorCode == 400)
            message = "Permintaan buruk. Periksa input Anda.";
        else if (reqEx.ErrorCode == 401)
            message = "Tidak diotorisasi. Kredensial tidak valid.";
        else if (reqEx.ErrorCode == 403)
            message = "Akses dilarang.";
        else if (reqEx.ErrorCode == 404)
            message = "Layanan tidak ditemukan.";
        else if (reqEx.ErrorCode == 409)
            message = "Username sudah ada. Pilih username lain.";
        else if (reqEx.ErrorCode == 429)
            message = "Terlalu banyak permintaan. Tunggu dan coba lagi.";
        else if (reqEx.ErrorCode == 500)
            message = "Kesalahan server. Coba lagi nanti.";
        else if (reqEx.ErrorCode == 503)
            message = "Layanan tidak tersedia. Coba lagi nanti.";
        else
            message = $"Permintaan gagal: {reqEx.Message}";

        PopupManager.ShowError(title, message);
    }

    private async Task AddGuruToBankSoalList(string newGuru, string guruPassword)
    {
        try
        {
            Debug.Log("Switching to BankSoal to update guru_list...");

            AuthenticationService.Instance.SignOut();
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync("BankSoal", "BankSoal#1234");

            var result = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { "guru_list" });

            List<string> guruList = new List<string>();
            if (result.TryGetValue("guru_list", out var json))
            {
                guruList = JsonUtility.FromJson<GuruListWrapper>(json.Value.GetAs<string>()).guruIds;
                Debug.Log("guru_list ditemukan. Total: " + guruList.Count);
            }
            else
            {
                Debug.LogWarning("guru_list belum ada, membuat baru...");
            }

            if (!guruList.Contains(newGuru))
            {
                guruList.Add(newGuru);
                string updatedJson = JsonUtility.ToJson(new GuruListWrapper { guruIds = guruList });

                await CloudSaveService.Instance.Data.Player.SaveAsync(new Dictionary<string, object>
                {
                    { "guru_list", updatedJson }
                });

                Debug.Log("Guru baru ditambahkan ke guru_list: " + newGuru);
            }
            else
            {
                Debug.Log("Guru sudah ada dalam guru_list: " + newGuru);
            }

            AuthenticationService.Instance.SignOut();
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(newGuru, guruPassword);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Add to guru_list failed: " + e.Message);
            PopupManager.ShowError("Kesalahan Setup", "Akun berhasil dibuat tetapi setup tidak lengkap. Anda masih bisa login normal.");
        }
    }

    [System.Serializable]
    public class GuruListWrapper
    {
        public List<string> guruIds;
    }
}