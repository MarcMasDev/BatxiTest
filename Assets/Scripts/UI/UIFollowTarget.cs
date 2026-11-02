using UnityEngine;


[DefaultExecutionOrder(100)] //Se ejecuta después de Cinemachine (que mueve la cámara en LateUpdate).
public class UIFollowTarget : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 2f, 0f);

    [SerializeField] private Camera cam;

    private void Start()
    {
        if (cam == null) cam = Camera.main;
    }

    private void LateUpdate()
    {
        Vector3 screenPoint = cam.WorldToScreenPoint(target.position + offset);
        transform.position = new Vector3(screenPoint.x, screenPoint.y, 0f);
    }
}
