using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Construction : MonoBehaviour
{
    private Vector2 temp;
    private Vector3 pos;
    Button last_button = null;
    GameObject selected_prefab = null;

    void Start()
    {
        //building = false;
        //col_tri = false;
        //BoxCollider2D col = gameObject.GetComponent<BoxCollider2D>();
        //Instantiate(myprefab);
    }
    void Update()
    {
        if (!selected_prefab)
        {
            temp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos = new Vector3(temp.x, temp.y, 0);
        }

        //if (building)
        //{
        //    if (!preview) preview = GameObject.Instantiate(myprefab) as GameObject;
        //    temp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    pos = new Vector3(temp.x, temp.y, 0);
        //    preview.transform.position = pos;
        //    if (Input.GetMouseButtonDown(0)
        //        && !EventSystem.current.IsPointerOverGameObject()
        //        && !col_tri)
        //    {
        //        GameObject build = GameObject.Instantiate(myprefab, pos, transform.rotation) as GameObject;
        //    }
        //}
        //else
        //{
        //    if (preview)
        //    {
        //        Destroy(preview);
        //        preview = null;
        //    }
        //}
    }
    //public void MyClick(int a)
    //{
    //    if (building)
    //    {
    //        building = false;
    //    }
    //    else
    //    {
    //        building = true;
    //    }
    //    //Debug.Log(building);
    //}
    //void OnTriggerEnter(Collider collider)
    //{
    //    col_tri = true;
    //}
    //void OnTriggerExit(Collider collider)
    //{
    //    col_tri = false;
    //}

    public void ButtonAnimationKeep(Button button)
    {
        ColorBlock cb = button.colors;
        ColorBlock cb_choose = button.colors;
        cb.normalColor = new Color32(255, 255, 255, 255);
        cb.selectedColor = new Color32(255, 255, 255, 255);
        cb_choose.normalColor = new Color32(200, 200, 200, 255);
        cb_choose.selectedColor = new Color32(200, 200, 200, 255);
        if (!last_button)
        {
            button.colors = cb_choose;
            last_button = button;
        }
        else if (last_button == button)
        {
            button.colors = cb;
            last_button = null;
        }
        else
        {
            last_button.colors = cb;
            button.colors = cb_choose;
            last_button = button;
        }
        //Debug.Log(last_button);
    }

    public void SelectPrefab(GameObject prefab)
    {
        if (selected_prefab == prefab)
        {
            selected_prefab = null;
        }
        else
        {
            selected_prefab = prefab;
        }
    }
}
