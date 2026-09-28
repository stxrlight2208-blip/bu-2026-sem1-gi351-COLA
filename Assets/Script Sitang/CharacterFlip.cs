using UnityEngine;

public class CharacterFlip : MonoBehaviour
{
    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        // 1. แปลงตำแหน่งเมาส์จากหน้าจอ (Screen Space) เป็นตำแหน่งในเกม (World Space)
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // 2. เช็คว่าเมาส์อยู่ทางขวาหรือทางซ้ายของตัวละคร
        if (mouseWorldPos.x > transform.position.x)
        {
            // เมาส์อยู่ทางขวา -> หันหน้าไปทางขวา (Scale เป็นบวก)
            transform.localScale = new Vector3(
                Mathf.Abs(originalScale.x),
                originalScale.y,
                originalScale.z
            );
        }
        else if (mouseWorldPos.x < transform.position.x)
        {
            // เมาส์อยู่ทางซ้าย -> พลิกไปทางซ้าย (Scale เป็นลบ)
            transform.localScale = new Vector3(
                -Mathf.Abs(originalScale.x),
                originalScale.y,
                originalScale.z
            );
        }
    }
}
