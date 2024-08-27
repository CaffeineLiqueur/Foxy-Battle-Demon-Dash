using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using System.Threading.Tasks;
using UnityEngine.Networking;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

/// <summary>
/// Component to help generate a UI Image or RawImage using Stable Diffusion.
/// </summary>
[ExecuteAlways]
public class Text2ImageSaver : StableDiffusionGenerator
{
    [ReadOnly]
    public string guid = "";
    
    public string prompt;
    public string negativePrompt;
    public string baseFilename = "";

    /// <summary>
    /// List of samplers to display as Drop-Down in the inspector
    /// </summary>
    [SerializeField]
    public string[] samplersList
    {
        get
        {
            if (sdc == null)
                sdc = GameObject.FindObjectOfType<StableDiffusionConfiguration>();
            return sdc.samplers;
        }
    }
    /// <summary>
    /// Actual sampler selected in the drop-down list
    /// </summary>
    [HideInInspector]
    public int selectedSampler = 0;

    public int width = 512;
    public int height = 512;
    public int steps = 90;
    public float cfgScale = 7;
    public long seed = -1;
    public int batch_size = 1;
    public long generatedSeed = -1;
    
    public bool transparent = true;
    public string filename = "";

    string root;
    string mat;

    /// <summary>
    /// List of models to display as Drop-Down in the inspector
    /// </summary>
    [SerializeField]
    public string[] modelsList
    {
        get
        {
            if (sdc == null)
                sdc = GameObject.FindObjectOfType<StableDiffusionConfiguration>();
            return sdc.modelNames;
        }
    }
    /// <summary>
    /// Actual model selected in the drop-down list
    /// </summary>
    [HideInInspector]
    public int selectedModel = 0;


    /// <summary>
    /// On Awake, fill the properties with default values from the selected settings.
    /// </summary>
    void Awake()
    {
#if UNITY_EDITOR
        if (width < 0 || height < 0)
        {
            StableDiffusionConfiguration sdc = GameObject.FindObjectOfType<StableDiffusionConfiguration>();
            if (sdc != null)
            {
                SDSettings settings = sdc.settings;
                if (settings != null)
                {

                    width = settings.width;
                    height = settings.height;
                    steps = settings.steps;
                    cfgScale = settings.cfgScale;
                    seed = settings.seed;
                    return;
                }
            }

            width = 512;
            height = 512;
            steps = 50;
            cfgScale = 7;
            seed = -1;
        }
#endif
    }

    void Update()
    {
#if UNITY_EDITOR
        // Clamp image dimensions values between 128 and 2048 pixels
        if (width < 128) width = 128;
        if (height < 128) height = 128;
        if (width > 2048) width = 2048;
        if (height > 2048) height = 2048;

        // If not setup already, generate a GUID (Global Unique Identifier)
        if (guid == "")
            guid = Guid.NewGuid().ToString();
#endif
    }

    // Internally keep tracking if we are currently generating (prevent re-entry)
    bool generating = false;

    /// <summary>
    /// Callback function for the inspector Generate button.
    /// </summary>
    public void Generate()
    {
        // Start generation asynchronously
        if (!generating && !string.IsNullOrEmpty(prompt))
        {
            StartCoroutine(GenerateAsync());
        }
    }

    /// <summary>
    /// Setup the output path and filename for image generation
    /// </summary>
    void SetupFolders()
    {
        // Get the configuration settings
        if (sdc == null)
            sdc = GameObject.FindObjectOfType<StableDiffusionConfiguration>();

        try
        {
            // Determine output path
            root = Application.dataPath + sdc.settings.OutputFolder;
            if (root == "" || !Directory.Exists(root))
                root = Application.streamingAssetsPath;
            mat = Path.Combine(root, "SDImages");
            // filename = Path.Combine(mat, guid + ".png");
            // int fileIndex = 1;
            // string baseFilename = "haha";
            // string extension = ".png";
            // filename = Path.Combine(mat, baseFilename + fileIndex + extension);

            // If folders not already exists, create them
            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);
            if (!Directory.Exists(mat))
                Directory.CreateDirectory(mat);

            // If the file already exists, delete it
            // while (File.Exists(filename))
            // {
            //     fileIndex++;
            //     filename = Path.Combine(mat, baseFilename + fileIndex + extension);
            // }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message + "\n\n" + e.StackTrace);
        }
    }

