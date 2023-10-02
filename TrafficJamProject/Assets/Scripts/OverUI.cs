using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OverUI : MonoBehaviour
{
    public bool overUI = false;

    private void Update()
    {
        overUI = EventSystem.current.IsPointerOverGameObject();
    }
}
