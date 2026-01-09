using UnityEngine;
using UnityEngine.XR;
using System.Collections;
using UnityEditor.Experimental.GraphView;

public class EnterWorldByController : MonoBehaviour
{
  


    public GameObject stencilBallPrefab; // 拖入你的 prefab
    public Transform head;               // 拖入你的 head 物体
    public float scaleMultiplier = 20f;  // 放大倍数
    
    public float scaleDuration = 2f;     // 缩放动画时间
    
    private GameObject currentBall = null;

    void Update()
    {
        // 检测 A 键 
        if (OVRInput.GetDown(OVRInput.Button.One))// A键或X键
        {
            Debug.Log("One!!!!!!!!!!!!");
            // if (currentBall == null)
            //{
            //SpawnBall();
            //}
            currentBall = Instantiate(stencilBallPrefab, head.position, Quaternion.identity);
            currentBall.transform.localScale = Vector3.one * scaleMultiplier;

        }

        // 检测 B 键 
        //if (OVRInput.GetDown(OVRInput.Button.Two)) 
        //{
        //    if (currentBall != null)
        //    {
        //        Destroy(currentBall);
        //    }
        //}
    }

    void SpawnBall()
    {
        currentBall = Instantiate(stencilBallPrefab, head.position, Quaternion.identity);
        currentBall.transform.localScale = Vector3.zero;
        StartCoroutine(ScaleUp(currentBall));
        Debug.Log("Stencil Created");
    }

    IEnumerator ScaleUp(GameObject ball)
    {
        if (ball == null) yield break;

        ball.transform.localScale = Vector3.zero;
        float time = 0f;

        Vector3 FinalScale = Vector3.one * scaleMultiplier;

        while (time < scaleDuration)
        {

            if (ball == null) yield break;  // ⭐修改：运行中被 Destroy 时停止
            time += Time.deltaTime;
            float t = time / scaleDuration;

            // EaseOutBack
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;
            float eased = 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);

            ball.transform.localScale = Vector3.Lerp(Vector3.zero, FinalScale, eased);
            yield return null;
        }

        if (ball != null)  // ⭐修改：最后保护
            ball.transform.localScale = FinalScale;
    }

}
