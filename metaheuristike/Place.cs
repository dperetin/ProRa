using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProRa
{
    class Place
    {
	    public Classroom soba;
	    public int i;
	    public int j;
	    public Place(Classroom p, int a, int b)
        {
            soba = p;
            i = a;
            j = b;
            score = 0;
        }
	    public int[] q = {0,0,0,0,0};
	    public int score;

    }
}
