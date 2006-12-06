#include <iostream>
#include "windows.h"
#include "gl\GL.h"
using namespace std;

GLuint selectbuffer[4096];

extern "C" void glCreateSelectBuffer()
{
//cout << "makeselectbuffer" << endl;	
   glSelectBuffer( 4096, selectbuffer );
}

extern "C" int glGetNearestBufferName( int inumhits )
{
   unsigned int bestdepth = 0;
   int bestpick = -1;
   for( int i = 0; i < inumhits; i++ )
   {
        unsigned int thisitem = selectbuffer[ i * 4 + 3 ];
        unsigned int thisdepth = selectbuffer[ i * 4 + 1 ];
       if( thisdepth < bestdepth || bestpick == -1 )
       {
         //  cout << "new best depth: " << bestdepth << " pick: " << thisitem << endl;
           bestdepth = thisdepth;
           bestpick = (int)thisitem;
       }
   }
  // cout<< "best hit: " << bestpick << endl;
   return bestpick;
}

/*
extern "C" void glGetSelectBuffer(unsigned int *parray, int numhits )
{
   for( int i = 0; i < 4 * numhits; i++)
   {
cout << selectbuffer[i] << " " << parray[i] << endl;
parray[i] = (unsigned int)selectbuffer[i];
    }
   return;
}
*/

