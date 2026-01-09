using UnityEngine;
using System.Collections;

/// <summary>
/// 简单的握手检测和Portal生成
/// 挂载到场景中的管理对象上
/// </summary>
public class HandshakeManager : MonoBehaviour
{
    [Header("Hand")]
    public GameObject leftHand;
    public GameObject rightHand;
    [Header("Head")]
  
    public Transform userHead;

    [Header("Handshake")]
    public float handshakeDistance = 0.12f; // 12cm
    public float handshakeHoldTime = 3f;

    [Header("Portal")]
    public GameObject portalPrefab;
    public GameObject cube;
    public float portalHeightOffset = 0.2f; // 20cm
    public float portalGrowDuration = 30000f;
    public Vector3 portalFinalScale = Vector3.one;


    [Header("Debug")]
    public bool showDebugInfo = true;
    public bool showDebugGizmos = true;

 
    private OVRSkeleton leftSkeleton;
    private OVRSkeleton rightSkeleton;
    private bool isHandshaking = false;
    private float handshakeTimer = 0f;
    private GameObject currentPortal;
    private bool portalSpawned = false;
    private Coroutine growPortalCoroutine;


    void Update()
    {
        // 实时获取组件（如果还没有的话）
        if (leftSkeleton == null && leftHand != null)
        {
           
            leftSkeleton = leftHand.GetComponent<OVRSkeleton>();
            Debug.Log($"左手！");
        }

        if (rightSkeleton == null && rightHand != null)
        {
           
            rightSkeleton = rightHand.GetComponent<OVRSkeleton>();
            Debug.Log($"右手！");
        }

        // 检测握手
        CheckHandshake();

        // 检测重置按键
        CheckResetInput();
    }

    /// <summary>
    /// 获取手掌位置（从骨骼）
    /// </summary>
    Vector3 GetHandPalmPosition(OVRSkeleton skeleton)
    {
        if (skeleton == null || !skeleton.IsInitialized || skeleton.Bones == null)
            return Vector3.zero;

        // 查找指定的骨骼palm
        foreach (var bone in skeleton.Bones)
        {
            if (bone.Id == OVRSkeleton.BoneId.XRHand_Palm && bone.Transform != null)
            {
                return bone.Transform.position;

            }
        }

        return Vector3.zero;
    }

    void CheckHandshake()
    {
        // 检查手是否存在且被追踪
        if (leftSkeleton == null || rightSkeleton == null )
        {
            if (isHandshaking)
            {
                isHandshaking = false;
                handshakeTimer = 0f;
            }
            return;
        }

        Vector3 leftPos = Vector3.zero;
        Vector3 rightPos = Vector3.zero;

        if (leftSkeleton != null)
        {
            // 获取手掌位置（从骨骼）
             leftPos = GetHandPalmPosition(leftSkeleton);
            //Debug.Log($"左手手掌！");
        }
        if (leftSkeleton != null)
        {
            rightPos = GetHandPalmPosition(rightSkeleton);
            //Debug.Log($"右手手掌！");
        }

        // 如果位置无效，跳过
        if (leftPos == Vector3.zero || rightPos == Vector3.zero)
            return;

        // 计算距离
        float distance = Vector3.Distance(leftPos, rightPos);

        // 判断是否握手
        if (distance < handshakeDistance)
        {
            if (!isHandshaking)
            {
                isHandshaking = true;
                handshakeTimer = 0f;
                if (showDebugInfo)
                    Debug.Log($"握手开始 - 距离: {distance:F3}m");
            }

            handshakeTimer += Time.deltaTime;

            // 持续足够时间 → 生成Portal
            if (handshakeTimer >= handshakeHoldTime && !portalSpawned)
            {
                // 生成Portal
                SpawnPortal(leftPos, rightPos, userHead);
                portalSpawned = true;
                
                //Instantiate(cube, new Vector3(0, 1, 1), Quaternion.identity);
                Debug.Log($"生成Portal");
            }
        }
        else
        {
            if (isHandshaking)
            {
                isHandshaking = false;
                handshakeTimer = 0f;
            }
        }
    }

