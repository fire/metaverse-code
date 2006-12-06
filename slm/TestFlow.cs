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
using System.Windows.Forms;
using System.Drawing;

public class Test : Form {
	private LayoutManager.LayoutControl con;
	private LayoutManager.FlowLayout layout;
	
	private void ClickBtn1(object sender, EventArgs e) {
		layout.TabOrder = !layout.TabOrder;
	}
	
	public Test() {
		Button btn1 = new Button();
		Button btn2 = new Button();
		Button btn3 = new Button();
		Button btn4 = new Button();
		Button btn5 = new Button();
		Random rnd  = new Random();
		
		layout = new LayoutManager.FlowLayout(3, 4);
		
		con = new LayoutManager.LayoutControl(layout);
		con.Size = new Size(600, 400);
		con.Dock = DockStyle.Fill;
		con.BackColor = Color.Red;
		
		btn1.Text = "Test Button 1";
		btn1.BackColor = Color.LightBlue;
		btn1.Size = new Size(rnd.Next(25, 200), rnd.Next(15, 60));
		btn1.Click += new EventHandler(ClickBtn1);
		btn1.TabIndex = 2;
		
		con.Controls.Add(btn1);
		
		btn2.Text = "Test Button 2";
		btn2.BackColor = Color.LightGreen;
		btn2.Size = new Size(rnd.Next(25, 200), rnd.Next(15, 60));
		btn2.TabIndex = 1;
		con.Controls.Add(btn2);
		
		btn3.Text = "Test Button 3";
		btn3.BackColor = Color.Red;
		btn3.Size = new Size(rnd.Next(25, 200), rnd.Next(15, 60));
		btn3.TabIndex = 5;
		con.Controls.Add(btn3);
		
		btn4.Text = "Test Button 4";
		btn4.BackColor = Color.Yellow;
		btn4.Size = new Size(rnd.Next(25, 200), rnd.Next(15, 60));
		btn4.TabIndex = 3;
		con.Controls.Add(btn4);
		
		btn5.Text = "Test Button 5";
		btn5.BackColor = Color.SteelBlue;
		btn5.Size = new Size(rnd.Next(25, 200), rnd.Next(15, 60));
		btn5.TabIndex = 4;
		con.Controls.Add(btn5);
		
		this.Controls.Add(con);
	}
	
	public static void Main() {
		Application.Run(new Test());
	}
}