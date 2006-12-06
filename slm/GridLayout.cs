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
using Control   = System.Windows.Forms.Control;
using Rectangle = System.Drawing.Rectangle;

namespace LayoutManager {
	public class GridLayout : ILayoutManager {
		private int curRow;
		private int curCol;
		private Control[][] controls;
		private LayoutControl con;
		// horizontal and vertical space between control and grid
		private int vSpace, hSpace;

		public GridLayout(int rows, int cols) : this (rows, cols, 0, 0) {
		}
		
		public GridLayout(int rows, int cols, int hSpace, int vSpace) {
			if (rows < 1) { rows = 1; }
			if (cols < 1) { cols = 1; }
			if (hSpace < 0) { hSpace = 0; }
			if (vSpace < 0) { vSpace = 0; }
			controls = new Control[rows][];
			for (int i = 0; i < rows; ++i) {
				controls[i] = new Control[cols];
			}
			this.hSpace = hSpace;
			this.vSpace = vSpace;
			curCol = 0;
			curRow = 0;
		}

		private int Rows {
			get {
				return controls.Length;
			}
		}

		private int Cols {
			get {
				return controls[0].Length;
			}
		}
		
		public int VerticalSpace {
			get {
				return vSpace;
			}
			set {
				if ( (value >= 0) && (value != vSpace) ) {
					vSpace = value;
					DoLayout();
				}
			}
		}

		public int HorizontalSpace {
			get {
				return hSpace;
			}
			set {
				if ( (value >= 0) && (value != hSpace) ) {
					hSpace = value;
					DoLayout();
				}
			}
		}

		public void GoTo(int row, int col) {
			if (row >= 0 && row < Rows) {
				curRow = row;
			}
			if (col >= 0 && col < Cols) {
				curCol = col;
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
			int rowSpace, colSpace;

			if (Control != null) {
				rowSpace = (Control.ClientSize.Height / Rows);
				colSpace = (Control.ClientSize.Width / Cols);
				
				for (int r = 0; r < Rows; ++r) {
					for (int c = 0; c < Cols; ++c) {
						if (controls[r][c] != null) {
							controls[r][c].Bounds = new Rectangle(c * colSpace + hSpace, r * rowSpace + vSpace, colSpace - 2 * hSpace, rowSpace - 2 * vSpace);
						}
					}
				}
			}
		}

		public void ControlAdded(Control c) {
			if (Control != null && curRow < controls.Length) {
				if (controls[curRow][curCol] != null) {
					Control.Controls.Remove(controls[curRow][curCol]);
				}
				controls[curRow][curCol] = c;
				curCol += 1;
				if (curCol >= Cols) {
					curCol = 0;
					curRow = curRow + 1;
				}
			}
		}

		public void ControlRemoved(Control c) {
			int rows, cols;
			
			if (Control != null) {
				rows = Rows;
				cols = Cols;
				for (int row = 0; row < rows; ++row) {
					for (int col = 0; col < cols; ++col) {
						if (controls[row][col] == c) {
							controls[row][col] = null;
						}
					}
				}
			}
		}

		#endregion
	}
}
