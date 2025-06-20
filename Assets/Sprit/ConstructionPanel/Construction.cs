using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using GameBase;

public class Construction : MonoBehaviour
{
    private Vector2 temp;
    private Vector3 pos;
    private Vector3 mouse_pos;
    Button last_button = null;
    GameObject selected_prefab = null;
    GameObject now_holding = null;
    Block selected_block = null;

    void Update()
    {
        if (selected_prefab)
        {
            temp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            bool a = (selected_block.size.x & 1) == 0, b = (selected_block.size.y & 1) == 0;
            pos = new Vector3(Mathf.Floor(temp.x + (a ? 0.5f : 0)) + (!a ? 0.5f : 0), Mathf.Floor(temp.y + (b ? 0.5f : 0)) + (!b ? 0.5f : 0), -1);
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
        selected_block = selected_prefab.GetComponent<Block>();
        if (!now_holding)
        {
            GameObject pro = new("project");
            pro.AddComponent<BuildingProject>().SetProject(selected_prefab);
            now_holding = pro;
        }
        else
        {
            now_holding.GetComponent<BuildingProject>().SetProject(selected_prefab);
        }
    }
}
