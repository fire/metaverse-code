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
using Hashtable = System.Collections.Hashtable;
using Contol    = System.Windows.Forms.Control;
using Rectangle = System.Drawing.Rectangle;

namespace LayoutManager {
	public class QConstrainedLayout : ILayoutManager {
		public enum HAlign:byte{ Left=0, Center=1, Right=2, Fill=3, Fix=0 };
		public enum VAlign:byte{ Top=0, Center=1, Bottom=2, Fill=3, Fix=0 };
		public enum Direction:byte {Horizontal, Vertical};
		Direction dir;

		#region private class Constrain
		private class Constrain {
			public Constrain(HAlign h, VAlign v) {
				this.h = h;
				this.v = v;
			}
			public HAlign h;
			public VAlign v;
		}
		#endregion

		Hashtable constrains;
		LayoutControl con;

		public QConstrainedLayout(Direction dir) {
			constrains = new Hashtable();
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

		public void SetAlign(object control, HAlign h, VAlign v) {
			if (control != null &&
				(h >= HAlign.Left && h <= HAlign.Fill) &&
				(v >= VAlign.Top && v <= VAlign.Fill)) {
				if (constrains.Contains(control)) {
					constrains[control] = new Constrain(h, v);
				} else {
					constrains.Add(control, new Constrain(h, v));
				}
			}
		}

		private void HorizontalLayout() {
			Constrain constr;
			int maxSpace, space, curPos, step;
			int varContAmount = con.Controls.Count;

			maxSpace = con.ClientSize.Width;
			for (int i = 0; i < con.Controls.Count; ++i) {
				constr = (Constrain)constrains[con.Controls[i]];
				if (constr != null && constr.h != HAlign.Fill) {
					maxSpace -= con.Controls[i].Width;
					--varContAmount;
				}
			}
			space  = (varContAmount > 1)?(maxSpace / varContAmount):maxSpace;
			curPos = 0;
			for (int i = 0; i < con.Controls.Count; ++i) {
				constr = (Constrain)constrains[con.Controls[i]];
				if (constr == null) {
					con.Controls[i].Bounds = new Rectangle(curPos, 0, space, con.ClientSize.Height);
					curPos += space;
				} else {
					step = (constr.h == HAlign.Fill)?space:con.Controls[i].Width;
					if (constr.v == VAlign.Top) {
						con.Controls[i].Bounds = new Rectangle(curPos, 0, step, con.Controls[i].Height);
					} else if (constr.v == VAlign.Center) {
						con.Controls[i].Bounds = new Rectangle(
							curPos,
							con.ClientSize.Height/2 - con.Controls[i].Height/2,
							step,
							con.Controls[i].Height);
					} else if (constr.v == VAlign.Bottom) {
						con.Controls[i].Bounds = new Rectangle(
							curPos,
							con.ClientSize.Height - con.Controls[i].Height,
							step,
							con.Controls[i].Height);
					} else { // constr.v == VAlign.Fill
						con.Controls[i].Bounds = new Rectangle(curPos, 0, step, con.ClientSize.Height);
					}
					curPos += step;
				}
			}
		}

		private void VerticalLayout() {
			Constrain constr;
			int maxSpace, space, curPos, step;
			int varContAmount = con.Controls.Count;

			maxSpace = con.ClientSize.Height;
			for (int i = 0; i < con.Controls.Count; ++i) {
				constr = (Constrain)constrains[con.Controls[i]];
				if (constr != null && constr.v != VAlign.Fill) {
					maxSpace -= con.Controls[i].Height;
					--varContAmount;
				}
			}
			space  = (varContAmount > 1)?(maxSpace / varContAmount):maxSpace;
			curPos = 0;
			for (int i = 0; i < con.Controls.Count; ++i) {
				constr = (Constrain)constrains[con.Controls[i]];
				if (constr == null) {
					con.Controls[i].Bounds = new Rectangle(0, curPos, con.ClientSize.Width, space);
					curPos += space;
				} else {
					step = (constr.v == VAlign.Fill)?space:con.Controls[i].Height;
					if (constr.h == HAlign.Left) {
						con.Controls[i].Bounds = new Rectangle(0, curPos, con.Controls[i].Width, step);
					} else if (constr.h == HAlign.Center) {
						con.Controls[i].Bounds = new Rectangle(
							con.ClientSize.Width/2 - con.Controls[i].Width/2,
							curPos,
							con.Controls[i].Width,
							step);
					} else if (constr.h == HAlign.Right) {
						con.Controls[i].Bounds = new Rectangle(
							con.ClientSize.Width - con.Controls[i].Width,
							curPos,
							con.Controls[i].Width,
							step);
					} else { // constr.h == HAlign.Fill
						con.Controls[i].Bounds = new Rectangle(0, curPos, con.ClientSize.Width, step);
					}
					curPos += step;
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
			if (Control.Controls.Count > 0) {
				if (dir == Direction.Horizontal) {
					HorizontalLayout();
				} else { // dir == Direction.Vertical
					VerticalLayout();
				}
			}

		}

		public void ControlAdded(System.Windows.Forms.Control c) {
			// Nothing to do
		}

		public void ControlRemoved(System.Windows.Forms.Control c) {
			if (c != null) {
				constrains.Remove(c);
			}
		}

		#endregion
	}
}
