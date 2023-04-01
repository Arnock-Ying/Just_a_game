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

		private ushort[] vs = new ushort[255];

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
		public Router()
		{
			for (int i = 0; i < 255; ++i) vs[i] = 0;
		}

		public bool ChangeRoute(int ip, Dircation dir, int len)
		{
			if (ip > 255 || ip < 0) return false;
			if (len <= 0 || Len((byte)ip) <= len) return false;

			Set((byte)ip, dir, len);
			return true;
		}
	}
}