using System;

namespace MobaClient
{
	public class DelegateType
	{
		public delegate void CM0();

		public delegate void CM1<a>(a _a);

		public delegate void CM2<a, b>(a _a, b _b);

		public delegate void CM3<a, b, c>(a _a, b _b, c _c);

		public delegate void CM4<a, b, c, d>(a _a, b _b, c _c, d _d);

		public delegate void CM5<a, b, c, d, e>(a _a, b _b, c _c, d _d, e _e);

		public delegate void CM6<a, b, c, d, e, f>(a _a, b _b, c _c, d _d, e _e, f _f);

		public delegate void CM7<a, b, c, d, e, f, g>(a _a, b _b, c _c, d _d, e _e, f _f, g _g);

		public delegate void CM8<a, b, c, d, e, f, g, h>(a _a, b _b, c _c, d _d, e _e, f _f, g _g, h _h);

		public delegate void CM9<a, b, c, d, e, f, g, h, l>(a _a, b _b, c _c, d _d, e _e, f _f, g _g, h _h, l _l);
	}
}
