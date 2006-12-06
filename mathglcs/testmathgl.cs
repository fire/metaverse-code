// Copyright Hugh Perkins 2006
// hughperkins@gmail.com  http://manageddreams.com
//
// This program is free software; you can redistribute it and/or modify it
// under the terms of the GNU General Public License version 2 as published by the
// Free Software Foundation;
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
//  more details.
//
// You should have received a copy of the GNU General Public License along
// with this program in the file licence.txt; if not, write to the
// Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-
// 1307 USA
// You can find the licence also on the web at:
// http://www.opensource.org/licenses/gpl-license.php
//

using System;
using MathGl;

class EntryPoint
{
    static void RunVectorTests()
    {
        Console.WriteLine("===============================================================");
        Console.WriteLine("");
        Console.WriteLine("Vector Tests");
        Console.WriteLine("");
        
        GLVector3d one = new GLVector3d( 2,5,7 );
        Console.WriteLine( one );
        Console.WriteLine( new GLVector3d( one ) );
        Console.WriteLine("");
        
        Console.WriteLine( one.dot( new GLVector3d( 1, 0 , 0 ) ) );
        Console.WriteLine( one.dot( new GLVector3d( 0, 1 , 0 ) ) );
        Console.WriteLine( one.dot( new GLVector3d( 0, 0 , 1 ) ) );
        Console.WriteLine("");
        
        Console.WriteLine( one.dot( new GLVector3d( 1, 1 , 0 ) ) );
        Console.WriteLine( one.dot( new GLVector3d( 1, 0 , 1 ) ) );
        Console.WriteLine( one.dot( new GLVector3d( 0, 1 , 1 ) ) );
        Console.WriteLine("");
        
        Console.WriteLine( one.dot( new GLVector3d( 3, 0 , 0 ) ) );
        Console.WriteLine( one.dot( new GLVector3d( 0, 9 , 0 ) ) );
        Console.WriteLine( one.dot( new GLVector3d( 0, 0 , 2 ) ) );
        Console.WriteLine("");
        Console.WriteLine( one - new GLVector3d( 5,8,-4 ) );
        Console.WriteLine( one * 2.5 );
        Console.WriteLine( one / 2.5 );
    }
    
    static void RunMatrixTests()
    {
        Console.WriteLine("===============================================================");
        Console.WriteLine("");
        Console.WriteLine("Matrix Tests");
        Console.WriteLine("");
        
        Console.WriteLine( GLMatrix4d.identity().ToString() );
        GLMatrix4d matrix = GLMatrix4d.identity();
        GLVector3d vector3 = new GLVector3d( 0, 5, 4 );
        Console.WriteLine( vector3 );
        Console.WriteLine("");
        
        Console.WriteLine("LoadRotate");
        matrix.loadRotate( 30, 1, 0, 0 );
        Console.WriteLine( matrix );
        Console.WriteLine( new GLMatrix4d( matrix ) );
        Console.WriteLine( matrix * vector3 );
        Console.WriteLine("");
        
        Console.WriteLine("LoadTranslate");
        matrix.loadTranslate( 30, -7, 1 );
        Console.WriteLine( matrix );
        Console.WriteLine( matrix * vector3 );
        Console.WriteLine("");
        
        Console.WriteLine("LoadScale");
        matrix.loadScale( 30, -7, 3 );
        Console.WriteLine( matrix );
        Console.WriteLine( matrix * vector3 );
        Console.WriteLine("");
        
        matrix = GLMatrix4d.identity();
        
        matrix.applyRotate( 30, 0, 1, 0 );
        matrix.applyRotate( 30, 0, 1, 0 );
        matrix.applyRotate( 30, 0, 1, 0 );
        Console.WriteLine( matrix );
        Console.WriteLine( matrix * vector3 );
        
        matrix.applyTranslate( 5, 3, -7 );
        Console.WriteLine( matrix );
        Console.WriteLine( matrix * vector3 );
        
        matrix.applyScale( 2, 3, 4 );
        Console.WriteLine( matrix );
        Console.WriteLine( matrix * vector3 );
    }

    public static void Main()
    {
        RunVectorTests();
        RunMatrixTests();
    }
}
