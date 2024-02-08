using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBackGround : MonoBehaviour
{

    private float length, startpos;
    [Header("Camera Settings")]
    [SerializeField] GameObject cam;
    [SerializeField] private float parallazEffect;

    private void Awake()
    {
        startpos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }


    // Update is called once per frame
    void Update()
    {
        float dist = (cam.transform.position.x * parallazEffect);
        transform.position = new Vector3(startpos + dist, transform.position.y, transform.position.z);
    }

}
