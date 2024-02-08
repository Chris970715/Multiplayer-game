using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCursor : MonoBehaviour
{
    [SerializeField] private Camera localCam;
    
    private void Start()
    {
        Cursor.visible = false;
    }

    private void Update()
    {
        Vector2 pos = localCam.ScreenToWorldPoint(Input.mousePosition);
        transform.position = pos;
    }
}
