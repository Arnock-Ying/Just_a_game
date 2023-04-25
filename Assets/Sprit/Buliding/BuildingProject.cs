using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using GameBase;
using Manager;
using Logist;

///<summary>
///建筑规划
///</summary>
public class BuildingProject : BaseBuilding
{
    //private GameObject prefab;
    private Block projecting;
    private BoxCollider2D coll;
    private Rigidbody2D rb;
    private SpriteRenderer spr;

    Vector3 mouse_pos = new Vector3(0, 0, -999);
    private bool isholding;
    //private bool collding;

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

    private void StandProject(Block pro)
    {
        projecting = pro;
        isholding = false;
        coll = GetComponent<BoxCollider2D>();
        spr = GetComponent<SpriteRenderer>();
        spr.color = new Color(0.5f, 0.5f, 1, 0.5f);
        coll.isTrigger = false;

        UpdateLoglist();
    }

    private void FinishBuild()
    {
        GameObject obj = Instantiate(projecting.gameObject);
        obj.transform.parent = this.transform.parent;
        obj.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, 0);
        obj.SetActive(true);
        MapManager.SetBuild(transform.position, projecting.size, obj.GetComponent<Block>());
        fin = false;
        UpdateLoglist();
        //处理物流
        //if (obj.GetComponent<Block>() is LogistPipe pipe)
        //{
        //    pipe.BuildPipe(true);
        //}
        //Debug.Log("建造完成");
        Destroy(this.gameObject);
    }

    bool fin = false;
    private void Update()
    {
        fin = projecting is LogistPipe;
        //Debug.Log(prefab);
        //建造完成
        if (!isholding && fin && projecting.gameObject)
        {
            FinishBuild();
        }
        if (isholding)
        {
            if (MapManager.GetBuild(transform.position, projecting.size))
            {
                spr.color = new Color(1, 0, 0, 0.5f);
                //collding = true;
            }
            else
            {
                spr.color = new Color(0.5f, 1, 0.5f, 0.5f);
                //collding = false;
                if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
                {
                    var obj = Instantiate(gameObject);
                    obj.transform.parent = transform.parent;
                    obj.SetActive(true);
                    var pro = obj.GetComponent<BuildingProject>();
                    MapManager.SetBuild(transform.position, projecting.size, pro);
                    pro.StandProject(projecting);
                    obj.isStatic = true;
                }
            }
            if (projecting is LogistPipe)
            {
                var pair = LogistPipe.GetImageAndAngles(transform.position);
                spr.sprite = pair.Key;
                transform.eulerAngles = new Vector3(0, 0, pair.Value);
            }
        }
        else if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(1))
            {
                mouse_pos = Input.mousePosition;
            }
            if (Input.GetMouseButtonUp(1) && (Input.mousePosition - mouse_pos).sqrMagnitude < 0.1f)
            {
                var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (pos.x > transform.position.x - (float)size.x / 2
                    && pos.x < transform.position.x + (float)size.x / 2
                    && pos.y > transform.position.y - (float)size.y / 2
                    && pos.y < transform.position.y + (float)size.y / 2)
                    DestroyBlock();
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

    //private void OnMouseUpAsButton()
    //{
    //    if (!collding && isholding && !EventSystem.current.IsPointerOverGameObject())
    //    {
    //        var obj = Instantiate(gameObject);
    //        obj.transform.parent = transform.parent;
    //        obj.SetActive(true);
    //        obj.GetComponent<BuildingProject>().StandProject(projecting);
    //        obj.isStatic = true;
    //        MapManager.SetBuild(transform.position, projecting.size, this);
    //    }
    //}
}


