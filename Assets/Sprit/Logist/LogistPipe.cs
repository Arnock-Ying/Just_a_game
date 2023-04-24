using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using GameBase;
using Manager;

namespace Logist
{
    public enum LogistCommand
    {
        Nulldate = -1,
        Update,
        Newdate,
        Outdate,
    }
    public class LogistPipe : Block
    {
        [SerializeField]
        SpriteRenderer spr;
        LogistNetBlock net;

        Router router = null;
        ushort[][] temp_rout = new ushort[4][], inter_rout = new ushort[4][];
        LogistCommand[] temp_com = new LogistCommand[4], inter_com = new LogistCommand[4];


        int con_num = 0;    //��������
        Block[] findbuilding = new Block[4];//���������Ƿ��н���

        private LogistNetBlock parentLogist = null;
        public override LogistNetBlock ParentLogist { get => parentLogist; set => parentLogist = value; }
        public Router Router { get => router; }

        private void Start()
        {
            for (int i = 0; i < 4; ++i)
            {
                temp_rout[i] = null;
                inter_rout[i] = null;
                temp_com[i] = LogistCommand.Nulldate;
                inter_com[i] = LogistCommand.Nulldate;
            }
            BuildPipe(true);
        }

        private void FixedUpdate()
        {
            bool flgchange = false;
            for (int i = 0; i < 4; ++i)
            {
                if (temp_com[i] != LogistCommand.Nulldate)
                {
                    relayRouteCommand(temp_com[i], (Dircation)i);
                    flgchange = true;
                }
                if (temp_rout[i] != null)
                {
                    relayRoute(temp_rout[i], (Dircation)i);
                    flgchange = true;
                }

            }
            SendRoute();
            for (int j = 0; j < 4; ++j)
            {
                if (flgchange && router != null)
                {
                    router.resend[j] = false;
                    router.comresend[j] = false;
                    router.command = LogistCommand.Nulldate;
                }
                temp_com[j] = inter_com[j];
                temp_rout[j] = inter_rout[j];
                inter_rout[j] = null;
                inter_com[j] = LogistCommand.Nulldate;

            }
            debug = (parentLogist == null ? "nullnet" :
                  $" LogistNetBlock = {(parentLogist.Inter == null ? "null" : parentLogist.Inter.building.name)} : {parentLogist.id}")
                 + $" LogistNet = {(parentLogist.ParentNet == null ? "null" : "Net id:" + parentLogist.ParentNet.id)}\n";

            if (router != null)
            {
                debug += $"router {parentLogist.ParentNet.MaxIpNum} {(router.newone ? "new one" : "")}:\n";
                for (byte i = 0; i < parentLogist.ParentNet.MaxIpNum; ++i)
                    debug += $"ip: {i}, dir: {router.Dir(i)}, len: {router.Len(i)}\n";
            }
        }

        public void setRelayRoute(ushort[] rout, Dircation dir)
        {
            inter_rout[(int)dir] = rout;
        }
        public void setRelayRouteCommand(LogistCommand command, Dircation dir)
        {
            inter_com[(int)dir] = command;
        }
        private void relayRoute(ushort[] rout, Dircation dir)
        {
            if (con_num <= 1) return;
            if (router == null)
            {
                for (int i = 0; i < 256; ++i)
                    if (rout[i] != 0) rout[i] += 4;
                for (int i = 0; i < 4; ++i)
                    if ((int)dir != (i ^ 1) && findbuilding[i] != null)
                    {
                        if (findbuilding[i] is LogistPipe pipe)
                        {
                            Debug.Log(transform.position + "pipe with null nouter to" + (Dircation)i);
                            pipe.setRelayRoute(rout, (Dircation)(i));
                        }
                        else if (findbuilding[i] is InterFace inter) inter.UpdateIp(rout);
                    }
            }
            else
            {
                Debug.Log(transform.position + "get router form " + (Dircation)dir);
                router.ChangeRoute(rout, dir);
            }
        }

        private void relayRouteCommand(LogistCommand command, Dircation dir)
        {
            if (con_num <= 1) return;
            if (router == null)
            {
                for (int i = 0; i < 4; ++i)
                    if ((int)dir != (i ^ 1) && findbuilding[i] != null)
                    {
                        if (findbuilding[i] is LogistPipe pipe)
                        {
                            Debug.Log($"{transform.position} pipe with null nouter to {(Dircation)i} with command {command}");
                            pipe.setRelayRouteCommand(command, (Dircation)(i));
                        }
                        else if (findbuilding[i] is InterFace inter)
                        {
                            Debug.Log($"{transform.position}pipe with null nouter to {(Dircation)i} interface with command {command}");
                            inter.GetCommand(command);
                        }
                    }
            }
            else
            {
                router.GetCommand(command, dir);
                Debug.Log($"{transform.position}nouter get command {command} from {(Dircation)((int)dir ^ 1)}");
            }
        }

