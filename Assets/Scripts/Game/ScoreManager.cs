using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public GameObject waitPanel;
    public Slider progressBar; 

    public GameObject selectPanel;
    public RawImage selectImage1;
    public RawImage selectImage2;
    public RawImage selectImage3;
    public Button selectButton1;
    public Button selectButton2;
    public Button selectButton3;

    public GameObject targetObject;
    public GameObject player;
    public GameObject pet;
    public GameObject petBullet;
    public GameObject petBulletSaver;
    Rigidbody2D playerRB;
    SpriteRenderer spriteRenderer;
    SpriteRenderer petBulletSpriteRenderer;
    PetController petControllerComponent;

    public int score = 0;
    public TextMeshProUGUI scoreText; // Reference to the UI Text component

    bool flag1 = false;
    bool flag2 = false;
    // public PlayerControler player; // Reference to the GameManager script
    // private Vector3 offset;   // Initial offset
    // PlayerControler playerControlerComponent;
    // Start is called before the first frame update
    string file_path1;
    string file_path2;
    string file_path3;
    string petBullet_path1;
    string file_path = "/home/jh/zyh/unityproject/MiniGame01/Assets/StreamingAssets/SDImages/pet.png";
    string petBullet_path = "/home/jh/zyh/unityproject/MiniGame01/Assets/StreamingAssets/SDImages/petBullet.png";
    float ori_shootInterval;
    void Start()
    {
        spriteRenderer = targetObject.GetComponent<SpriteRenderer>();
        petBulletSpriteRenderer = petBullet.GetComponent<SpriteRenderer>();
        // playerControlerComponent = player.GetComponent<PlayerControler>();
        playerRB = player.GetComponent<Rigidbody2D>();
        petControllerComponent = pet.GetComponent<PetController>();
        selectButton1.onClick.AddListener(selectSth1);
        selectButton2.onClick.AddListener(selectSth2);
        selectButton3.onClick.AddListener(selectSth3);
    }

    void Awake()
    {
        // 确保只有一个 ScoreManager 实例存在
        if (instance == null)
        {
            instance = this;
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
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

    // Update is called once per frame
    void Update()
    {
        scoreText.text = "分数：" + score.ToString();
        // 当分数达到一定时，可以触发奖励。
        if ((score > 50 && !flag1) || (score>100 && !flag2))
        {
            if(score < 100)
                flag1 = true;
            else
                flag2 = true;
        
            ori_shootInterval = petControllerComponent.shootInterval;
            // petControllerComponent.shootInterval = 10f;
            petControllerComponent.canShoot = false;

            OpenWait();
            playerRB.isKinematic = true;
            Text2ImageSaver text2ImgSaverComponent = waitPanel.GetComponent<Text2ImageSaver>();
            // Text2ImageSaver petBulletText2ImgSaverComponent = petBullet.GetComponent<Text2ImageSaver>();

            if (text2ImgSaverComponent != null)
            {
                text2ImgSaverComponent.Generate(); 
            }

            // 读取图像文件路径
            file_path1 = "/home/jh/zyh/unityproject/MiniGame01/Assets/StreamingAssets/SDImages/pet0.png";
            file_path2 = "/home/jh/zyh/unityproject/MiniGame01/Assets/StreamingAssets/SDImages/pet1.png";
            file_path3 = "/home/jh/zyh/unityproject/MiniGame01/Assets/StreamingAssets/SDImages/pet2.png";
            
            // petBullet_path1 = "/home/jh/zyh/unityproject/MiniGame01/Assets/StreamingAssets/SDImages/petBullet0.png";

            string [] filePaths = { file_path1, file_path2, file_path3 };
            foreach (string path in filePaths)
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }



            StartCoroutine(CheckFilesAndLoadImages(new string[] { file_path1, file_path2, file_path3 }));

            // if (petBulletText2ImgSaverComponent != null)
            // {
            //     petBulletText2ImgSaverComponent.Generate(); 
            // }

            // petControllerComponent.shootInterval = ori_shootInterval / 2;
        }
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
        // playerControlerComponent.closeInvincible();
        OpenSelect();
        LoadImage(filePaths[0], selectImage1);
        LoadImage(filePaths[1], selectImage2);
        LoadImage(filePaths[2], selectImage3);
        Text2ImageSaver petBulletText2ImgSaverComponent = petBulletSaver.GetComponent<Text2ImageSaver>();
        petBullet_path1 = "/home/jh/zyh/unityproject/MiniGame01/Assets/StreamingAssets/SDImages/petBullet0.png";
        if (petBulletText2ImgSaverComponent != null)
            {
                petBulletText2ImgSaverComponent.Generate(); 
            }
    }

    void selectSth1()
    {
        if (File.Exists("/home/jh/zyh/unityproject/MiniGame01/Assets/StreamingAssets/SDImages/pet.png"))
            {
                File.Delete("/home/jh/zyh/unityproject/MiniGame01/Assets/StreamingAssets/SDImages/pet.png");
            }
        File.Copy(file_path1, "/home/jh/zyh/unityproject/MiniGame01/Assets/StreamingAssets/SDImages/pet.png");
        byte[] imageData = File.ReadAllBytes(file_path);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageData);
        texture.Apply();
        // Texture2D texture2D = selectImage2.texture as Texture2D;
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        spriteRenderer.sprite = sprite;
        CloseSelect();
        loadBullet();
        petControllerComponent.canShoot = true;
        petControllerComponent.shootInterval = ori_shootInterval / 2;
        playerRB.isKinematic = false;
    }

    void selectSth2()
    {
        if (File.Exists("/home/jh/zyh/unityproject/MiniGame01/Assets/StreamingAssets/SDImages/pet.png"))
            {
                File.Delete("/home/jh/zyh/unityproject/MiniGame01/Assets/StreamingAssets/SDImages/pet.png");
            }
        File.Copy(file_path2, "/home/jh/zyh/unityproject/MiniGame01/Assets/StreamingAssets/SDImages/pet.png");
        byte[] imageData = File.ReadAllBytes(file_path);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageData);
        texture.Apply();
        // Texture2D texture2D = selectImage2.texture as Texture2D;
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        spriteRenderer.sprite = sprite;
        CloseSelect();
        loadBullet();
        petControllerComponent.canShoot = true;
        petControllerComponent.shootInterval = ori_shootInterval / 2;
        playerRB.isKinematic = false;
    }

    void selectSth3()
    {
        if (File.Exists("/home/jh/zyh/unityproject/MiniGame01/Assets/StreamingAssets/SDImages/pet.png"))
            {
                File.Delete("/home/jh/zyh/unityproject/MiniGame01/Assets/StreamingAssets/SDImages/pet.png");
            }
        File.Copy(file_path3, "/home/jh/zyh/unityproject/MiniGame01/Assets/StreamingAssets/SDImages/pet.png");
        byte[] imageData = File.ReadAllBytes(file_path);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageData);
        texture.Apply();
        // Texture2D texture2D = selectImage2.texture as Texture2D;
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        spriteRenderer.sprite = sprite;
        CloseSelect();
        loadBullet();
        petControllerComponent.canShoot = true;
        petControllerComponent.shootInterval = ori_shootInterval / 2;
        playerRB.isKinematic = false;
    }

    void loadBullet(){
        if (File.Exists("/home/jh/zyh/unityproject/MiniGame01/Assets/StreamingAssets/SDImages/petBullet.png"))
            {
                File.Delete("/home/jh/zyh/unityproject/MiniGame01/Assets/StreamingAssets/SDImages/petBullet.png");
            }
        File.Copy(petBullet_path1, "/home/jh/zyh/unityproject/MiniGame01/Assets/StreamingAssets/SDImages/petBullet.png");
        byte[] imageData = File.ReadAllBytes(petBullet_path);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageData);
        texture.Apply();
        // Texture2D texture2D = selectImage2.texture as Texture2D;
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        petBulletSpriteRenderer.sprite = sprite;
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
