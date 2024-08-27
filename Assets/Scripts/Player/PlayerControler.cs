using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerControler : MonoBehaviour
{
    public Rigidbody2D rb;
    public float xspeed;
    public float yspeed;
    // public int score = 0;
    // public 
    public int life = 10;
    public GameObject bulletPrefab; // 子弹Prefab
    public Transform firePoint; // 发射点
    public float bulletSpeed = 20f; // 子弹速度
    public float fireRate = 0.5f; // 射击频率，每秒射击一次
    private Vector2 direction = Vector2.right; // 初始方向向右
    public float maxDistance = 10f; // 子弹与玩家的最大距离
    public TextMeshProUGUI lifeText;
    private AudioSource shootingAudioSource; // 射击音效的 AudioSource 组件
    private AudioSource hurtAudioSource; // 受伤音效的 AudioSource 组件
    private AudioSource pickAudioSource; // 拣取音效的 AudioSource 组件


    // 定义方向枚举
    private enum Direction
    {
        Up,
        Down,
        Left,
        Right,
        UpLeft,
        UpRight,
        DownLeft,
        DownRight
    }



    // Start is called before the first frame update
    void Start()
    {
        AudioSource[] audioSources = GetComponents<AudioSource>(); // 获取所有AudioSource组件
        shootingAudioSource = audioSources[0]; // 第一个是射击音效
        hurtAudioSource = audioSources[1]; // 第二个是受伤音效
        pickAudioSource = audioSources[2]; // 第三个是拣取音效
    }

    void Update()
    {
        lifeText.text = "生命：" + life.ToString();
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Shoot();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        movement();
    }
    
    void openInvincible()
    {
        rb.isKinematic = true;
    }

    void closeInvincible()
    {
        rb.isKinematic = false;
    }

    void movement()
    {
        float faceto_x = 0;
        float faceto_y = 0;

        // 使用 GetKey 来处理输入
        if (Input.GetKey(KeyCode.RightArrow))
        {
            faceto_x = 1;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            faceto_x = -1;
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            faceto_y = 1;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            faceto_y = -1;
        }

        // 更新角色的速度
        rb.velocity = new Vector2(faceto_x * xspeed, faceto_y * yspeed);

        // 更新角色的方向
        if (faceto_x != 0 || faceto_y != 0)
        {
            if (faceto_x != 0){
                transform.localScale = new Vector3(faceto_x, 1, 0);
            }
            UpdateDirection(faceto_x, faceto_y);
        }
    }

    void UpdateDirection(float faceto_x, float faceto_y)
    {
        float offset = 0.5f; // 根据需要调整偏移量
        if (faceto_x > 0 && faceto_y > 0)
        {
            direction = new Vector2(1, 1).normalized;
            firePoint.localPosition = new Vector3(offset, offset, 0);
        }
        else if (faceto_x > 0 && faceto_y < 0)
        {
            direction = new Vector2(1, -1).normalized;
            firePoint.localPosition = new Vector3(offset, -offset, 0);
        }
        else if (faceto_x < 0 && faceto_y > 0)
        {
            direction = new Vector2(-1, 1).normalized;
            firePoint.localPosition = new Vector3(offset, offset, 0);
        }
        else if (faceto_x < 0 && faceto_y < 0)
        {
            direction = new Vector2(-1, -1).normalized;
            firePoint.localPosition = new Vector3(offset, -offset, 0);
        }
        else if (faceto_x > 0)
        {
            direction = Vector2.right;
            firePoint.localPosition = new Vector3(offset, 0, 0);
        }
        else if (faceto_x < 0)
        {
            direction = Vector2.left;
            firePoint.localPosition = new Vector3(offset, 0, 0);
        }
        else if (faceto_y > 0)
        {
            direction = Vector2.up;
            firePoint.localPosition = new Vector3(0, offset, 0);
        }
        else if (faceto_y < 0)
        {
            direction = Vector2.down;
            firePoint.localPosition = new Vector3(0, -offset, 0);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision){
        if (collision.tag == "Collection"){
            Destroy(collision.gameObject);
            // score ++;
            ScoreManager.instance.score ++;

            if (pickAudioSource != null)
            {
                pickAudioSource.Play();
            }
        }
        // if (collision.tag == "Monster")
        // {
        //     Debug.Log("Player hit by monster! Apply penalty.");
        //     life --;
        // }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Monster"))
        {
            life --;
            if (life <= 0)
            {
                SceneManager.LoadScene(2); //1和下面的0表示场景的编号
            }

            if (hurtAudioSource != null)
            {
                hurtAudioSource.Play();
            }

        }
    }

    void Shoot()
    {
        // 实例化子弹
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        
        // 设置子弹的速度
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.velocity = direction * bulletSpeed; // 使用方向设置子弹速度

        BulletController bulletController = bullet.AddComponent<BulletController>();
        // bulletController.maxDistance = 1f; // 设置子弹与玩家的最大距离

        // 播放射击音效
        if (shootingAudioSource != null)
        {
            shootingAudioSource.Play();
        }
    }
}
