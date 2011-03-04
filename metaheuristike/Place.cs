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
    struct raspored
    {
        int[, ,] sobe;
        int[, ,] profesori;
        int[, ,] kolegiji;
        int[, ,] grupe;
        int[,] eventi;

        public raspored(Schedule podaci)
        {
            sobe = new int[podaci.ClassroomList.Count,5,12];
            profesori = new int[podaci.LecturerList.Count, 5, 12];
            kolegiji = new int[podaci.CourseList.Count, 5, 12];
            grupe = new int[podaci.GroupList.Count, 5, 12];
            eventi = new int[podaci.EventList.Count, 3];
        }
    }
}
