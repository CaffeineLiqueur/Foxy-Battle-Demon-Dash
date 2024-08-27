using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickupGenerator : MonoBehaviour
{
    public GameObject pickupPrefab; // Reference to the gem prefab
    public int numberOfPickups = 10; // Number of pickups to generate
    // public Vector2 mapSize = new Vector2(1024, 512); // Size of the map to generate pickups in
    public RawImage backgroundImage; // Reference to the background RawImage

    void Start()
    {
        GeneratePickups();
    }

    void GeneratePickups()
    {

        // 获取 RawImage 的 RectTransform
        RectTransform rectTransform = backgroundImage.GetComponent<RectTransform>();

         // 获取背景图片的宽度和高度
        float width = rectTransform.rect.width;
        float height = rectTransform.rect.height;

        // 获取背景图片的左下角位置（锚点位置）
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        Vector3 bottomLeft = corners[0];
        Vector3 center = rectTransform.position;

        for (int i = 0; i < numberOfPickups; i++)
        {
            // 在背景图片区域内生成随机位置
            // float randomX = Random.Range(0, width);
            // float randomY = Random.Range(0, height);
            // Vector3 generatePosition = bottomLeft + new Vector3(randomX, randomY, 0);
            float randomX = Random.Range(-width / 8, width / 8);    // 除以8是因为首先，canvas的scale是0.25，先除以4，然后是中心点，再除以2
            float randomY = Random.Range(-height / 8, height / 8);
            Vector3 generatePosition = center + new Vector3(randomX, randomY, 0);

            // 生成 Pickup
            Instantiate(pickupPrefab, generatePosition, Quaternion.identity, transform);
        }
    }
}