using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class PopupManager : MonoBehaviour
{
    public GameObject popupPanel; // 引用 PopupPanel 对象
    public Button openButton; // 引用 OpenButton 对象
    public Button closeButton; // 引用 CloseButton 对象
    public Button changeButton; // 引用 ChangeButton 对象
    public TMP_InputField inputField; // 引用 TextMeshPro - Input Field 对象

    public GameObject waitPanel; // 引用 WaitPanel 对象
    public Slider progressBar; // 引用进度条对象

    public GameObject selectPanel; // 引用 selectPanel 对象
    public RawImage selectImage1;
    public RawImage selectImage2;
    public RawImage selectImage3;
    public Button selectButton1;
    public Button selectButton2;
    public Button selectButton3;

    public GameObject player;
    public GameObject pet;
    Rigidbody2D playerRB;
    PetController petControllerComponent;

    public string filename; 
    public GameObject targetObject;
    SpriteRenderer spriteRenderer;
    string file_path1;
    string file_path2;
    string file_path3;
    string file_path = "/home/jh/zyh/unityproject/MiniGame01/Assets/StreamingAssets/SDImages/prop.png";

    void Start()
    {
        playerRB = player.GetComponent<Rigidbody2D>();
        petControllerComponent = pet.GetComponent<PetController>();
        spriteRenderer = targetObject.GetComponent<SpriteRenderer>();
        // 初始化时隐藏弹出界面
        popupPanel.SetActive(false);
        waitPanel.SetActive(false);
        selectPanel.SetActive(false);

        // 添加按钮点击事件
        openButton.onClick.AddListener(OpenPopup);
        closeButton.onClick.AddListener(ClosePopup);
        changeButton.onClick.AddListener(changeSth);
        selectButton1.onClick.AddListener(selectSth1);
        selectButton2.onClick.AddListener(selectSth2);
        selectButton3.onClick.AddListener(selectSth3);
    }

    void OpenPopup()
    {
        // 显示弹出界面
        popupPanel.SetActive(true);
    }

    void ClosePopup()
    {
        // 隐藏弹出界面
        popupPanel.SetActive(false);
    }

    void OpenWait()
    {
        // 显示等待界面
        waitPanel.SetActive(true);
    }

    void CloseWait()
    {
        // 隐藏等待界面
        waitPanel.SetActive(false);
    }

    void OpenSelect()
    {
        // 显示选择界面
        selectPanel.SetActive(true);
    }

    void CloseSelect()
    {
        // 隐藏选择界面
        selectPanel.SetActive(false);
    }

    void changeSth()
    {
        ClosePopup();
        petControllerComponent.canShoot = false;
        OpenWait();
        playerRB.isKinematic = true;
        // 获取输入框中的文本
        string userInput = inputField.text;
        Debug.Log("你输入了：" + userInput);

        Text2ImageSaver text2ImgSaverComponent = waitPanel.GetComponent<Text2ImageSaver>();
        if (text2ImgSaverComponent != null)
        {
            text2ImgSaverComponent.prompt += userInput;
            text2ImgSaverComponent.Generate(); 
        }

        // 读取图像文件路径
        file_path1 = "/home/jh/zyh/unityproject/MiniGame01/Assets/StreamingAssets/SDImages/prop0.png";
        file_path2 = "/home/jh/zyh/unityproject/MiniGame01/Assets/StreamingAssets/SDImages/prop1.png";
        file_path3 = "/home/jh/zyh/unityproject/MiniGame01/Assets/StreamingAssets/SDImages/prop2.png";
        string [] filePaths = { file_path1, file_path2, file_path3 };

        foreach (string path in filePaths)
        {
            while (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        StartCoroutine(CheckFilesAndLoadImages(filePaths));
    }

    IEnumerator CheckFilesAndLoadImages(string[] filePaths)
    {
        progressBar.value = 0;
        // float progress = 0;

        foreach (string path in filePaths)
        {
            while (!File.Exists(path))
            {
                yield return new WaitForSeconds(0.1f); // 等待文件生成
                            // progress += 1.0f / filePaths.Length;
            if (progressBar.value != 1)
                progressBar.value += 0.0020f;
            }
        }

        CloseWait();
        playerRB.isKinematic = false;
        OpenSelect();
        LoadImage(filePaths[0], selectImage1);
        LoadImage(filePaths[1], selectImage2);
        LoadImage(filePaths[2], selectImage3);

    }

    void selectSth1()
    {
        Texture2D texture2D = selectImage1.texture as Texture2D;
        Sprite sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.zero);
        spriteRenderer.sprite = sprite;
        targetObject.transform.localScale = new Vector3(0.05f, 0.05f, 1f); // 缩放为原来的五分之一
        CloseSelect();
        petControllerComponent.canShoot = true;
    }

    void selectSth2()
    {
        Texture2D texture2D = selectImage2.texture as Texture2D;
        Sprite sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.zero);
        spriteRenderer.sprite = sprite;
        targetObject.transform.localScale = new Vector3(0.05f, 0.05f, 1f); // 缩放为原来的五分之一
        CloseSelect();
        petControllerComponent.canShoot = true;
    }

    void selectSth3()
    {
        Texture2D texture2D = selectImage3.texture as Texture2D;
        Sprite sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.zero);
        spriteRenderer.sprite = sprite;
        targetObject.transform.localScale = new Vector3(0.05f, 0.05f, 1f); // 缩放为原来的五分之一
        CloseSelect();
        petControllerComponent.canShoot = true;
    }

    public void LoadImage(string path, RawImage targetRawImage)
    {
        if (File.Exists(path))
        {
            byte[] imageData = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageData);
            texture.Apply();
            targetRawImage.texture = texture;
        }
        else
        {
            Debug.LogError("文件不存在: " + path);
        }
    }
}
