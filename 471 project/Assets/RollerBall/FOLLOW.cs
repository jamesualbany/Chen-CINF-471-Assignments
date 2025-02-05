using UnityEngine;

public class FOLLOW : MonoBehaviour
{
    public Transform player; 
    public float rotationSpeed = 5f; 

    void Update()
    {
        transform.position = player.position + new Vector3(0, 1, -5);

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f)); 

        Vector3 direction = mousePosition - transform.position;

        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z)); 
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
