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
 
using System.Windows.Forms;
using System.Drawing;

public class Test : Form {
	private LayoutManager.LayoutControl con;
	private LayoutManager.ILayoutManager layout;
	
	public Test() {
		Button btn = new Button();
		
		layout = new LayoutManager.RubberLayout();
		con = new LayoutManager.LayoutControl(layout);
		con.Size = new Size(100, 100);
		con.Dock = DockStyle.Fill;
		con.BackColor = Color.Red;
		
		btn.Text = "Test Button";
		btn.Bounds =  new Rectangle(0, 0, 50, 25);
		btn.BackColor = Color.LightBlue;
		
		con.Controls.Add(btn);
		
		this.Controls.Add(con);
	}
	
	public static void Main() {
		Application.Run(new Test());
	}
}