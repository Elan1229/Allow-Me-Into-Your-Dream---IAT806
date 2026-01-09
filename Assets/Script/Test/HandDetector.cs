using UnityEngine;

/// <summary>
/// 检测单个手的状态
/// 挂载到左手和右手追踪对象上
/// </summary>
public class HandDetector : MonoBehaviour
{
   
    public bool isLeftHand = false;

   
    public Transform palmCenter; // 手掌中心点Transform

    public bool showDebugGizmos = true;

    // 手掌朝向
    public Vector3 PalmNormal
    {
        get
        {
            // 右手：手掌法线是-right，左手：手掌法线是right
            return isLeftHand ? transform.right : -transform.right;
        }
    }

    // 手掌中心世界坐标
    public Vector3 PalmPosition
    {
        get { return palmCenter != null ? palmCenter.position : transform.position; }
    }

    // 手的前进方向（手指指向）
    public Vector3 HandForward
    {
        get { return transform.forward; }
    }

    void OnDrawGizmos()
    {
        if (!showDebugGizmos || palmCenter == null)
            return;

        // 绘制手掌位置
        Gizmos.color = isLeftHand ? Color.blue : Color.red;
        Gizmos.DrawWireSphere(PalmPosition, 0.03f);

        // 绘制手掌法线（手心朝向）
        Gizmos.color = Color.green;
        Gizmos.DrawRay(PalmPosition, PalmNormal * 0.1f);

        // 绘制手指方向
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(PalmPosition, HandForward * 0.1f);
    }
}