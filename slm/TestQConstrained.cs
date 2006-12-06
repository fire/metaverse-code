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

public class Test : Form {

  public Test() {
    LayoutManager.QConstrainedLayout topLayout, bottomLayout;
    LayoutManager.LayoutControl mainCon, topCon, bottomCon;
    //
    // topCon
    //
    topLayout = new LayoutManager.QConstrainedLayout(
      LayoutManager.QConstrainedLayout.Direction.Vertical);
    topCon = new LayoutManager.LayoutControl(topLayout);
    Label lab1, lab2, lab3, lab4, lab5, lab6, lab7;
    lab1 = new Label();
    lab1.Text = "Left Fix Label";
    lab1.Size = new Size(150, 24);
    lab1.BackColor = Color.LightBlue;
    topLayout.SetAlign(lab1, HAlign.Left, VAlign.Fix);

    lab2 = new Label();
    lab2.Text = "Center Fix Label";
    lab2.Size = new Size(150, 24);
    lab2.BackColor = Color.Red;
    topLayout.SetAlign(lab2, HAlign.Center, VAlign.Fix);

    lab3 = new Label();
    lab3.Text = "Right Fix Label";
    lab3.Size = new Size(150, 24);
    lab3.BackColor = Color.Green;
    topLayout.SetAlign(lab3, HAlign.Right, VAlign.Fix);

    lab4 = new Label();
    lab4.Text = "Left Fill Label";
    lab4.Size = new Size(150, 24);
    lab4.BackColor = Color.LightBlue;
    topLayout.SetAlign(lab4, HAlign.Left, VAlign.Fill);

    lab5 = new Label();
    lab5.Text = "Center Fill Label";
    lab5.Size = new Size(150, 24);
    lab5.BackColor = Color.Red;
    topLayout.SetAlign(lab5, HAlign.Center, VAlign.Fill);

    lab6 = new Label();
    lab6.Text = "Right Fill Label";
    lab6.Size = new Size(150, 24);
    lab6.BackColor = Color.Green;
    topLayout.SetAlign(lab6, HAlign.Right, VAlign.Fill);

    lab7 = new Label();
    lab7.Text = "Fill Fill Label";
    lab7.BackColor = Color.LightBlue;

    topCon.Controls.AddRange(
      new Control[] { lab1, lab2, lab3, lab4, lab5, lab6, lab7 } );

    //
    // bottomCon
    //
    bottomLayout = new LayoutManager.QConstrainedLayout(
      LayoutManager.QConstrainedLayout.Direction.Horizontal);
    bottomCon = new LayoutManager.LayoutControl(bottomLayout);

    Label bottomLab;
    TextBox txt;
    Button btn;

    bottomLab           = new Label();
    bottomLab.Text      = "Very Long Label:";
    bottomLab.BackColor = Color.LightBlue;
    bottomLab.Bounds    = new Rectangle(0, 0, 200, 24);
    bottomLayout.SetAlign(bottomLab, HAlign.Fix, VAlign.Top);

    txt           = new TextBox();
    txt.Multiline = true;

    btn      = new Button();
    btn.Text = "Button";
    bottomLayout.SetAlign(btn, HAlign.Fix, VAlign.Bottom);

    bottomCon.Controls.AddRange( new Control[]{ bottomLab, txt, btn } );
    //
    // mainCon
    //
    mainCon = new LayoutManager.LayoutControl (
      new LayoutManager.QLayout(LayoutManager.QLayout.Direction.Vertical));
    mainCon.Controls.AddRange(new Control[] { topCon, bottomCon });
    mainCon.Dock = DockStyle.Fill;
    //
    // this
    //
    this.Controls.Add(mainCon);
  }

  public static void Main() {
    Application.Run(new Test());
  }
}