using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public float rotateSpeed = 100f;

    void Update()
    {
        transform.Rotate(Vector3.back, -rotateSpeed * Time.deltaTime);
    }
}
