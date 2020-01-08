using UnityEngine;

public class CameraRotator : MonoBehaviour
{
    public float RotationsPerMinute = 1.0f;
    public void Update()
    {
        var yAngle = 6.0f * RotationsPerMinute * Time.deltaTime; //seconds since the last frame update
        transform.Rotate(0f, yAngle, 0f);
    }
}