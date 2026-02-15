using System;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Simple HTTP Client helper for Unity
/// </summary>
public class HTTPClient
{
    private HttpClient httpClient;
    private string baseURL;
    
    public HTTPClient(string ip, int port) {
        baseURL = $"http://{ip}:{port}";
        httpClient = new HttpClient();
        httpClient.Timeout = TimeSpan.FromSeconds(5);
    }
    
    public void Dispose() {
        httpClient?.Dispose();
    }
    
    /// <summary>
    /// Send GET request
    /// </summary>
    public async Task<string> GetAsync(string endpoint) {
        try {
            string url = baseURL + endpoint;
            Debug.Log($"[HTTP Client] GET {url}");
            
            var startTime = DateTime.Now;
            string response = await httpClient.GetStringAsync(url);
            var elapsed = (DateTime.Now - startTime).TotalMilliseconds;
            
            Debug.Log($"[HTTP Client] Response: {response} ({elapsed:F0}ms)");
            return response;
        } catch (Exception e) {
            Debug.LogError($"[HTTP Client] GET Error: {e.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// Send POST request
    /// </summary>
    public async Task<string> PostAsync(string endpoint, string data) {
        try {
            string url = baseURL + endpoint;
            Debug.Log($"[HTTP Client] POST {url} | Data: {data}");
            
            var content = new StringContent(data);
            var startTime = DateTime.Now;
            var httpResponse = await httpClient.PostAsync(url, content);
            string response = await httpResponse.Content.ReadAsStringAsync();
            var elapsed = (DateTime.Now - startTime).TotalMilliseconds;
            
            Debug.Log($"[HTTP Client] Response: {response} ({elapsed:F0}ms)");
            return response;
        } catch (Exception e) {
            Debug.LogError($"[HTTP Client] POST Error: {e.Message}");
            return null;
        }
    }
}
