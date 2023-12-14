using UnityEngine;

public class CameraAspectRatio : MonoBehaviour
{
    public float targetAspectRatio = 9f / 16f; 

    void Start()
    {
        Camera camera = GetComponent<Camera>();

        if (camera != null)
        {
            float currentAspectRatio = (float)Screen.width / Screen.height;

            if (!Mathf.Approximately(currentAspectRatio, targetAspectRatio))
            {
                float scaleHeight = currentAspectRatio / targetAspectRatio;
                camera.fieldOfView *= scaleHeight;

                currentAspectRatio = (float)Screen.width / Screen.height;

                if (!Mathf.Approximately(currentAspectRatio, targetAspectRatio))
                {
                    float scaleWidth = 1.0f / scaleHeight;
                    camera.projectionMatrix = Matrix4x4.Scale(new Vector3(scaleWidth, 1.0f, 1.0f)) * camera.projectionMatrix;
                }
            }
        }
    }
}
