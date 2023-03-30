using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using GameBase;
using Manager;

///<summary>
///�����滮
///</summary>
public class BuildingProject : Block
{
    private Block projecting;
    private BoxCollider2D coll;
    private Rigidbody2D rb;
    private SpriteRenderer spr;

    Vector3 mouse_pos = new Vector3(0, 0, -999);
    private bool isholding;
    private bool collding;
    public void SetProject(GameObject prefab)
    {
        coll = GetComponent<BoxCollider2D>();
        spr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        if (!coll) coll = gameObject.AddComponent<BoxCollider2D>();
        if (!spr) spr = gameObject.AddComponent<SpriteRenderer>();
        if (!rb) rb = gameObject.AddComponent<Rigidbody2D>();
        projecting = prefab.GetComponent<Block>();
        if (!projecting) return;
        coll.size = prefab.GetComponent<BoxCollider2D>().size;
        coll.isTrigger = true;
        spr.sprite = prefab.GetComponent<SpriteRenderer>().sprite;
        transform.localScale = prefab.transform.localScale;

        rb.bodyType = RigidbodyType2D.Kinematic;

        size = projecting.size;
        spr.color = new Color(0.5f, 1, 0.5f, 0.5f);
        isholding = true;
        tag = "Building";
    }

    private void StandProject()
    {
        isholding = false;
        coll = GetComponent<BoxCollider2D>();
        spr = GetComponent<SpriteRenderer>();
        spr.color = new Color(0.5f, 0.5f, 1, 0.5f);
        coll.isTrigger = false;
    }

    private void Update()
    {
        if (isholding)
        {
            if (MapManager.GetBuild(transform.position, projecting.size))
            {
                spr.color = new Color(1, 0, 0, 0.5f);
                collding = true;
            }
            else
            {
                spr.color = new Color(0.5f, 1, 0.5f, 0.5f);
                collding = false;
            }
        }
        else if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(1))
            {
                mouse_pos = Input.mousePosition;
                //Debug.Log(mouse_pos);
            }
            if (Input.GetMouseButtonUp(1) && Input.mousePosition == mouse_pos)
            {
                //Debug.Log(mouse_pos);
                //Destroy(this);
            }
        }
    }

    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    if (isholding)
    //    {
    //        if (other.tag == "Building" || other.tag == "Mineral")
    //        {
    //            spr.color = new Color(1, 0, 0, 0.5f);
    //            collding = true;
    //        }
    //    }
    //}

    //private void OnTriggerStay2D(Collider2D collision)
    //{
    //    if (isholding)
    //    {
    //        if (collision.tag == "Building" || collision.tag == "Mineral")
    //        {
    //            spr.color = new Color(1, 0, 0, 0.5f);
    //            collding = true;
    //        }
    //    }
    //}

    //private void OnTriggerExit2D(Collider2D other)
    //{
    //    //Debug.Log(other.tag);
    //    if (isholding)
    //    {
    //        if (other.tag == "Building" || other.tag == "Mineral")
    //        {
    //            spr.color = new Color(0.5f, 1, 0.5f, 0.5f);
    //            collding = false;
    //        }
    //    }
    //}

    private void OnMouseUpAsButton()
    {
        if (!collding && isholding && !EventSystem.current.IsPointerOverGameObject())
        {
            var obj = Instantiate(gameObject);
            obj.transform.parent = transform.parent;
            obj.SetActive(true);
            obj.GetComponent<BuildingProject>().StandProject();
            obj.isStatic = true;
            MapManager.SetBuild(transform.position, projecting.size, projecting);
        }
    }
}


