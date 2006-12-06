/*
Copyright (c) 2003 Markus Loeberbauer

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.
 
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE
OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
 
/* Author:   Markus Loeberbauer
 * Homepage: http://www.ssw.uni-linz.ac.at/General/Staff/ML/
 * EMail:    Loeberbauer@ssw.jku.at
 * Phone:    + 43-732-2468-7134
 *
 * Compiler: Microsoft .NET Framework 1.1
 * */
using System;
using Hashtable = System.Collections.Hashtable;
using Rectangle = System.Drawing.Rectangle;
using Control   = System.Windows.Forms.Control;

namespace LayoutManager {
	public class RubberLayout : ILayoutManager {
		Hashtable boundTable;
		LayoutControl con;

		#region private class RelBounds
		/// <summary>
		///  This class contains the relative Bound
		///  for a control.
		/// </summary>
		private class RelBounds {
			private double x, y, width, height;

			public RelBounds(double x, double y, double width, double height) {
				this.x = x;
				this.y = y;
				this.width = width;
				this.height = height;
			}

			public RelBounds(Rectangle controlBound, Rectangle panelBound) {
				x = (double)controlBound.X / (double)panelBound.Width;
				y = (double)controlBound.Y / (double)panelBound.Height;
				width = (double)controlBound.Width / (double)panelBound.Width;
				height = (double)controlBound.Height / (double)panelBound.Height;
			}

			public Rectangle GetAbsBounds(Rectangle panelBounds) {
				return new Rectangle((int)(x * panelBounds.Width), (int)(y * panelBounds.Height),
					(int)(width * panelBounds.Width), (int)(height * panelBounds.Height));
			}

			public override string ToString() {
				return string.Format("x = {0}, y = {1}, width = {2}, height = {3}", x, y, width, height);
			}

		}
		#endregion

		public LayoutControl Control {
			get {
				return con;
			}
			set {
				if (value != null && value != con) {
					con = value;
					ReInit();
				}
			}
		}

		public RubberLayout() {
			boundTable = new Hashtable();
			con = null;
		}

		/// <summary>
		/// Call this method to take over the actual layout.
		/// </summary>
		public void ReInit() {
			boundTable = new Hashtable();
			foreach (Control c in Control.Controls) {
				boundTable.Add(c, new RelBounds(c.Bounds, Control.Bounds));
			}
		}

		public void DoLayout() {
			RelBounds rb;

			if (Control != null) {
				foreach (Control c in Control.Controls) {
					rb = (RelBounds)boundTable[c];
					if (rb != null) {
						c.Bounds = rb.GetAbsBounds(Control.Bounds);
					}
				}
			}
		}

		public void ControlAdded(Control c) {
			if (c != null) {
				if (boundTable.Contains(c)) {
					boundTable[c] = new RelBounds(c.Bounds, Control.Bounds);
				} else {
					boundTable.Add(c, new RelBounds(c.Bounds, Control.Bounds));
				}
			}
		}

		public void ControlRemoved(Control c) {
			if (c != null) {
				boundTable.Remove(c);
			}
		}
	}
}
