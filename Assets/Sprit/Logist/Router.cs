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
		Right
	}
	public class Router
	{

		private ushort[] vs = new ushort[256];
		public readonly LogistPipe pipe;

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

		private void Set(byte ip, Dircation dir, int len)
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

		public KeyValuePair<bool, bool> ChangeRoute(ushort[] ipTable, Dircation _dir)
		{
			Debug.Log($"{(pipe == null ? "interface" : pipe.transform.position)},{_dir}\n" + ToString(ipTable));
			bool change = false;
			bool rechange = false;
			int dir = (int)_dir ^ 1;
			for (int i = 0; i < 256; ++i)
			{
				if (ipTable[i] == (ushort)0xFFFF && vs[i] != 0)
				{
					if ((vs[i] & 3) == dir)
						vs[i] = 0;
					else
						rechange = true;
				}
				if ((((ipTable[i] >> 2) < (vs[i] >> 2)) || (vs[i] >> 2) == 0) && (ipTable[i] >> 2) != 0)
				{
					vs[i] = (ushort)(((ipTable[i] >> 2) << 2) | dir);
					Debug.Log($"pi:{i},dir:{Dir((byte)i)},len:{Len((byte)i)}");
					change = true;
				}
			}
			return new(change, rechange);
		}

		public void Clear()
		{
			for (int i = 0; i < 256; ++i)
				vs[i] = 0;
		}
		public static ushort[] MakeTable(byte ip, Dircation dir, int len = 1)
		{
			ushort[] vs = new ushort[256];
			vs[ip] = (ushort)((len << 2) | ((byte)dir & 3));
			return vs;
		}

		public override string ToString()
		{
			string str = "";
			for (byte i = 0; i < 8; ++i)
				str += $"ip: {i}, dir: {Dir(i)}, len: {Len(i)}\n";
			return str;
		}

		public static string ToString(ushort[] vs)
		{
			string str = "";
			for (byte i = 0; i < 8; ++i)
				str += $"ip: {i}, dir: {(Dircation)(vs[i] & 3)}, len: {vs[i] >> 2}\n";
			return str;
		}
	}
}