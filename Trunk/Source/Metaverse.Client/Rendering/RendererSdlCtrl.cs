// Copyright Hugh Perkins 2004,2005,2006
// hughperkins@gmail.com http://manageddreams.com
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

//! \file
//! \brief This module carries out OpenGL initialization and manages drawing of the world on demand

using System;
using System.Collections;

using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using Tao.Sdl;
using Tao.OpenGl;
using SdlDotNet;
using Tao.Platform.Windows;

namespace OSMP
{
    public delegate void WriteNextFrameCallback();

    // This creates the window into which we render, and also the chat panel
    // provides a Menu, a ContextMenu, and event callbacks for key, mouse, contextmenu
    //
    // Note to self: would be nice to have some sort of publisher/subscriber pattern for creating panels and rendering areas
    public class RendererSdlCtrl : Form, IRenderer
    {        
        public new event MouseEventHandler MouseDown;
        public new event MouseEventHandler MouseMove;
        public new event MouseEventHandler MouseUp;
        public new event KeyEventHandler KeyUp;
        public new event KeyEventHandler KeyDown;
            
        public event EventHandler ContextMenuPopup;
        
        static RendererSdlCtrl instance;
        public static IRenderer GetInstance()
        {
            return (IRenderer)instance;
        }
        
        MainLoopDelegate mainloop;

        IGraphicsHelper graphics;
        Camera camera;
        
        string WindowName = "The OpenSource Metaverse Project";

        Container components = new Container();
        SimpleOpenGlControl glControl = null;
        Panel impanel;
        
        int iWindowWidth;
        int iWindowHeight;
            
        WriteNextFrameCallback writenextframecallback;
        
        IPicker3dModel picker3dmodel = new Picker3dModelGl();
        
        public IPicker3dModel GetPicker3dModel()
        {
            return picker3dmodel;
        }
        
        public RendererSdlCtrl()
        {
            InitializeComponent();
            Show();
        }
        
        public Panel IMPanel{
            get{ return impanel; }
        }
        
        public int WindowWidth
        {
            get
            {
                return iWindowWidth;
            }
        }
        
        public int WindowHeight
        {
            get
            {
                return iWindowHeight;
            }
        }
        
        public void MakeRendererActiveControl()
        {
            glControl.Focus();
        }

        //public Point WindowTopLeftScreenCoords
        //{
          //  get
            //{
              //  return PointToScreen(new Point(0, 0));
            //}
        //}

        void InitializeComponent()
        {
            SuspendLayout();

            LayoutManager.LayoutControl renderermainwindowlayout = new LayoutManager.LayoutControl(new LayoutManager.RubberLayout());
            renderermainwindowlayout.TabStop = true;
            renderermainwindowlayout.Size = new Size(100,100);

            //glControl.Location = new Point( 20, 20 );
            //glControl.Dock = DockStyle.Fill;

            glControl = new SimpleOpenGlControl();   
            glControl.Visible = true;
            
            glControl.MouseMove +=  new MouseEventHandler( this._MouseMove );
            glControl.MouseDown +=  new MouseEventHandler( this._MouseDown );
            glControl.MouseUp +=  new MouseEventHandler( this._MouseUp );
            glControl.KeyDown +=  new KeyEventHandler( this._KeyDown );
            glControl.KeyUp +=  new KeyEventHandler( this._KeyUp );
            
            glControl.AutoSwapBuffers = false;
            glControl.AutoFinish = false;
            
            glControl.Bounds = new Rectangle( 0, 0, 100, 87 ); // position (0,0), 100% width, 85% height
            renderermainwindowlayout.Controls.Add( glControl );
            
          // ideally this would be done via publisher/subscriber ,but not quite sure how for now
            // This panel is owned by the ChatController class, or equivalent, for doing IM
            impanel = new Panel();
            impanel.Bounds = new Rectangle( 0, 87, 100, 13 );
            impanel.TabStop = false;
            renderermainwindowlayout.Controls.Add( impanel );

            Menu = new MainMenu();
            //ContextMenu = new ContextMenu();

          //  this.LocationChanged += new EventHandler(RendererSdlCtrl_LocationChanged);

            //ContextMenu.Popup += new EventHandler( _ContextMenuPopup );
            
          //  Controls.Add( glControl );
            //glControl.Location = new Point( 20, 20 );
            //glControl.Dock = DockStyle.Fill;
            renderermainwindowlayout.Location = new Point( 0, 0 );
            renderermainwindowlayout.Dock = DockStyle.Fill;
            Controls.Add( renderermainwindowlayout );
            
            //this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            //this.ClientSize = new System.Drawing.Size(292, 273);
           // this.ClientSize = new System.Drawing.Size(800, 600);
            this.Name = "OSMP2";
            this.Text = "OSMP2";

            Rectangle screenworkingarea =Screen.GetWorkingArea(this);
            Left=0;
            Top=0;
            Size=new Size(screenworkingarea.Width,screenworkingarea.Height );
            WindowState = FormWindowState.Maximized;

            ResumeLayout(false);
        }

