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
	public class QLayout : ILayoutManager {
		public enum Direction:byte {Horizontal, Vertical};
		LayoutControl con;
		Direction dir;

		public QLayout(Direction dir) {
			this.Dir = dir;
		}

		public Direction Dir {
			get {
				return dir;
			}
			set {
				if (value == Direction.Horizontal || value == Direction.Vertical) {
					dir = value;
				}
			}
		}

		#region ILayoutMgr Members
		public LayoutControl Control {
			get {
				return con;
			}
			set {
				if (value != null && value != con) {
					con = value;
				}
			}
		}

		public void DoLayout() {
			int space;

			if (Control != null && Control.Controls.Count > 0) {
				if (dir == Direction.Horizontal) {
					space = Control.ClientSize.Width / Control.Controls.Count;
					for (int i = 0; i < Control.Controls.Count; ++i) {
						Control.Controls[i].Bounds = new Rectangle(i * space, 0, space, Control.ClientSize.Height);
					}
				} else { // dir == Direction.Vertical
					space = Control.ClientSize.Height / Control.Controls.Count;
					for (int i = 0; i < Control.Controls.Count; ++i) {
						Control.Controls[i].Bounds = new Rectangle(0, i * space, Control.ClientSize.Width, space);
					}
				}
			}
		}

		public void ControlAdded(Control c) {
			// Nothing to do
		}

		public void ControlRemoved(Control c) {
			// Nothing to do
		}
		#endregion
	}
}