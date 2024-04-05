using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{

    public Transform follow;
    public Transform lookAt;
    public float smootness;


    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {

        transform.position = Vector3.Lerp(transform.position, follow.position, smootness * Time.deltaTime);
        transform.LookAt(lookAt);

    }
}