        private void SendRoute()
        {
            if (router == null) return;

            for (int i = 0; i < 4; ++i)
            {
                if (findbuilding[i] != null && router.comresend[i])
                {
                    if (findbuilding[i] is LogistPipe pipe)
                    {
                        Debug.Log(transform.position + $"nouter send command {router.command} to " + (Dircation)i);
                        pipe.setRelayRouteCommand(router.command, (Dircation)(i));
                    }
                    else if (findbuilding[i] is InterFace inter) inter.GetCommand(router.command);
                }
            }
            for (int i = 0; i < 4; ++i)
            {
                if (findbuilding[i] != null && router.resend[i])
                {
                    if (findbuilding[i] is LogistPipe pipe)
                    {
                        Debug.Log(transform.position + "nouter send table to " + (Dircation)i);
                        pipe.setRelayRoute(router.CopyIpTable(), (Dircation)(i));
                    }
                    else if (findbuilding[i] is InterFace inter) inter.UpdateIp(router.CopyIpTable());
                }
            }

        }

        public void BuildPipe(bool rebuild, bool relogist = false)
        {
            Vector2Int pos = new(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
            //Debug.Log(pos);
            con_num = 0;
            for (int i = 0; i < 4; i++)
            {
                int[] step = { 0, 0, -1, 1, 1, -1, 0, 0 };
                var block = MapManager.GetBlock(pos.x + step[i], pos.y + step[i + 4]);
                //Debug.Log(block);

                if (block != null)
                {
                    if (block is BaseBuilding building)
                    {
                        con_num++;
                        if (findbuilding[i] is not InterFace nowinter || nowinter.building != building)
                        {
                            InterFace inter = Instantiate(Resources.Load<GameObject>("Pipe/InterFace")).GetComponent<InterFace>();
                            inter.gameObject.transform.position = new Vector3(transform.position.x + step[i] * 0.5f, transform.position.y + step[i + 4] * 0.5f, -1.5f);
                            building.AddInterFace(inter);
                            inter.pipe = this;
                            inter.dir = (Dircation)(i ^ 1);
                            findbuilding[i] = inter;
                        }
                    }
                    else if (block is LogistPipe pipe)
                    {
                        if (findbuilding[i] is InterFace inter)
                            inter.DestroyBlock();

                        con_num++;
                        findbuilding[i] = pipe;

                        if (rebuild)
                            pipe.BuildPipe(false);
                    }
                }
                else
                {
                    if (findbuilding[i] is InterFace inter)
                    {
                        inter.DestroyBlock();
                    }
                    findbuilding[i] = null;
                }
            }
            //foreach (var i in findbuilding)
            //	Debug.Log(i);

            if (!(con_num == 2 && !(findbuilding[0] ^ findbuilding[1])) && con_num >= 2)
            {
                if (router == null)
                {
                    router = new(this);
                    for (int i = 0; i < 4; ++i)
                    {
                        if (findbuilding[i] is LogistPipe pipe)
                        {
                            pipe.setRelayRouteCommand(LogistCommand.Newdate, (Dircation)i);
                        }
                        if(findbuilding[i] is InterFace inter)
                        {
                            inter.GetCommand(LogistCommand.Newdate);
                        }
                    }
                }
            }
            else
            {
                router = null;
                GC.Collect();//�ֶ��������������ͷ�һ��
            }
            ChooseImage();

            if (rebuild)
            {
                //if (parentLogist == null) parentLogist = new LogistNetBlcok();
                //��������������Ϣ
                UpdateParentLogist();
                if (relogist)
                {
                    for (int i = 0; i < 4; ++i)
                        if (findbuilding[i] is LogistPipe pipe)
                        {
                            pipe.UpdateParentLogist();
                        }
                }
            }

        }

        ///<summary>
        ///��������������Ϣ
        ///</summary>
        public void UpdateParentLogist()
        {
            //�����ĸ�����
            int[] counts = new int[4];
            for (int i = 0; i < 4; ++i)
            {
                if (findbuilding[i] == null) counts[i] = -1;
                else if (findbuilding[i] is LogistPipe pipe)
                    counts[i] = pipe.ParentLogist.Count;
                else if (findbuilding[i] is InterFace inter)
                    counts[i] = 0;
            }

            //Ѱ�ҽ�С�������
            int min = 0;
            int minnum = -1;
            for (int i = 0; i < 4; ++i)
            {
                if (minnum == -1 || (minnum > counts[i] && counts[i] >= 0))
                {
                    min = i;
                    minnum = counts[i];
                }
            }

            //���룬�������û���򴴽�
            if (minnum == -1) this.parentLogist = new();
            else if (findbuilding[min] is InterFace inter)
            {
                this.parentLogist = new();
                parentLogist.Inter = inter;
                inter.ParentLogist = this.ParentLogist;
                if (inter.building is LogistCentral)
                {
                    parentLogist.ParentNet.SetManager(inter);
                }
                else
                {
                    parentLogist.ParentNet.SetIp(this.ParentLogist);
                }
            }
            else
            {
                parentLogist = findbuilding[min].ParentLogist;
            }
            parentLogist.Add(this);

            //�ϲ��������
            if (parentLogist.Inter == null)
            {
                //���ҽ�С�Ŀ������
                for (int i = 0; i < 4; ++i)
                {
                    if (findbuilding[i] == null) counts[i] = -2;
                    else if (findbuilding[i] is LogistPipe pipe)
                        counts[i] = (pipe.ParentLogist.Inter == null ? -1 : pipe.ParentLogist.Count);
                    else if (findbuilding[i] is InterFace)
                        counts[i] = 0;
                }
                int min2 = 0;
                int minnum2 = -2;
                for (int i = 0; i < 4; ++i)
                {
                    if (minnum2 < 0 || (minnum2 > counts[i] && counts[i] >= 0))
                    {
                        min2 = i;
                        minnum2 = counts[i];
                    }
                }
                //�ϲ�
                if (minnum2 != -2)
                {
                    findbuilding[min2].ParentLogist.Marge(this.ParentLogist);
                }
            }


            //������Χ������飬�ϲ���������������
            for (int i = 0; i < 4; ++i)
            {
                if (findbuilding[i] is LogistPipe pipe)
                {
                    if (pipe.ParentLogist.Inter == null)
                        ParentLogist.Marge(pipe.ParentLogist);
                    else
                    //�ϲ����ǿ���������
                    //��Χ����ǿգ�����ͬ�������磬�ϲ������粻��������������������ֳ�ͻ
                    //��ʵӦ��д��LogistNet�ľ�̬������QAQ
                    if ((ParentLogist.ParentNet != pipe.ParentLogist.ParentNet)
                        && LogistNet.BuildSum(ParentLogist.ParentNet, pipe.ParentLogist.ParentNet)
                            <= Math.Max(ParentLogist.ParentNet.MaxIpNum, pipe.ParentLogist.ParentNet.MaxIpNum)
                        && !((ParentLogist.ParentNet.Manager is not null && ParentLogist.ParentNet.Manager.building is LogistCentral)
                            && (pipe.ParentLogist.ParentNet.Manager is not null && pipe.ParentLogist.ParentNet.Manager.building is LogistCentral)))

                    {
                        if (ParentLogist.ParentNet.MaxIpNum > pipe.ParentLogist.ParentNet.MaxIpNum)
                            ParentLogist.ParentNet.Marge(pipe.ParentLogist.ParentNet);
                        else
                            pipe.ParentLogist.ParentNet.Marge(ParentLogist.ParentNet);
                    }
                }
            }

            //�ӿڴ���
            for (int i = 0; i < 4; ++i)
            {
                if (findbuilding[i] is InterFace inter)
                    if (this.ParentLogist.Inter != inter)
                        if (inter.ParentLogist != null)
                            ParentLogist.ParentNet.SetIp(inter.ParentLogist);
                        else
                        {
                            inter.ParentLogist = new();
                            inter.ParentLogist.ParentNet = this.ParentLogist.ParentNet;
                            this.ParentLogist.ParentNet.Blocks.Add(inter.ParentLogist);
                            this.ParentLogist.ParentNet.SetIp(inter.ParentLogist);
                        }

            }

            //�ͷźϲ��������������ռ�
            GC.Collect();
            return;
        }

        public static KeyValuePair<Sprite, float> GetImageAndAngles(Vector3 position)
        {
            Vector2Int pos = new(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.y));
            //Debug.Log(pos);
            int con_num = 0;
            bool[] findbuilding = new bool[4];
            int[] step = { -1, 1, 0, 0, 0, 0, 1, -1 };
            for (int i = 0; i < 4; i++)
            {
                findbuilding[i] = false;


                var block = MapManager.GetBlock(pos.x + step[i], pos.y + step[i + 4]);
                if (block is BaseBuilding || block is LogistPipe)
                {
                    ++con_num;
                    findbuilding[i] = true;
                }
            }
            Sprite sprite;
            //����con_num��Ӧ��ͼ
            if (con_num == 2 && !(findbuilding[0] ^ findbuilding[1]))
                sprite = LogistManager.PipeImage[5];
            else
                sprite = LogistManager.PipeImage[con_num];

            //��ת
            //Debug.Log(cnt + "--(" + und[i] + "," + und[(i + 1) % 4] + ")" + (findbuilding[und[i]] != null) + ":" + (findbuilding[und[(i + 1) % 4]] != null));
            float ans = 0;
            if (con_num == 0 || con_num == 4) ans = 0;
            else if (con_num == 2 && !(findbuilding[0] ^ findbuilding[1]))
            {
                if (findbuilding[2]) ans = 90;
            }
            else
            {
                int[] und = { 2, 0, 3, 1 };
                int cnt = 0;
                for (int i = 0; (findbuilding[und[i]]) || !(findbuilding[und[(i + 1) % 4]]); ++i) { if (i > 4) Debug.Log(i); ++cnt; }
                if (con_num == 1) ans = 90 * (cnt);
                else if (con_num == 2) ans = 90 * (cnt + 1);
                else if (con_num == 3) ans = 90 * (cnt + 2);
            }
            return new KeyValuePair<Sprite, float>(sprite, ans);
        }

        private void ChooseImage()
        {
            var pair = GetImageAndAngles(this.transform.position);
            spr.sprite = pair.Key;
            transform.eulerAngles = new Vector3(0, 0, pair.Value);
        }
    }
}