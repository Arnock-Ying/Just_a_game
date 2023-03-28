using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    Camera cam;
    [SerializeField]
    private Vector2 pos_temp;
    [SerializeField]
    private Vector3 mouse_pos_last;
    bool moving;

    private void Start()
    {
        cam = gameObject.GetComponent<Camera>();
    }
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            moving = true;
            pos_temp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouse_pos_last = new Vector3(pos_temp.x, pos_temp.y, -10);
        }
        if (Input.GetMouseButtonUp(1))
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
    }
}
