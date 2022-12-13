using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace APIManager
{
    public static class APIManager
    {
        //https://hajar.staging-server.in:8083/
        // private const string BASE_URL = "https://48d8-2405-201-2014-303c-6481-b7e1-87e8-20d1.ngrok.io/api/v1/";
        private const string BASE_URL = "https://hajar.staging-server.in:8083/api/v1/";
        private const string PF_TOKEN_KEY = "authToken";
        private const string PF_TOKEN_DEFAULT = "TOKEN";
        private static string authToken = PF_TOKEN_DEFAULT;

        public delegate void APICallback<T>(APIResponse<T> response);

        public static IEnumerator CallPostAPI<T>(string apiName, Dictionary<string, string> data, APICallback<T> callback, APICallback<T> errorCallback = null, bool isGET = false)
        {

            string apiURL = GetAPIUrl(apiName);
            Debug.Log($"Calling {apiURL} API, with data: {System.Environment.NewLine}{ShowInputData(data)}");

            UnityWebRequest request = (!isGET) ? UnityWebRequest.Post(apiURL, data) : UnityWebRequest.Get(apiURL);
            request.SetRequestHeader("Accept", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {authToken}");
            request.timeout = 15;

            yield return request.SendWebRequest();
            if (!request.result.HasAnyError())
            {
                string str = request.downloadHandler.text;
                Debug.Log($"{apiName} API Response: {str}");
                APIResponse<T> response = APIResponse<T>.Parse(str);

                if (!response.error)
                {
                    callback?.Invoke(response);
                }
                else
                {
                    Debug.LogError($"Server Error Code: {response.statusCode}");
                    errorCallback?.Invoke(response);
                }
            }
            else
            {
                Debug.LogError($"{request.downloadHandler.text}");
                APIResponse<T> response = new APIResponse<T>();
                response.error = true;
                response.message = $"<b>Error: {request.result}</b>{System.Environment.NewLine}{request.error}";

                errorCallback?.Invoke(response);
                Debug.Log($"{request.responseCode} {request.result} {request.error}");
            }
            request.Dispose();
        }

        public static IEnumerator CallPostAPI<T>(string apiName, WWWForm data, APICallback<T> callback, APICallback<T> errorCallback = null)
        {

            string apiURL = GetAPIUrl(apiName);
            Debug.Log($"Calling {apiURL} API, with Form data");
            UnityWebRequest request = UnityWebRequest.Post(apiURL, data);
            request.SetRequestHeader("Accept", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {authToken}");
            request.timeout = 15;

            yield return request.SendWebRequest();
            if (!request.result.HasAnyError())
            {
                string str = request.downloadHandler.text;
                Debug.Log($"{apiName} API Response: {str}");
                APIResponse<T> response = APIResponse<T>.Parse(str);

                if (!response.error)
                {
                    callback?.Invoke(response);
                }
                else
                {
                    Debug.LogError($"Server Error Code: {response.statusCode}");
                    errorCallback?.Invoke(response);
                }
            }
            else
            {
                Debug.LogError($"{request.downloadHandler.text}");
                APIResponse<T> response = new APIResponse<T>();
                response.error = true;
                response.message = $"<b>Error: {request.result}</b>{System.Environment.NewLine}{request.error}";

                errorCallback?.Invoke(response);
                Debug.Log($"{request.responseCode} {request.result} {request.error}");
            }
            request.Dispose();
        }

        public static IEnumerator DownloadImage(string url, Action<Texture2D> callback)
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
            yield return request.SendWebRequest();

            if (request.result.HasAnyError())
                yield break;

            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            callback(texture);
            request.Dispose();
        }

        public static void SetAuthToken(string _authToken)
        {
            Debug.Log($"settings Token");
            authToken = _authToken;
            PlayerPrefs.SetString(PF_TOKEN_KEY, authToken);
        }

        public static void ClearToken()
        {
            authToken = PF_TOKEN_DEFAULT;
            PlayerPrefs.SetString(PF_TOKEN_KEY, PF_TOKEN_DEFAULT);
        }

        public static bool TokenAvailable()
        {
            if (authToken != PF_TOKEN_DEFAULT)
                return true;

            authToken = PlayerPrefs.GetString(PF_TOKEN_KEY, PF_TOKEN_DEFAULT);
            Debug.Log($"Getting the Token: {PlayerPrefs.GetString(PF_TOKEN_KEY)}");
            if (authToken == PF_TOKEN_DEFAULT || string.IsNullOrEmpty(authToken))
            {
                return false;
            }

            return true;
        }

        private static string ShowInputData(Dictionary<string, string> data)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var item in data)
            {
                builder.Append(System.Environment.NewLine).Append(item.Key).Append("=>").Append(item.Value);
            }
            return builder.ToString();
        }

        private static string GetAPIUrl(string apiName)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(BASE_URL).Append(apiName);
            return builder.ToString();
        }

        public static string AuthToken => authToken;
    }

    internal static class InternalExtensionMethods
    {
        public static bool HasAnyError(this UnityWebRequest.Result result)
        {
            return (result == UnityWebRequest.Result.ConnectionError || result == UnityWebRequest.Result.DataProcessingError);
        }
    }
}