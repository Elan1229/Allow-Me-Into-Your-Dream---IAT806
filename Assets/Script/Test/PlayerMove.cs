using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;       // 移动速度
    public float lookSensitivity = 2f; // 鼠标灵敏度
    public float verticalSpeed = 3f;   // 上下移动速度
    private float rotationX = 0f;      // 用于垂直旋转

    void Update()
    {
        // 获取键盘输入
        float horizontal = 0f;
        float vertical = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed) horizontal = -1f;
            if (Keyboard.current.dKey.isPressed) horizontal = 1f;
            if (Keyboard.current.wKey.isPressed) vertical = 1f;
            if (Keyboard.current.sKey.isPressed) vertical = -1f;
        }

        // 基础移动（前后左右）
        Vector3 move = transform.right * horizontal + transform.forward * vertical;

        // 上下移动（Q/E）
        if (Keyboard.current != null)
        {
            if (Keyboard.current.qKey.isPressed)
                move += Vector3.down * verticalSpeed;
            if (Keyboard.current.eKey.isPressed)
                move += Vector3.up * verticalSpeed;
        }

        transform.position += move * moveSpeed * Time.deltaTime;

        // 鼠标控制视角
        Vector2 mouseDelta = Vector2.zero;
        if (Mouse.current != null)
        {
            mouseDelta = Mouse.current.delta.ReadValue();
        }

        float mouseX = mouseDelta.x * lookSensitivity * 0.02f; // 乘以0.02f来匹配旧系统的灵敏度
        float mouseY = mouseDelta.y * lookSensitivity * 0.02f;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f); // 限制垂直角度

        transform.localRotation = Quaternion.Euler(rotationX, transform.localEulerAngles.y + mouseX, 0f);
    }
}