public IEnumerator GenerateAsync()
{
    generating = true;

    SetupFolders();

    // Set the model parameters
    yield return sdc.SetModelAsync(modelsList[selectedModel]);

    // 创建 UnityWebRequest 并设置为 POST 方法
    string url = sdc.settings.StableDiffusionServerURL + sdc.settings.TextToImageAPI;
    //using UnityWebRequest request = new UnityWebRequest(url, "POST");
    //request.SetRequestHeader("Content-Type", "application/json");
    using UnityWebRequest request = CreateUnityWebRequest(url);

    

    // 发送请求并等待响应
    yield return request.SendWebRequest();

    // 检查结果
    if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
    {
        Debug.LogError("Error: " + request.error);
        generating = false;
#if UNITY_EDITOR
        EditorUtility.ClearProgressBar();
#endif
        yield break;
    }

    // 处理响应
    ProcessResponse(request.downloadHandler.text);

#if UNITY_EDITOR
    EditorUtility.ClearProgressBar();
#endif
    // PlayerPrefs.SetString("GeneratedImageFilename", filename);
    // PlayerPrefs.Save();
    generating = false;
    yield return null;
}

private UnityWebRequest CreateUnityWebRequest(string url)
{
    UnityWebRequest request = new UnityWebRequest(url, "POST");
    request.SetRequestHeader("Content-Type", "application/json");

    // 添加认证头
    if (sdc.settings.useAuth && !sdc.settings.user.Equals("") && !sdc.settings.pass.Equals(""))
    {
        string encodedCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(sdc.settings.user + ":" + sdc.settings.pass));
        request.SetRequestHeader("Authorization", "Basic " + encodedCredentials);
    }

    // 创建 SDParamsInTxt2Img 并序列化为 JSON
    SDParamsInTxt2Img sd = new SDParamsInTxt2Img();
    sd.prompt = prompt;
    sd.negative_prompt = negativePrompt;
    sd.steps = steps;
    sd.cfg_scale = cfgScale;
    sd.width = width;
    sd.height = height;
    sd.seed = seed;
    sd.tiling = false;
    sd.batch_size = batch_size;
    if (!transparent)
        sd.alwayson_scripts["LayerDiffuse"] = new List<object>{
              false,
              false,
              "(SDXL) Only Generate Transparent Image (Attention Injection)",
              1,
              1,
              null,
              null,
              null,
              "Crop and Resize",
              "",
              "",
              ""
            };

    if (selectedSampler >= 0 && selectedSampler < samplersList.Length)
        sd.sampler_name = samplersList[selectedSampler];

    string json = JsonConvert.SerializeObject(sd);
    Debug.Log("json"+json);
    byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
    request.uploadHandler = new UploadHandlerRaw(bodyRaw);
    request.downloadHandler = new DownloadHandlerBuffer();

    return request;
}

private void ProcessResponse(string responseText)
{
    try
    {
        // 反序列化响应
        SDResponseTxt2Img jsonResponse = JsonConvert.DeserializeObject<SDResponseTxt2Img>(responseText);

        // 如果没有图像，则表示可能发生了错误
        if (jsonResponse.images == null || jsonResponse.images.Length == 0)
        {
            Debug.LogError("No image was returned by the server. This should not happen. Verify that the server is correctly setup.");
            generating = false;
#if UNITY_EDITOR
            EditorUtility.ClearProgressBar();
#endif
            return;
        }

        // 将图像从Base64字符串解码为字节数组
        // Debug.Log(jsonResponse.images[0]);
        Debug.Log("jsonResponse" + jsonResponse.images);
        for (int i = 0; i < batch_size ; i++)
        {
            byte[] imageData = Convert.FromBase64String(jsonResponse.images[i]);

            // 将其写入指定的项目输出文件夹
            // string baseFilename = "haha";
            string extension = ".png";
            // filename = Path.Combine(mat, baseFilename + fileIndex + extension);
            // while (File.Exists(filename))
            //     {
            //         fileIndex++;
            //         filename = Path.Combine(mat, baseFilename + fileIndex + extension);
            //     }
            filename = Path.Combine(mat, baseFilename + i + extension);
            using (FileStream imageFile = new FileStream(filename, FileMode.Create))
            {
                #if UNITY_EDITOR
                            AssetDatabase.StartAssetEditing();
                #endif
                            imageFile.Write(imageData, 0, imageData.Length);
                #if UNITY_EDITOR
                            AssetDatabase.StopAssetEditing();
                            AssetDatabase.SaveAssets();
                #endif
            }

    }
        }

        
    catch (Exception e)
    {
        Debug.LogError(e.Message + "\n\n" + e.StackTrace);
    }
}

}

