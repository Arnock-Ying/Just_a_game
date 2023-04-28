using System.Collections;
using UnityEngine;
using System.Collections.Generic;

namespace Logist
{
    public enum Dircation : byte
    {
        Up,
        Down,
        Left,
        Right,
    }

    public class Router
    {

        private ushort[] vs = new ushort[256];
        public readonly LogistPipe pipe;
        public bool[] resend = new bool[4];
        public bool[] comresend = new bool[4];
        public bool newone = true;
        public LogistCommand command = LogistCommand.Nulldate;

        public ushort[] CopyIpTable()
        {
            ushort[] uvs = (ushort[])vs.Clone();
            for (int i = 0; i < 256; ++i)
                if (uvs[i] != 0) uvs[i] += 4;
            return uvs;
        }

        public int Len(byte ip)
        {
            return vs[ip] >> 2;
        }
        public Dircation Dir(byte ip)
        {
            return (Dircation)(vs[ip] & 3);
        }

        public void Set(byte ip, Dircation dir, int len)
        {
            vs[ip] = (ushort)((len << 2) | ((byte)dir & 3));
        }
        public Router(LogistPipe pipe)
        {
            this.pipe = pipe;
            for (int i = 0; i < 255; ++i) vs[i] = 0;
        }

        public bool ChangeRoute(int ip, Dircation dir, int len)
        {
            dir = (Dircation)((int)dir ^ 1);
            if (ip > 255 || ip < 0) return false;
            if (len == 0)
                Set((byte)ip, 0, 0);
            else if (len < 0 || Len((byte)ip) <= len) return false;

            else Set((byte)ip, dir, len);
            return true;
        }

        public void ChangeRoute(ushort[] ipTable, Dircation _dir)
        {
            Debug.Log($"{(pipe == null ? "interface" : pipe.transform.position)},{_dir}\n" + ToString(ipTable));
            bool change = true;
            bool rechange = true;
            int dir = (int)_dir ^ 1;
            for (int i = 0; i < 256; ++i)
            {
                if ((ipTable[i] >> 2) == 0 && vs[i] != 0 && (vs[i] & 3) == dir)
                {
                    vs[i] = 0;
                    rechange = false;
                }
                if ((((ipTable[i] >> 2) < (vs[i] >> 2)) || (vs[i] >> 2) == 0 || (vs[i] & 3) == dir )
                    && (ipTable[i] >> 2) != 0)
                {
                    vs[i] = (ushort)(((ipTable[i] >> 2) << 2) | dir);
                    Debug.Log($"pi:{i},dir:{Dir((byte)i)},len:{Len((byte)i)}");
                    change = false;
                }
            }
            if (command == LogistCommand.Update && change)
            {
                for (int i = 0; i < 4; ++i)
                {
                    resend[i] = false;
                    comresend[i] = false;
                }
            }
            else if (command == LogistCommand.Outdate && rechange)
            {
                for (int i = 0; i < 4; ++i)
                {
                    resend[i] = !resend[i];
                    comresend[i] = !comresend[i];
                }
                command = LogistCommand.Update;
            }
            return;
        }

        public void GetCommand(LogistCommand com, Dircation dir)
        {
            command = com;
            switch (com)
            {
                case LogistCommand.Update:
                    for (int i = 0; i < 4; ++i)
                    {
                        if ((i ^ 1) != (int)dir)
                        {
                            //未修改，设false
                            resend[i] = true;
                            comresend[i] = true;
                        }
                        else
                        {
                            resend[i] = false;
                            comresend[i] = false;
                        }
                    }
                    if (newone) newone = false;
                    break;
                case LogistCommand.Newdate:
                    for (int i = 0; i < 4; ++i)
                    {
                        if (newone)
                        {
                            resend[i] = false;
                            comresend[i] = false;
                        }
                        else
                        {
                            if ((i ^ 1) != (int)dir)
                            {
                                resend[i] = false;
                                comresend[i] = false;
                            }
                            else
                            {
                                resend[i] = true;
                                comresend[i] = true;
                            }
                            command = LogistCommand.Update;
                        }
                    }
                    break;
                case LogistCommand.Outdate:
                    for (int i = 0; i < 4; ++i)
                    {
                        if ((i ^ 1) != (int)dir)
                        {
                            //未修改，置false
                            resend[i] = true;
                            //未修改，置false
                            comresend[i] = true;
                        }
                        else
                        {
                            //未修改，置true
                            resend[i] = false;
                            //未修改，置true,command更新为Update
                            comresend[i] = false;
                        }
                    }
                    break;
                default:
                    for (int i = 0; i < 4; ++i)
                    {
                        resend[i] = false;
                        comresend[i] = false;
                    }
                    break;
            }
        }
        public void Clear()
        {
            newone = true;
            for (int i = 0; i < 256; ++i)
                vs[i] = 0;
        }
        public static ushort[] MakeTable(byte ip, Dircation dir, int len = 1)
        {
            ushort[] vs = new ushort[256];
            vs[ip] = (ushort)((len << 2) | ((int)dir & 3));
            return vs;
        }
        public static ushort[] MakeDeleteTable()
        {
            ushort[] vs = new ushort[256];
            for (int i = 0; i < 256; ++i)
                vs[i] = 0;
            return vs;
        }

        public override string ToString()
        {
            string str = "";
            for (byte i = 0; i < 8; ++i)
                str += $"ip: {i}, {(vs[i] == 0 ? " router null " : $"dir: {Dir(i)}, len: {Len(i)}")}\n";
            return str;
        }

        public static string ToString(ushort[] vs)
        {
            string str = "";
            for (byte i = 0; i < 8; ++i)
                str += $"ip: {i}, {(vs[i] == 0 ? " router null " : $"dir: {(Dircation)(vs[i] & 3)}, len: {vs[i] >> 2}")}\n";
            return str;
        }

    }
}