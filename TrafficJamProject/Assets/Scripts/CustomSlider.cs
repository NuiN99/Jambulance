using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CustomSlider : MonoBehaviour
{
    RectTransform rectTransform;

    [SerializeField] RectTransform startPos;
    [SerializeField] RectTransform endPos;

    [SerializeField] RectTransform handle;

    public float currentValue;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void UpdateValue(float value)
    {
        currentValue = value;

        float dist = endPos.position.y - startPos.position.y;

        // Calculate the intermediate position based on currentValue
        float intermediateY = startPos.position.y + dist * currentValue;

        // Set the handle's position
        handle.position = new Vector3(rectTransform.position.x, intermediateY);
        handle.position = new Vector3(handle.position.x, Mathf.Clamp(handle.position.y, startPos.position.y, endPos.position.y));
    }
}
