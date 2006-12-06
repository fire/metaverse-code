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
	
	private void HandleClk(object sender, System.EventArgs e) {
		if (con.Manager is LayoutManager.RubberLayout) {
			System.Console.WriteLine("Rubber       -> Q");
			con.Manager = new LayoutManager.QLayout(LayoutManager.QLayout.Direction.Vertical);
		} else if (con.Manager is LayoutManager.QLayout) {
			System.Console.WriteLine("Q            -> Grid");
			con.Manager = new LayoutManager.GridLayout(2, 2);
		} else if (con.Manager is LayoutManager.GridLayout) {
			System.Console.WriteLine("Grid         -> QConstrained");
			con.Manager = new LayoutManager.QConstrainedLayout(LayoutManager.QConstrainedLayout.Direction.Horizontal);
		} else if (con.Manager is LayoutManager.QConstrainedLayout) {
			System.Console.WriteLine("QConstrained -> Rubber");
			con.Manager = new LayoutManager.RubberLayout();
		}
	}
	
	public Test() {		
		con           = new LayoutManager.LayoutControl(new LayoutManager.RubberLayout());
		con.Size      = new Size(100, 100);
		con.Dock      = DockStyle.Fill;
		con.BackColor = Color.Red;

		Button btn1    = new Button();		
		btn1.Text      = "Test Button 1";
		btn1.Bounds    = new Rectangle(10, 10, 50, 50);
		btn1.BackColor = Color.LightBlue;
		btn1.Click    += new System.EventHandler(HandleClk);
		con.Controls.Add(btn1);

		Button btn2    = new Button();		
		btn2.Text      = "Test Button 2";
		btn2.Bounds    = new Rectangle(35, 70, 50, 25);
		btn2.BackColor = Color.LightGreen;
		btn2.Click    += new System.EventHandler(HandleClk);
		con.Controls.Add(btn2);

		Button btn3    = new Button();		
		btn3.Text      = "Test Button 3";
		btn3.Bounds    = new Rectangle(65, 15, 33, 25);
		btn3.BackColor = Color.Yellow;
		btn3.Click    += new System.EventHandler(HandleClk);
		con.Controls.Add(btn3);
		
		this.Controls.Add(con);
	}
	
	public static void Main() {
		Application.Run(new Test());
	}
}