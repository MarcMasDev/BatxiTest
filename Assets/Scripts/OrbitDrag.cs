using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class OrbitDrag : MonoBehaviour
{
    [SerializeField] private CinemachineOrbitalFollow orbit;
    [SerializeField] private float speed = 0.3f;
    [SerializeField] private float friction = 4f;

    private bool dragging;
    private float velocity;

    void Update()
    {
        Pointer pointer = Pointer.current;
        if (pointer == null) return;

        if (pointer.press.wasPressedThisFrame)
        {
            dragging = true;
            velocity = 0f;
        }
        if (pointer.press.wasReleasedThisFrame)
        {
            dragging = false;
        }


        if (dragging)
        {
            float step = pointer.delta.ReadValue().x * speed;
            orbit.HorizontalAxis.Value += step;
            velocity = Mathf.Lerp(velocity, step / Time.deltaTime, 10f * Time.deltaTime);
        }
        else
        {
            velocity = Mathf.Lerp(velocity, 0f, friction * Time.deltaTime);
            orbit.HorizontalAxis.Value += velocity * Time.deltaTime;
        }
    }

    public void ResetRotation()
    {
        velocity = 0f;
        orbit.HorizontalAxis.TriggerRecentering();
    }
}