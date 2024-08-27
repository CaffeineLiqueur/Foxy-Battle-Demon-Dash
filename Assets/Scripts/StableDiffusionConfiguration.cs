using System;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Global Stable Diffusion parameters configuration.
/// </summary>
[ExecuteInEditMode]
public class StableDiffusionConfiguration : MonoBehaviour
{
    [SerializeField] 
    public SDSettings settings;

    [SerializeField]
    public string[] samplers = new string[]{
        "Euler a", "Euler", "LMS", "Heun", "DPM2", "DPM2 a", "DPM++ 2S a", "DPM++ 2M", "DPM++ SDE", "DPM fast", "DPM adaptive",
        "LMS Karras", "DPM2 Karras", "DPM2 a Karras", "DPM++ 2S a Karras", "DPM++ 2M Karras", "DPM++ SDE Karras", "DDIM", "PLMS"
    };

    [SerializeField]
    public string[] modelNames;

    /// <summary>
    /// Data structure that represents a Stable Diffusion model to help deserialize from JSON string.
    /// </summary>
    class Model
    {
        public string title;
        public string model_name;
        public string hash;
        public string sha256;
        public string filename;
        public string config;
    }

    /// <summary>
    /// Method called when the user click on List Model from the inspector.
    /// </summary>
    public void ListModels()
    {
        StartCoroutine(ListModelsAsync());
    }

    /// <summary>
    /// Get the list of available Stable Diffusion models.
    /// </summary>
    /// <returns></returns>
    IEnumerator ListModelsAsync()
    {
        // Stable diffusion API url for getting the models list
        string url = settings.StableDiffusionServerURL + settings.ModelsAPI;

        using UnityWebRequest request = new UnityWebRequest(url, "GET");
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        
        if (settings.useAuth && !settings.user.Equals("") && !settings.pass.Equals(""))
        {
            Debug.Log("Using API key to authenticate");
            byte[] bytesToEncode = Encoding.UTF8.GetBytes(settings.user + ":" + settings.pass);
            string encodedCredentials = Convert.ToBase64String(bytesToEncode);
            request.SetRequestHeader("Authorization", "Basic " + encodedCredentials);
        }
        
        yield return request.SendWebRequest();

        try
        {
            // Deserialize the response to a class
            Model[] ms = JsonConvert.DeserializeObject<Model[]>(request.downloadHandler.text);

            // Keep only the names of the models
            List<string> modelsNames = new List<string>();

            foreach (Model m in ms) 
                modelsNames.Add(m.model_name);

            // Convert the list into an array and store it for futur use
            modelNames = modelsNames.ToArray();
        }
        catch (Exception)
        {
            Debug.Log(request.downloadHandler.text);
            Debug.Log("Server needs and API key authentication. Please check your settings!");
        }
    }

    /// <summary>
    /// Set a model to use by Stable Diffusion.
    /// </summary>
    /// <param name="modelName">Model to set</param>
    /// <returns></returns>
public IEnumerator SetModelAsync(string modelName)
{
    // Stable diffusion API url for setting a model
    string url = settings.StableDiffusionServerURL + settings.OptionAPI;

    // Load the list of models if not filled already
    if (modelNames == null || modelNames.Length == 0)
        yield return ListModelsAsync();

    // 创建 UnityWebRequest 并设置为 POST 方法
    UnityWebRequest request = CreateUnityWebRequest(url, modelName);

    // 发送请求并等待响应
    yield return request.SendWebRequest();

    // 检查结果
    if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
    {
        Debug.LogError("Error: " + request.error);
    }
    else
    {
        Debug.Log("Response: " + request.downloadHandler.text);
    }
}

private UnityWebRequest CreateUnityWebRequest(string url, string modelName)
{
    // 创建 UnityWebRequest 并设置为 POST 方法
    UnityWebRequest request = new UnityWebRequest(url, "POST");
    request.SetRequestHeader("Content-Type", "application/json");

    // 添加认证头
    if (settings.useAuth && !settings.user.Equals("") && !settings.pass.Equals(""))
    {
        string encodedCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(settings.user + ":" + settings.pass));
        request.SetRequestHeader("Authorization", "Basic " + encodedCredentials);
    }

    // 创建 SDOption 并序列化为 JSON
    SDOption sd = new SDOption();
    sd.sd_model_checkpoint = modelName;
    string json = JsonConvert.SerializeObject(sd);

    // 将 JSON 数据转换为字节数组并附加到请求
    byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
    request.uploadHandler = new UploadHandlerRaw(bodyRaw);
    request.downloadHandler = new DownloadHandlerBuffer();

    return request;
}

}
