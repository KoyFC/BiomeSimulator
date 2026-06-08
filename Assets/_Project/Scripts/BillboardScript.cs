using UnityEngine;

public class BillboardScript : MonoBehaviour
{
    private Transform m_CameraTransform;
    private bool m_OnlyUpdateFirstFrame = true;

    private void Start()
    {
        m_CameraTransform = Camera.main.transform;
        if (m_OnlyUpdateFirstFrame)
        {
            LookAtCamera();
            enabled = false;
        }
    }

    private void LateUpdate()
    {
        LookAtCamera();
    }

    private void LookAtCamera()
    {
        if (m_CameraTransform == null) return;

        transform.LookAt(transform.position + m_CameraTransform.rotation * Vector3.forward, m_CameraTransform.rotation * Vector3.up);
    }
}