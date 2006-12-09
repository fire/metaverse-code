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
using LayoutEventHandler  = System.Windows.Forms.LayoutEventHandler;
using LayoutEventArgs     = System.Windows.Forms.LayoutEventArgs;
using ControlEventHandler = System.Windows.Forms.ControlEventHandler;
using ControlEventArgs    = System.Windows.Forms.ControlEventArgs;
using Control             = System.Windows.Forms.Control;
using BrowsableAttribute  = System.ComponentModel.BrowsableAttribute;
using ReadOnlyAttribute   = System.ComponentModel.ReadOnlyAttribute;

namespace LayoutManager {
	public class LayoutControl : Control {
		private ILayoutManager layoutMgr;
		
		public LayoutControl(ILayoutManager layoutMgr) {
			Manager = layoutMgr;
			this.Layout         += new LayoutEventHandler(DoLayout);
			this.ControlAdded   += new ControlEventHandler(ContrAdd);
			this.ControlRemoved += new ControlEventHandler(ContrRem);
		}

		public LayoutControl() : this (null) { }

		[Browsable(true)]
		[ReadOnly(true)]
		public LayoutManager.ILayoutManager Manager {
			get {
				return layoutMgr;
			}
			set {
				if (value != null && value != layoutMgr) {
					layoutMgr = value;
					layoutMgr.Control = this;
					foreach(Control c in Controls) {
						layoutMgr.ControlAdded(c);
					}
					layoutMgr.DoLayout();
				}
			}
		}

		private void ContrAdd(object sender, ControlEventArgs e) {
			layoutMgr.ControlAdded(e.Control);
		}

		private void ContrRem(object sender, ControlEventArgs e) {
			layoutMgr.ControlRemoved(e.Control);
		}

		private void DoLayout(object sender, LayoutEventArgs e) {
			layoutMgr.DoLayout();
		}
	}
}
