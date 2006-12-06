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
using Control     = System.Windows.Forms.Control;
using Rectangle   = System.Drawing.Rectangle;
using Point       = System.Drawing.Point;
using ArrayList   = System.Collections.ArrayList;
using ICollection = System.Collections.ICollection;

namespace LayoutManager {
	public class FlowLayout : ILayoutManager {
		private int hPad, vPad; // horizontal and vertical padding values
		private LayoutControl con;
		private bool tabOrder;
		private ControlTabComparer comp = new ControlTabComparer();
		private ArrayList sortedControls;


		private class ControlTabComparer : System.Collections.IComparer {
			public int Compare(Control x, Control y) {				
				if (x == null && y == null) { return 0; }
				else if (x == null) { return -1; }
				else if (y == null) { return  1; }
				else { return x.TabIndex - y.TabIndex; }
			}
		
			public int Compare(object x, object y) {			
				return Compare((Control) x, (Control) y);
			}
		}

		public FlowLayout(int horizontalPadding, int verticalPadding, bool tabOrder) {
			hPad = horizontalPadding;
			vPad = verticalPadding;
			this.tabOrder = tabOrder;
			sortedControls = new ArrayList();
		}

		public FlowLayout(int horizontalPadding, int verticalPadding) : 
		this(horizontalPadding, verticalPadding, false) { }
		
		public FlowLayout() : this(0, 0, false) { }

		// true ... controls will be sorted by TabIndex
		// false .. controls will be sorted by insertion order
		public bool TabOrder {
			get {
				return tabOrder;
			}
			set {
				if (value != tabOrder) {
					tabOrder = value;
					DoLayout();
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
			int curTopLine, curLeftLine, curBottomLine;
			
			if (Control != null && Control.Controls.Count > 0) {
				curBottomLine = 0;
				curTopLine  = curBottomLine + vPad;
				curLeftLine = hPad;
				
				ICollection col = (TabOrder)?(ICollection)sortedControls:(ICollection)con.Controls;

				foreach (Control c in col) {
					if (curLeftLine + c.Size.Width > con.Width) {
						curTopLine  = curBottomLine + vPad;
						curLeftLine = hPad;
					}
					c.Location = new Point(curLeftLine, curTopLine);
					curLeftLine = curLeftLine + c.Size.Width + hPad;
					if (curBottomLine < (curTopLine + c.Size.Height)) {
						curBottomLine = (curTopLine + c.Size.Height);
					}
				}
			}
		}
		
		public void ControlAdded(Control c) {
			sortedControls.Add(c);
			sortedControls.Sort(comp);
		}

		public void ControlRemoved(Control c) {
			sortedControls.Remove(c);
		}

		#endregion
	}
}
