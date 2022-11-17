using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Sheets.v4;
using UnityEngine;
using UnityEngine.UI;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Debug = UnityEngine.Debug;

public class DownloadManager : MonoBehaviour
{
    [SerializeField] private string exampleSheetId = "";
    [SerializeField] private Button button;

    private bool _initialized;
    private Credentials _credentials;

    private void Start()
    {
        var credentialsProvider = new CredentialProvider();
        var credentials = credentialsProvider.GetCredentials();

        if (credentials.HasValue && credentials.Value.IsCorrect())
        {
            _credentials = credentials.Value;
            _initialized = true;
        }
        else
        {
            Debug.LogError("[DownloadManager] Service not initialized");
        }
    }

    private void OnEnable()
    {
        button.onClick.AddListener(Click);
    }
    
    private void OnDisable()
    {
        button.onClick.RemoveAllListeners();
    }
    
    private void Click()
    {
        if (!_initialized)
        {
            Debug.LogError("[DownloadManager] Service not initialized");
            return;
        }
        
        Task.Run(() => Download(OnDownloaded));
    }
    
    private async Task Download(Action<string> cb)
    {
        var sw = new Stopwatch();
        sw.Start();
        
        var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
            new ClientSecrets
            {
                ClientId = _credentials.ClientId,
                ClientSecret = _credentials.Secret
            },
            new[] { SheetsService.Scope.Spreadsheets },
            "user", CancellationToken.None);
        
        var service = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = _credentials.AppName
        });
            
        var response = await service.HttpClient.GetAsync($"https://docs.google.com/spreadsheets/d/{exampleSheetId}/export?format=csv");

        if (response.IsSuccessStatusCode)
        {
            var str = await response.Content.ReadAsStringAsync();
            cb.Invoke(str);
        }
        
        sw.Stop();
        Debug.Log($"Total ms:{sw.ElapsedMilliseconds}");
    }

    private void OnDownloaded(string csvTable)
    {
        Debug.Log(csvTable);
    }
}
