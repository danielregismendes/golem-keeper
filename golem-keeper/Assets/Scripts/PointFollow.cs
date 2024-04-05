using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointFollow : MonoBehaviour
{
    public Transform follow;
    public float sensivity;


    private Vector3 offset;
    private float radius;
    private float mouseX;


    void Start()
    {

        offset = transform.position - follow.position;
        radius = offset.magnitude;

    }

    void Update()
    {
        
        transform.position = follow.position + offset;

        mouseX -= Input.GetAxis("Mouse X") * sensivity * Time.deltaTime;
        offset = new Vector3(Mathf.Cos(mouseX) * radius, offset.y, Mathf.Sin(mouseX) * radius);

    }
}
