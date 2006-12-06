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
using HAlign = LayoutManager.QConstrainedLayout.HAlign;
using VAlign = LayoutManager.QConstrainedLayout.VAlign;
using QConstrainedSpace = LayoutManager.QConstrainedSpace;

public class Test : Form {

  public Test() {
    LayoutManager.QConstrainedLayout topLayout, bottomLayout;
    LayoutManager.LayoutControl mainControl, topControl, bottomControl;
    //
    // topControl
    //
    topLayout = new LayoutManager.QConstrainedLayout(
      LayoutManager.QConstrainedLayout.Direction.Vertical);
    topControl = new LayoutManager.LayoutControl(topLayout);
    Label lab1, lab2, lab3, lab4, lab5, lab6, lab7;
    lab1 = new Label();
    lab1.Text = "Label Blue";
    lab1.BackColor = Color.LightBlue;

    lab2 = new Label();
    lab2.Text = "Label Red";
    lab2.BackColor = Color.Red;

    lab3 = new Label();
    lab3.Text = "Label Green";
    lab3.BackColor = Color.Green;

    lab4 = new Label();
    lab4.Text = "Label LightBlue";
    lab4.BackColor = Color.LightBlue;

    topControl.Controls.AddRange(
      new Control[] {
        lab1,
        QConstrainedSpace.CreateSpace(),
        lab2,
        lab3,
        QConstrainedSpace.CreateSpace(topLayout, 10),
        lab4
        }
      );

    //
    // bottomControl
    //
    bottomLayout = new LayoutManager.QConstrainedLayout(
      LayoutManager.QConstrainedLayout.Direction.Horizontal);
    bottomControl = new LayoutManager.LayoutControl(bottomLayout);


    lab5 = new Label();
    lab5.Text = "Label Red 2";
    lab5.BackColor = Color.Red;

    lab6 = new Label();
    lab6.Text = "Label Green 2";
    lab6.BackColor = Color.Green;

    lab7 = new Label();
    lab7.Text = "Label LightBlue 2";
    lab7.BackColor = Color.LightBlue;


    bottomControl.Controls.AddRange(
      new Control[] {
        QConstrainedSpace.CreateSpace(bottomLayout, 15),
        lab5,
        QConstrainedSpace.CreateSpace(bottomLayout, 5),
        lab6,
        QConstrainedSpace.CreateSpace(bottomLayout, 5),
        lab7,
        QConstrainedSpace.CreateSpace(bottomLayout, 15)
      }
    );
    //
    // mainControl
    //
    mainControl = new LayoutManager.LayoutControl(
      new LayoutManager.QLayout(LayoutManager.QLayout.Direction.Vertical));
    mainControl.Controls.AddRange(new Control[] { topControl, bottomControl });
    mainControl.Dock = DockStyle.Fill;
    //
    // this
    //
    this.Controls.Add(mainControl);
  }

  public static void Main() {
    Application.Run(new Test());
  }
}
