using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;       // ลากตัวละคร (Player) มาใส่ในช่องนี้
    public Vector3 offset = new Vector3(0, 0, -10); // ระยะห่างระหว่างกล้องกับตัวละคร (สำหรับ 2D)
    public float smoothSpeed = 5f; // ความเร็วในการเคลื่อนที่ตาม

    void LateUpdate()
    {
        if (target == null) return;

        // ตำแหน่งเป้าหมายที่กล้องต้องไป
        Vector3 desiredPosition = target.position + offset;

        // ค่อยๆ เคลื่อนกล้องไปยังตำแหน่งเป้าหมายแบบนุ่มนวล (Lerp)
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // อัปเดตตำแหน่งกล้อง
        transform.position = smoothedPosition;
    }
}
