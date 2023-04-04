using System.Collections;
using UnityEngine;

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

		public ushort[] IpTable { get { return (ushort[])vs.Clone(); } }

		private int Len(byte ip)
		{
			return vs[ip] >> 2;
		}
		private Dircation Dir(byte ip)
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

		public bool ChangeRoute(ushort[] ipTable, Dircation _dir)
		{

			bool change = false;
			int dir = (int)_dir ^ 1;
			for (int i = 0; i < 256; ++i)
			{
				if (ipTable[i] == 0)
				{
					if ((vs[i] ^ dir) == 0)
						vs[i] = 0;
					else
						change = true;
				}
				if (((ipTable[i] + 4) | 3) < (vs[i] | 3))
				{
					vs[i] = (ushort)(ipTable[i] + 4);
				}
			}
			return change;
		}

		public static ushort[] MakeTable(byte ip, Dircation dir, int len = 1)
		{
			ushort[] vs = new ushort[256];
			vs[ip] = (ushort)((len << 2) | ((byte)dir & 3));
			return vs;
		}
	}
}