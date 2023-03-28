using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Construction : MonoBehaviour
{
    //[SerializeField]
    private Vector2 temp;
    //[SerializeField]
    private Vector3 pos;
    private Vector3 mouse_pos;
    //[SerializeField]
    Button last_button = null;
    //[SerializeField]
    GameObject selected_prefab = null;
    //[SerializeField]
    GameObject now_holding = null;

    void Start()
    {
    }
    void Update()
    {
        if (selected_prefab)
        {
            temp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos = new Vector3(Mathf.Floor(temp.x + 0.5f), Mathf.Floor(temp.y + 0.5f), -1);
            now_holding.transform.position = pos;
            if (Input.GetMouseButtonDown(1))
            {
                mouse_pos = Input.mousePosition;
            }
            if (Input.GetMouseButtonUp(1) && Input.mousePosition == mouse_pos)
            {
                ColorBlock cbtemp = last_button.colors;
                cbtemp.normalColor = new Color32(255, 255, 255, 255);
                cbtemp.selectedColor = new Color32(255, 255, 255, 255);
                last_button.colors = cbtemp;
                last_button = null;
                Destroy(now_holding);
                selected_prefab = null;
            }
        }
    }

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
            if (now_holding)
            {
                Destroy(now_holding);
                now_holding = null;
            }
        }
        else
        {
            selected_prefab = prefab;
        }
        if (!selected_prefab) return;

        if (!now_holding)
        {
            GameObject pro = new GameObject("project");
            pro.AddComponent<BuildingProject>().SetProject(prefab);
            now_holding = pro;
        }
        else
        {
            now_holding.GetComponent<BuildingProject>().SetProject(prefab);
        }
    }
}
