using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameBase;
using Manager;

public class LogistPipe : Block
{
    int con_num = 0;    //连接数量
    bool[] findbuilding = new bool[4];//上右下左是否有建筑
    int[] step = { 0, 1, 0, -1, 1, 0, -1, 0 };//上右下左
    bool isstand = false;

    private void Start()
    {
        FindBuilding();
        ChooseMap();
        isstand = true;
    }

    private void FindBuilding()
    {
        Vector2Int pos = new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
        //Debug.Log(pos);
        for (int i = 0; i < 4; i++)
        {
            var block = MapManager.GetBlock(pos.x + step[i], pos.y + step[i + 4]);
            //Debug.Log(block);
            if (block != null && block.gameObject.tag == "Building")
            {
                findbuilding[i] = true;
                con_num++;
                if (!isstand && block is LogistPipe) ((LogistPipe)block).UpdateMap();
            }
        }
    }

    private void ChooseMap()
    {
        //加载con_num对应贴图
        //旋转
        Debug.Log(con_num);
        if (con_num == 0 || con_num == 4) return;
        if (con_num == 2 && findbuilding[0] != findbuilding[2])
        {
            if (findbuilding[1]) transform.eulerAngles = new Vector3(0, 0, 180);
            //if (findbuilding[1]) transform.Rotate(Vector3.forward, 180);
        }
        else
        {
            int cnt = 0;
            while (!findbuilding[cnt]) cnt++;
            transform.eulerAngles = new Vector3(0, 0, 90 * cnt);
            //transform.Rotate(Vector3.forward, 90 * cnt);
            //Debug.Log(cnt);
        }
    }

    private void UpdateMap()
    {
        con_num = 0;
        for (int i = 0; i < 4; i++) findbuilding[i] = false;
        FindBuilding();
        ChooseMap();
    }
}
