using UnityEngine;

public class MonsterController : MonoBehaviour
{
    public float moveSpeed = 2f; // 怪物移动速度
    public float wanderRadius = 3f; // 游荡半径
    public float maxDistanceFromPlayer = 10f; // 怪物与玩家的最大距离

    private Vector2 wanderTarget;
    private Transform playerTransform;
    private SpriteRenderer spriteRenderer;

    private AudioSource bombAudioSource; // 死亡音效

    void Start()
    {
        wanderTarget = (Vector2)transform.position + Random.insideUnitCircle * wanderRadius;
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        spriteRenderer = GetComponent<SpriteRenderer>(); // 获取SpriteRenderer组件
        bombAudioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // 游荡行为
        Vector2 currentPosition = transform.position;
        transform.position = Vector2.MoveTowards(transform.position, wanderTarget, moveSpeed * Time.deltaTime);

        // 检查水平移动方向并进行翻转
        Vector2 direction = (Vector2)transform.position - currentPosition;
        if (direction.x < 0)
        {
            spriteRenderer.flipX = true; // 向左移动时翻转
        }
        else if (direction.x > 0)
        {
            spriteRenderer.flipX = false; // 向右移动时恢复
        }

        if (Vector2.Distance(transform.position, wanderTarget) < 0.1f)
        {
            wanderTarget = (Vector2)transform.position + Random.insideUnitCircle * wanderRadius;
        }

        // 检查与玩家的距离
        float distanceFromPlayer = Vector2.Distance(transform.position, playerTransform.position);
        if (distanceFromPlayer > maxDistanceFromPlayer)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            // if (bombAudioSource != null)
            // {
            //     bombAudioSource.Play();
            // }
            Destroy(collision.gameObject); // 销毁子弹
            Destroy(gameObject); // 销毁怪物
            ScoreManager.instance.score += 2;
        }
    }
}
