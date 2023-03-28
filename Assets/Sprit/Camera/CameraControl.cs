using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    Camera cam;
    //[SerializeField]
    private Vector2 pos_temp;
    //[SerializeField]
    private Vector3 mouse_pos_last;
    bool moving;

    private void Start()
    {
        cam = gameObject.GetComponent<Camera>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
        {
            moving = true;
            pos_temp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouse_pos_last = new Vector3(pos_temp.x, pos_temp.y, -10);
        }
        if (Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2))
        {
            moving = false;
        }
        if (moving)
        {
            pos_temp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            cam.transform.position = new Vector3(cam.transform.position.x - (pos_temp.x - mouse_pos_last.x), cam.transform.position.y - (pos_temp.y - mouse_pos_last.y), -10);

            pos_temp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouse_pos_last = new Vector3(pos_temp.x, pos_temp.y, -10);
        }
        //鼠标滚轮的效果
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (Camera.main.fieldOfView <= 100)
                Camera.main.fieldOfView += 2;
            if (Camera.main.orthographicSize <= 20)
                Camera.main.orthographicSize += 0.5F;
        }
        //Zoom in
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (Camera.main.fieldOfView > 2)
                Camera.main.fieldOfView -= 2;
            if (Camera.main.orthographicSize >= 1)
                Camera.main.orthographicSize -= 0.5F;
        }
    }
}