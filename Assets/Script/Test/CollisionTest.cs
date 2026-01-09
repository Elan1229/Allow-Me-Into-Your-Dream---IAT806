using UnityEngine;

public class CollisionTest : MonoBehaviour

{
  

    //Moves this GameObject 2 units a second in the forward direction
    void Update()
    {
       
    }

    //Upon collision with another GameObject, this GameObject will reverse direction
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided!");
        
    }
}