    void SpawnPortal(Vector3 leftPos, Vector3 rightPos, Transform userHead)
    {
        if (currentPortal != null || portalPrefab == null)
            return;

        // 计算Portal位置
        Vector3 midpoint = (leftPos + rightPos) / 2f;
        Vector3 portalPos = midpoint + Vector3.up * portalHeightOffset + Vector3.forward*0.12f;

        // 计算Portal朝向：从握手点指向用户头部
        Quaternion portalRot;

        if (userHead != null)
        {
            // 从握手点到头部的方向
            Vector3 toHead = userHead.position - portalPos;

            // 投影到XZ平面（水平方向）
            toHead.y = 0;

            if (toHead.magnitude > 0.01f)
            {
                // Portal facing the user - 不知道为啥就是方向反了，乘了个-1
                portalRot = Quaternion.LookRotation(toHead.normalized*-1, Vector3.up);
                Debug.Log("Portal面向用户");
            }
            else
            {
                // 如果头部正好在Portal上方，使用头部的forward
                Vector3 headForward = userHead.forward;
                headForward.y = 0;
                portalRot = Quaternion.LookRotation(headForward.normalized, Vector3.up);
                Debug.Log("头部正好在Portal上方");
            }
        }
        else
        {
            // 没有头部引用，使用默认朝向
            portalRot = Quaternion.identity;
            Debug.Log("没有找到用户头部，Portal使用默认朝向");
        }


        // 生成Portal
        currentPortal = Instantiate(portalPrefab, portalPos, portalRot);

        if (showDebugInfo)
            Debug.Log("Portal已生成！");

        //// 生长动画
        //StartCoroutine(GrowPortal(currentPortal));
        // ⭐修改：记录协程 handle
        growPortalCoroutine = StartCoroutine(GrowPortal(currentPortal));

    }




    IEnumerator GrowPortal(GameObject portal)
    {
        if (portal == null) yield break;

        portal.transform.localScale = Vector3.zero;
        float time = 0f;

        while (time < portalGrowDuration)
        {

            if (portal == null) yield break;  // ⭐修改：运行中被 Destroy 时停止
            time += Time.deltaTime;
            float t = time / portalGrowDuration;

            // EaseOutBack
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;
            float eased = 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);

            portal.transform.localScale = Vector3.Lerp(Vector3.zero, portalFinalScale, eased);
            yield return null;
        }

        if (portal != null)  // ⭐修改：最后保护
            portal.transform.localScale = portalFinalScale;
    }

    void CheckResetInput()
    {
        if (OVRInput.GetDown(OVRInput.Button.One)) // A键或X键
        {
            ResetPortal();
        }
    }

    public void ResetPortal()
    {
        // ⭐修改：停止协程，防止协程访问已被销毁的 portal
        if (growPortalCoroutine != null)
        {
            StopCoroutine(growPortalCoroutine);
            growPortalCoroutine = null;
        }


        if (currentPortal != null)
            Destroy(currentPortal);

        currentPortal = null;
        portalSpawned = false;
        isHandshaking = false;
        handshakeTimer = 0f;

        if (showDebugInfo)
            Debug.Log("Portal已重置");
    }

    //void OnDrawGizmos()
    //{
    //    if (!showDebugGizmos || !Application.isPlaying) return;


    //    Vector3 leftPos = GetHandPalmPosition(leftSkeleton);
    //    Vector3 rightPos = GetHandPalmPosition(rightSkeleton);

    //    if (leftPos == Vector3.zero || rightPos == Vector3.zero) return;

    //    // 手的位置
    //    Gizmos.color = Color.blue;
    //    Gizmos.DrawWireSphere(leftPos, 0.02f);
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(rightPos, 0.02f);

    //    // 连线
    //    Gizmos.color = isHandshaking ? Color.green : Color.gray;
    //    Gizmos.DrawLine(leftPos, rightPos);

    //    // Portal位置
    //    Vector3 midpoint = (leftPos + rightPos) / 2f;
    //    Vector3 portalPos = midpoint + Vector3.up * portalHeightOffset;
    //    //Gizmos.color = portalSpawned ? Color.cyan : Color.yellow;
    //    //Gizmos.DrawWireSphere(portalPos, 0.05f);
    //}
}