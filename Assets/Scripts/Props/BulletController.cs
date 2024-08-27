using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float maxDistance = 5f; // 子弹与玩家的最大距离
    public float lifetime = 3f; // 子弹的最大生存时间
    private Transform playerTransform; // 玩家Transform
    private float timeElapsed = 0f; // 计时器

    void Start()
    {
        // 获取玩家的Transform
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        // 计算子弹与玩家的距离
        float distance = Vector2.Distance(transform.position, playerTransform.position);

        // 如果距离超过最大距离，销毁子弹
        if (distance > maxDistance)
        {
            Destroy(gameObject);
        }

        // 计时器更新
        timeElapsed += Time.deltaTime;

        // 如果生存时间超过设定的最大时间，销毁子弹
        if (timeElapsed >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}
