using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Joystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private RectTransform background;
    [SerializeField] private RectTransform handle;
    [SerializeField] private float deadZone = 0.1f;
    [SerializeField] private float range = 1f;

    private Vector2 input = Vector2.zero;
    private Canvas canvas;
    private Camera cam;

    public float Horizontal { get { return input.x; } }
    public float Vertical { get { return input.y; } }
    public Vector2 Direction { get { return new Vector2(Horizontal, Vertical); } }

    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
            Debug.LogError("The Joystick is not placed inside a canvas");

        if (background == null)
            background = transform as RectTransform;

        cam = canvas.worldCamera;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 position = RectTransformUtility.WorldToScreenPoint(cam, background.position);
        Vector2 radius = background.sizeDelta / 2;
        input = (eventData.position - position) / (radius * canvas.scaleFactor);
        
        // Clamp input vector to circle with radius 1
        if (input.magnitude > 1)
            input = input.normalized;

        // Move joystick handle
        handle.anchoredPosition = input * radius * range;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        input = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
    }
} 