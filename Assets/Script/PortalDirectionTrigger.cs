using UnityEngine;
using UnityEngine.Events;


public class PortalDirectionTrigger : MonoBehaviour
{
    
   
    

    public ChangeLayer ChangeLayer;

    [Header("Events")]
    public UnityEvent OnEnterFront;
    public UnityEvent OnEnterBack;


    private void OnTriggerEnter(Collider other)
    {

        Debug.Log("Collided!撞了1!Check1: Collided with {other.name}, Layer: {other.gameObject.layer}, Enabled: {other.enabled}");


        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))

            {
            Vector3 dir = (other.transform.position - transform.position).normalized;
            float dot = Vector3.Dot(dir, transform.forward);
            Debug.Log($"撞了2!");

            if (dot > 0)
            {
                Debug.Log("dot > 0!人和portal方向反着，返回原世界");
                //OnEnterFront?.Invoke(); // call the event
                ChangeLayer.ChangeRendererLayerMask("StencilThisWorld", "WorldA");
                ChangeLayer.ChangeRendererLayerMask("StencilPortalWorld", "WorldB");
                Debug.Log($"撞了！: Collided with {other.name}, Layer: {other.gameObject.layer}, Enabled: {other.enabled}");
            }
            else if (dot < 0)
            {
                Debug.Log("dot<0!人和portal方向顺着，进入新世界");
                ChangeLayer.ChangeRendererLayerMask("StencilThisWorld", "WorldB");
                ChangeLayer.ChangeRendererLayerMask("StencilPortalWorld", "WorldA");
                //OnEnterBack?.Invoke(); // call the event
                Debug.Log($"撞了！: Collided with {other.name}, Layer: {other.gameObject.layer}, Enabled: {other.enabled}");
            }
        }
    }




}