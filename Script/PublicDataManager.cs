using UnityEngine;
using System.Collections.Generic;
using Unity.Services.CloudSave;
using System.Threading.Tasks;

public class PublicDataManager : MonoBehaviour
{
    private static PublicDataManager _instance;
    public static PublicDataManager Instance => _instance;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Simpan data dengan prefix untuk membedakan data guru
    public async Task<bool> SavePublicData(string teacherName, string soalData, string settingsData)
    {
        try
        {
            var data = new Dictionary<string, object>
            {
                { $"public_soal_{teacherName}", soalData },
                { $"public_settings_{teacherName}", settingsData }
            };

            await CloudSaveService.Instance.Data.Player.SaveAsync(data);
            
            Debug.Log($"Successfully saved public data for {teacherName}");

            // Update guru list
            await UpdateGuruList(teacherName);
            
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save public data: {e.Message}");
            return false;
        }
    }

    public async Task<bool> LoadPublicData(string teacherName, System.Action<string, string> onSuccess)
    {
        try
        {
            var keys = new HashSet<string> 
            { 
                $"public_soal_{teacherName}", 
                $"public_settings_{teacherName}" 
            };

            var result = await CloudSaveService.Instance.Data.Player.LoadAsync(keys);

            string soalData = "";
            string settingsData = "";

            if (result.TryGetValue($"public_soal_{teacherName}", out var soal))
            {
                soalData = soal.Value.GetAsString();
            }

            if (result.TryGetValue($"public_settings_{teacherName}", out var settings))
            {
                settingsData = settings.Value.GetAsString();
            }

            if (!string.IsNullOrEmpty(soalData) && !string.IsNullOrEmpty(settingsData))
            {
                onSuccess?.Invoke(soalData, settingsData);
                return true;
            }

            Debug.LogError($"Data tidak lengkap untuk guru: {teacherName}");
            return false;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load public data: {e.Message}");
            return false;
        }
    }

    private async Task UpdateGuruList(string teacherName)
    {
        try
        {
            var result = await CloudSaveService.Instance.Data.Player.LoadAsync(
                new HashSet<string> { "public_guru_list" }
            );

            List<string> guruList = new List<string>();

            if (result.TryGetValue("public_guru_list", out var data))
            {
                var wrapper = JsonUtility.FromJson<GuruListWrapper>(data.Value.GetAsString());
                guruList = wrapper.list ?? new List<string>();
            }

            if (!guruList.Contains(teacherName))
            {
                guruList.Add(teacherName);
            }

            var updatedWrapper = new GuruListWrapper { list = guruList };
            var updateData = new Dictionary<string, object>
            {
                { "public_guru_list", JsonUtility.ToJson(updatedWrapper) }
            };

            await CloudSaveService.Instance.Data.Player.SaveAsync(updateData);
            Debug.Log($"Updated guru list with {teacherName}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to update guru list: {e.Message}");
        }
    }

    public async Task<List<string>> GetGuruList()
    {
        try
        {
            var result = await CloudSaveService.Instance.Data.Player.LoadAsync(
                new HashSet<string> { "public_guru_list" }
            );

            if (result.TryGetValue("public_guru_list", out var data))
            {
                var wrapper = JsonUtility.FromJson<GuruListWrapper>(data.Value.GetAsString());
                return wrapper.list ?? new List<string>();
            }

            return new List<string>();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to get guru list: {e.Message}");
            return new List<string>();
        }
    }

    [System.Serializable]
    public class GuruListWrapper
    {
        public List<string> list;
    }
}