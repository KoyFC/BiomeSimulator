using UnityEngine;

public class BillboardScript : MonoBehaviour
{
    private Transform m_CameraTransform;

    private void Start()
    {
        m_CameraTransform = Camera.main.transform;
    }

    private void LateUpdate()
    {
        if (m_CameraTransform == null) return;

        transform.LookAt(transform.position + m_CameraTransform.rotation * Vector3.forward, m_CameraTransform.rotation * Vector3.up);
    }
}