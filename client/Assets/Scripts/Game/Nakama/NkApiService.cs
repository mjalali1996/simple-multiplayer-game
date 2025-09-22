using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Game.Nakama;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class NkApiService : HttpService
{
    private readonly string _nakamaAddress;
    private readonly int _nakamaPort;
    private readonly string _httpKey;
    private readonly string _mediaType;
    private Dictionary<string, string> _headers;
    private Dictionary<string, string> _defaultParameters;

    private string BaseUrl => $"http://{_nakamaAddress}:{_nakamaPort}/v2/rpc";

    public NkApiService(string nakamaAddress, int nakamaPort, string httpKey = "defaultkey")
    {
        _nakamaAddress = nakamaAddress;
        _nakamaPort = nakamaPort;
        _httpKey = httpKey;

        _mediaType = "application/json";
        InitHeaders();
        InitDefaultParameters();
    }

    private void InitHeaders()
    {
        _headers = new Dictionary<string, string>();
    }

    private void InitDefaultParameters()
    {
        _defaultParameters = new Dictionary<string, string>
        {
            { "http_key", _httpKey }
        };
    }

    public async Task SetEndMatchResult(EndMatchData data)
    {
        var url = $"{BaseUrl}/SetEndMatch";
        var jsonData = JsonConvert.SerializeObject(data);
        var response = await Post(url, jsonData, _defaultParameters, _headers, _mediaType);
        var content = await response.Content.ReadAsStringAsync();
        Debug.Log($"Set end match result: {response.IsSuccessStatusCode}\n {content}");
    }


    public async Task DisposeMatch(string gameId)
    {
        var url = $"{BaseUrl}/DisposeGameRoom";
        var response = await Post(url, gameId, _defaultParameters, _headers, _mediaType);
        var content = await response.Content.ReadAsStringAsync();
        Debug.Log($"Dispose game room result: {content}");
    }
}