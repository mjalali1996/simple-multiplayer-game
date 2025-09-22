using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;


public class HttpService
{
    protected async Task<HttpResponseMessage> Post(string url, string data, IDictionary<string, string> parameters,
        IDictionary<string, string> headers, string mediaType)
    {
        url = InjectParameters(parameters, url) + "&unwrap";

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
        // Set headers
        InjectHeaders(headers, httpClient);

        // Set the request content
        var content = new StringContent(data, Encoding.UTF8, mediaType);

        // Make the POST request
        var response = await httpClient.PostAsync(url, content);

        return response;
    }

    protected async Task<HttpResponseMessage> Get(string url, IDictionary<string, string> parameters, string mediaType)
    {
        url = InjectParameters(parameters, url);

        using var client = new HttpClient();
        // Set headers if needed
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));


        // Make the GET request
        var response = await client.GetAsync(url);

        return response;
    }

    protected async Task<HttpResponseMessage> Put(string url, string data, IDictionary<string, string> headers,
        string mediaType)
    {
        url += "&unwrap";
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));

        // Set headers if needed
        InjectHeaders(headers, httpClient);

        // Set the request content
        HttpContent content = new StringContent(data, Encoding.UTF8, mediaType);

        // Make the PUT request
        var response = await httpClient.PutAsync(url, content);

        return response;
    }

    protected static async Task<HttpResponseMessage> SendDeleteRequest(string url,
        IDictionary<string, string> parameters)
    {
        // Build the URL
        url = InjectParameters(parameters, url);

        using var client = new HttpClient();

        // Make the DELETE request
        var response = await client.DeleteAsync(url);

        return response;
    }

    private static void InjectHeaders(IDictionary<string, string> headers, HttpClient httpClient)
    {
        foreach (var (key, value) in headers)
        {
            httpClient.DefaultRequestHeaders.Add(key, value);
        }
    }

    private static string InjectParameters(IDictionary<string, string> parameters, string url)
    {
        var firstParam = true;
        foreach (var (key, value) in parameters)
        {
            if (firstParam)
            {
                url += $"?{key}={value}";
                firstParam = false;
            }
            else
                url += $"&{key}={value}";
        }

        return url;
    }
}