using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Construction : MonoBehaviour
{
    public GameObject myprefab;
    private bool building;
    private Vector2 temp;
    private Vector3 pos;
    private GameObject preview = null;
    private bool col_tri;

    void Start()
    {
        building = false;
        col_tri = false;
        //BoxCollider2D col = gameObject.GetComponent<BoxCollider2D>();
        //Instantiate(myprefab);
    }
    void Update()
    {
        if (building)
        {
            if (!preview) preview = GameObject.Instantiate(myprefab) as GameObject;
            temp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos = new Vector3(temp.x, temp.y, 0);
            preview.transform.position = pos;

            if (Input.GetMouseButtonDown(0)
                && !EventSystem.current.IsPointerOverGameObject()
                && !col_tri)
            {
                GameObject build = GameObject.Instantiate(myprefab, pos, transform.rotation) as GameObject;
            }
        }
        else
        {
            if (preview)
            {
                Destroy(preview);
                preview = null;
            }
        }
    }
    public void Click()
    {
        if (building)
        {
            building = false;
        }
        else
        {
            building = true;
        }
        //Debug.Log(building);
    }
    void OnTriggerEnter(Collider collider)
    {
        col_tri = true;
    }
    void OnTriggerExit(Collider collider)
    {
        col_tri = false;
    }
}
