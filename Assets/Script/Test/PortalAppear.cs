
using UnityEngine;
using Oculus.Interaction;

public class PortalAppear : MonoBehaviour
{
    public GameObject portalWindow; // Assign your portal prefab in Inspector

    private void Awake()
    {
       
    }


    void OnTriggerEnter(Collider other)
    {
        
            portalWindow.SetActive(true);


        
    }
}