//
// System.Collections.Specialized.BitVector32.cs
//
// Author:
//   Miguel de Icaza (miguel@ximian.com)
//   Lawrence Pit (loz@cable.a2000.nl)
//
// (C) Ximian, Inc.  http://www.ximian.com
//

using System;
using System.Text;

namespace System.Collections.Specialized {
	
	public struct BitVector32 {
		int bits;

		public struct Section {
			private short mask;
			private short offset;
			
			internal Section (short mask, short offset) {
				this.mask = mask;
				this.offset = offset;
			}
			
			public short Mask {
				get { return mask; }
			}
			
			public short Offset {
				get { return offset; }
			}
			
			public override bool Equals (object o) 
			{
				if (! (o is Section))
					return false;
					
				Section section = (Section) o;
				return this.mask == section.mask &&
				       this.offset == section.offset;
			}			
			
			public override int GetHashCode ()
			{
				return (((Int16) mask).GetHashCode () << 16) + 
				       ((Int16) offset).GetHashCode ();
			}
			
			public override string ToString ()
			{
				return "Section{0x" + Convert.ToString(mask, 16) + 
				       ", 0x" + Convert.ToString(offset, 16) + "}";
			}
		}
		
		// Constructors
		
		public BitVector32 (BitVector32 source)
		{
			bits = source.bits;
		}

		public BitVector32 (int init)
		{
			bits = init;
		}
		
		// Properties
		
		public int Data {
			get { return bits; }
		}
		
		public int this [BitVector32.Section section] {
			get {
				return ((bits >> section.Offset) & section.Mask);
			}

			set {
				if (value < 0)
					throw new ArgumentException ("Section can't hold negative values");
				if (value > section.Mask)
					throw new ArgumentException ("Value too large to fit in section");
				bits &= (~section.Mask << section.Offset);
				bits |= (value << section.Offset);
			}
		}
		
		public bool this [int mask] {
			get {
				return (bits & mask) == mask;
			}
			
			set { 
				if (value)
					bits |= mask;
				else
					bits &= ~mask;
			}
		}
		
		// Methods
		
		public static int CreateMask ()
		{
			return 1;
		}

		public static int CreateMask (int prev)
		{
			if (prev == 0)
				return 1;
			if (prev == Int32.MinValue) 
				throw new InvalidOperationException ("all bits set");
			return prev << 1;
		}

		public static Section CreateSection (short maxValue)
		{
			return CreateSection (maxValue, new Section (0, 0));
		}
		
		public static Section CreateSection (short maxValue, BitVector32.Section previous)
		{
			if (maxValue < 1)
				throw new ArgumentException ("maxValue");
			
			int newmask = (int) maxValue;
			int mask = 0x8000;
			while ((newmask & mask) == 0)
				mask >>= 1;
			while (mask > 0) {
				newmask |= mask;
				mask >>= 1;
			}

			short count = 0;
			int prev = previous.Mask;
			mask = 0x8000;
			while (mask > 0) {
				if ((prev & mask) != 0)
					count++;
				mask >>= 1;
			}

			return new Section ((short) newmask, (short) (previous.Offset + count));
		}
		
		public override bool Equals (object o)
		{
			if (!(o is BitVector32))
				return false;

			return bits == ((BitVector32) o).bits;
		}

		public override int GetHashCode ()
		{
			return bits.GetHashCode ();
		}
		
		public override string ToString () 
		{
			return ToString (this);
		}
		
		public static string ToString (BitVector32 value)
		{
			StringBuilder b = new StringBuilder ();
			b.Append ("BitVector32{");
			long mask = (long) 0x80000000;
			while (mask > 0) {
				b.Append (((value.bits & mask) == 0) ? '0' : '1');
				mask >>= 1;
			}
			b.Append ('}');
			return b.ToString ();
		}
	}
}
