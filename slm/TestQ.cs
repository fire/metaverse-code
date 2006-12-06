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
	private LayoutManager.LayoutControl mainCon, leftCon, rightCon;
	
	public Test() {
		Button btnL1 = new Button();
		Button btnL2 = new Button();
		Button btnL3 = new Button();
		Button btnR1 = new Button();
		Button btnR2 = new Button();

		mainCon = new LayoutManager.LayoutControl(new LayoutManager.QLayout(LayoutManager.QLayout.Direction.Horizontal));
		mainCon.Dock = DockStyle.Fill;
		
		leftCon = new LayoutManager.LayoutControl(new LayoutManager.QLayout(LayoutManager.QLayout.Direction.Vertical));
		rightCon = new LayoutManager.LayoutControl(new LayoutManager.QLayout(LayoutManager.QLayout.Direction.Horizontal));
		
		mainCon.Controls.AddRange( new Control[]{ leftCon, rightCon } );
		
		btnL1.Text = "Test Button L 1";
		btnL2.Text = "Test Button L 2";
		btnL3.Text = "Test Button L 3";
		leftCon.Controls.AddRange ( new Control[]{ btnL1, btnL2, btnL3 } );
		
		btnR1.Text = "Test Button R 1";
		btnR2.Text = "Test Button R 2";
		rightCon.Controls.AddRange ( new Control[]{ btnR1, btnR2 } );
		
		this.Controls.Add(mainCon);
	}
	
	public static void Main() {
		Application.Run(new Test());
	}
}