using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterGenerator : MonoBehaviour
{
    public GameObject monsterPrefab; // 怪物预制件
    public Transform playerTransform; // 玩家Transform
    public int numberOfMonsters = 5; // 生成怪物的数量
    public float spawnRadius = 5f; // 生成怪物的半径
    private int currentMonsterCount;

    void Start()
    {
        SpawnMonsters(numberOfMonsters);
    }

    void Update()
    {
        // 检查当前怪物数量，如果少于目标数量，则补足
        currentMonsterCount = GameObject.FindGameObjectsWithTag("Monster").Length;
        if (currentMonsterCount < numberOfMonsters)
        {
            SpawnMonsters(numberOfMonsters - currentMonsterCount);
        }
    }

    void SpawnMonsters(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector2 spawnPosition = playerTransform.position + (Vector3)(Random.insideUnitCircle * spawnRadius);
            GameObject monster = Instantiate(monsterPrefab, spawnPosition, Quaternion.identity);
            // monster.tag = "Monster"; // 设置怪物的标签
            monster.transform.parent = transform;
        }
    }
}