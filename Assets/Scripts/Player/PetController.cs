using System.Collections;
using UnityEngine;

public class PetController : MonoBehaviour
{
    public Transform playerTransform; // 玩家Transform
    public float followSpeed = 2f; // 跟随速度
    public float followDistance = 1f; // 跟随的最小距离

    public GameObject bulletPrefab; // 子弹预制件
    public Transform firePoint; // 射击起点

    public float shootInterval = 2f; // 固定射击间隔
    public float bulletSpeed = 5f; // 子弹速度

    private SpriteRenderer spriteRenderer; // 宠物的 SpriteRenderer 组件
    private Vector3 originalFirePointPosition; // 记录firePoint的初始本地位置

    public bool canShoot = true;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); // 获取SpriteRenderer组件
        originalFirePointPosition = firePoint.localPosition; // 记录firePoint的初始本地位置
        StartCoroutine(AutoShoot()); // 启动自动射击协程
    }

    void Update()
    {
        // 计算宠物与玩家的距离
        float distance = Vector2.Distance(transform.position, playerTransform.position);

        // 如果距离大于跟随距离，则移动宠物靠近玩家
        if (distance > followDistance)
        {
            Vector2 currentPosition = transform.position;
            Vector2 direction = (playerTransform.position - transform.position).normalized;

            // 移动宠物靠近玩家
            transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, followSpeed * Time.deltaTime);

            // 检查水平移动方向并进行翻转
            Vector2 newPosition = transform.position;
            Vector2 moveDirection = newPosition - currentPosition;

            if (moveDirection.x < 0)
            {
                // 向左移动时翻转
                spriteRenderer.flipX = true;
                firePoint.localPosition = new Vector3(-originalFirePointPosition.x, originalFirePointPosition.y, originalFirePointPosition.z);
            }
            else if (moveDirection.x > 0)
            {
                // 向右移动时恢复
                spriteRenderer.flipX = false;
                firePoint.localPosition = originalFirePointPosition;
            }
        }
    }

    IEnumerator AutoShoot()
    {
        while (true)
        {
            // 固定等待时间
            yield return new WaitForSeconds(shootInterval);

            // 执行射击
            if (canShoot)
                Shoot();
        }
    }

    void Shoot()
    {
        // 实例化子弹
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // 设置子弹的速度，根据 firePoint 的方向调整
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        Vector2 shootDirection = spriteRenderer.flipX ? -firePoint.right : firePoint.right; // 根据翻转情况调整方向
        rb.velocity = shootDirection * bulletSpeed; // 使用调整后的方向设置子弹速度

         // 忽略子弹与主角的碰撞
        Collider2D bulletCollider = bullet.GetComponent<Collider2D>();
        Collider2D playerCollider = playerTransform.GetComponent<Collider2D>();
        Physics2D.IgnoreCollision(bulletCollider, playerCollider);
    }
}
