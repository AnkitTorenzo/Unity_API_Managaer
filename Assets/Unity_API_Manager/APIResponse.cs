using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace APIManager
{
    public class APIResponse<T>
    {
        public bool error;
        public string token;
        public int statusCode;
        public string message;
        public string messageCode;
        public T data;

        public static APIResponse<T> Parse(string jsonString)
        {
            //return JsonUtility.FromJson<APIResponse<T>>(jsonString);
            return JsonConvert.DeserializeObject<APIResponse<T>>(jsonString);
        }

        public static T GetDataOnly(string jsonString)
        {
            //return JsonUtility.FromJson<APIResponse<T>>(jsonString).data;
            return JsonConvert.DeserializeObject<APIResponse<T>>(jsonString).data;
        }
    }
}
