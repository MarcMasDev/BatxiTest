using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//Hace que un botón se sienta como un botón: se aplasta al pulsar y rebota como un muelle al soltar
public class ButtonFeel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Button button;

    [SerializeField] private float pressedScale = 0.9f;
    [SerializeField] private float stiffness = 800f;
    [SerializeField] private float damping = 12f;
    [SerializeField] private float squash = 0.5f;

    private Vector3 baseScale;
    private float size = 1f; //Tamaño actual: 1 = normal
    private float speed;
    private float target = 1f;

    private void Awake()
    {
        if (button == null) button = GetComponent<Button>();
        baseScale = transform.localScale;
    }

    private void OnDisable()
    {
        //Si se oculta a medio pulsar, vuelve a su tamaño
        size = 1f;
        speed = 0f;
        target = 1f;
        transform.localScale = baseScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!button.IsInteractable()) return; //Los botones desactivados no reaccionan
        target = pressedScale;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        target = 1f;
    }

    //Un empujón al muelle: rebota sin haberlo pulsado (por ejemplo, al acertar)
    public void Punch(float strength)
    {
        speed += strength;
    }

    private void Update()
    {
        if (size == target && speed == 0f) return;

        float dt = Mathf.Min(Time.unscaledDeltaTime, 1f / 30f);
        speed += (target - size) * stiffness * dt;
        speed -= speed * damping * dt;
        size += speed * dt;

        if (Mathf.Abs(target - size) < 0.001f && Mathf.Abs(speed) < 0.01f)
        {
            size = target;
            speed = 0f;
        }

        float width = 1f + (1f - size) * squash;
        transform.localScale = new Vector3(baseScale.x * width, baseScale.y * size, baseScale.z);
    }
}
