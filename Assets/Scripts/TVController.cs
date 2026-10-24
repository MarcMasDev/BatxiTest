using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine;
public class TVController : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject tvShot;

    public UnityEvent onZoomIn;
    public UnityEvent onZoomOut;
    void Awake()
    {
        tvShot.SetActive(false);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        //Drag isn't a tap: ignore it if the pointer moved
        if ((eventData.position - eventData.pressPosition).magnitude > EventSystem.current.pixelDragThreshold) return;

        tvShot.SetActive(!tvShot.activeSelf);
        if (tvShot.activeSelf) onZoomIn.Invoke();
        else onZoomOut.Invoke();
    }
}