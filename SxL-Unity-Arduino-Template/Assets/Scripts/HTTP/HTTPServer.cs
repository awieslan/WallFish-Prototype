using System;
using System.Net;
using System.Text;
using System.Threading;
using UnityEngine;

/// <summary>
/// Simple HTTP Server helper for Unity
/// </summary>
public class HTTPServer
{
    private HttpListener listener;
    private Thread listenerThread;
    private bool isRunning = false;
    
    public delegate string RequestHandler(HttpListenerContext context);
    private System.Collections.Generic.Dictionary<string, RequestHandler> routes;
    
    public HTTPServer(int port) {
        listener = new HttpListener();
        listener.Prefixes.Add($"http://*:{port}/");
        routes = new System.Collections.Generic.Dictionary<string, RequestHandler>();
    }
    
    /// <summary>
    /// Register a route handler
    /// </summary>
    public void AddRoute(string path, RequestHandler handler) {
        routes[path] = handler;
        Debug.Log($"[HTTP Server] Route registered: {path}");
    }
    
    /// <summary>
    /// Start the HTTP server
    /// </summary>
    public void Start() {
        if (isRunning) return;
        
        isRunning = true;
        listenerThread = new Thread(Listen);
        listenerThread.IsBackground = true;
        listenerThread.Start();
        
        Debug.Log("[HTTP Server] Started on " + string.Join(", ", listener.Prefixes));
    }
    
    /// <summary>
    /// Stop the HTTP server
    /// </summary>
    public void Stop() {
        isRunning = false;
        listener?.Stop();
        listener?.Close();
        Debug.Log("[HTTP Server] Stopped");
    }
    
    private void Listen() {
        listener.Start();
        
        while (isRunning) {
            try {
                HttpListenerContext context = listener.GetContext();
                ProcessRequest(context);
            } catch (Exception e) {
                if (isRunning) {
                    Debug.LogError($"[HTTP Server] Error: {e.Message}");
                }
            }
        }
    }
    
    private void ProcessRequest(HttpListenerContext context) {
        string path = context.Request.Url.LocalPath;
        string method = context.Request.HttpMethod;
        string clientIP = context.Request.RemoteEndPoint.ToString();
        
        Debug.Log($"[HTTP Server] {method} {path} from {clientIP}");
        
        string response = "Not Found";
        int statusCode = 404;
        
        if (routes.ContainsKey(path)) {
            try {
                response = routes[path](context);
                statusCode = 200;
            } catch (Exception e) {
                response = "Internal Server Error: " + e.Message;
                statusCode = 500;
                Debug.LogError($"[HTTP Server] Handler error: {e.Message}");
            }
        }
        
        byte[] buffer = Encoding.UTF8.GetBytes(response);
        context.Response.StatusCode = statusCode;
        context.Response.ContentLength64 = buffer.Length;
        context.Response.OutputStream.Write(buffer, 0, buffer.Length);
        context.Response.OutputStream.Close();
        
        Debug.Log($"[HTTP Server] Response: {response} | Status: {statusCode}");
    }
}