        //void RendererSdlCtrl_LocationChanged(object sender, EventArgs e)
        //{
          //  Console.WriteLine("new location: " + this.Location.X + " " + this.Location.Y);
        //}
        
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad(e);
            InitGL();
        }
            
        protected override void OnResize( EventArgs e )
        {
            base.OnResize(e);
            ResizeGL(glControl.Width, glControl.Height);
        }
            
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if( components != null )
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
                
        void _MouseMove( object source, MouseEventArgs e )
        {
            if( MouseMove != null )
            {
                MouseMove( source, e );
            }
        }
        
        void _MouseDown( object source, MouseEventArgs e )
         {
             if( MouseDown != null )
             {
                 MouseDown( source, e );
             }
         }
        
         void _MouseUp( object source, MouseEventArgs e )
         {
             if( MouseUp != null )
             {
                 MouseUp( source, e );
             }
         }
        
         void _KeyDown( object source, KeyEventArgs e )
         {
             if( KeyDown != null )
             {
                 KeyDown( source, e );
             }
         }
        
        void _KeyUp( object source, KeyEventArgs e )
        {
            if( KeyUp != null )
            {
                KeyUp( source, e );
            }
        }
        
        void _ContextMenuPopup( object source, EventArgs e )
        {
            if( ContextMenuPopup != null )
            {
                ContextMenuPopup( source, e );
            }               
        }
        
        private void InitGL()
        {
            glControl.InitializeContexts();
            
            Gl.glClearColor(0.0f,0.0f,0.0f,0.0f);

            Gl.glShadeModel(Gl.GL_SMOOTH);

            // enable alpha textures (eg for editing handles):
            Gl.glEnable (Gl.GL_BLEND);
            Gl.glBlendFunc (Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glEnable (Gl.GL_CULL_FACE);
    
            Gl.glEnable(Gl.GL_LIGHTING);
            Gl.glEnable(Gl.GL_LIGHT0);
    
            // note to self: maybe make this configurable as part of world somehow
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_AMBIENT, new float[]{0.4f, 0.4f, 0.4f, 1.0f});
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_DIFFUSE, new float[]{ 0.6f, 0.6f, 0.6f, 1.0f });
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_SPECULAR, new float[]{ 0.2f, 0.2f, 0.2f, 1.0f });
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, new float[]{ -1.5f, 1.0f, -4.0f, 1.0f });
            
            OnResize(null);
        }

        public void ApplyViewingMatrixes()
        {
            Gl.glMatrixMode( Gl.GL_PROJECTION );
            Gl.glLoadIdentity();
            Glu.gluPerspective( 45.0, (float)iWindowWidth / (float)iWindowHeight, 0.5, 100.0 );
            
            Gl.glMatrixMode( Gl.GL_MODELVIEW );
            Gl.glViewport (0, 0, iWindowWidth, iWindowHeight);
        }
            
        private void ResizeGL( int iWindowWidth, int iWindowHeight )
        {
            this.iWindowWidth = iWindowWidth;
            this.iWindowHeight = iWindowHeight;
            
            Gl.glLoadIdentity();
    
            ApplyViewingMatrixes();
        }
        
        private void DrawGL()
        {
            DrawWorld();
            
            Gl.glFinish();
            Gl.glFlush();
            glControl.SwapBuffers();
        }
        
        void CallbackAddName( Entity entity )
        {
            Picker3dController.GetInstance().AddHitTarget( entity );
        }
    
        // maybe change this to publisher/subsriber pattern?
        void DrawRenderable3d()
        {
            WorldView.GetInstance().Render();
            Editing3d.GetInstance().DrawEditBarsToOpenGL();
        }
        
        public void DrawWorld()
        {
            camera = Camera.GetInstance();
            graphics = GraphicsHelperFactory.GetInstance();
            
            Gl.glClear( Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT );
    
            camera.ApplyCamera();    
            DrawRenderable3d();
        }
        
        public void StartMainLoop()
        {
            while( true )
            {
                Invalidate(true);  // note to self: and what if we remove  this line???
                DrawGL();
                Application.DoEvents();
                if( !Created )
                {
                    Console.WriteLine("Renderer window shut down");
                    System.Environment.Exit(0);
                }
                mainloop(); // this is a callback to the rest of the client, such as game logic, and so on
            }
        }    
            
        //! Functions to register callbacks:
        public void RegisterMainLoopCallback(MainLoopDelegate mainloop )
        {
            this.mainloop = mainloop;
        }
    }
}    
