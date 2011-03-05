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
    struct Raspored
    {
        int[, ,] sobe;
        int[, ,] profesori;
        int[, ,] kolegiji;
        int[, ,] grupe;
        int[,] eventi;

        public Raspored(Schedule podaci)
        {
            sobe = new int[podaci.ClassroomList.Count,5,12];
            profesori = new int[podaci.LecturerList.Count, 5, 12];
            kolegiji = new int[podaci.CourseList.Count, 5, 12];
            grupe = new int[podaci.GroupList.Count, 5, 12];
            eventi = new int[podaci.EventList.Count, 3];
        }

        public bool isRoomAvailable(int roomId, int i, int j, int t)
        {
            int status = 0;
            if (t + j > 12)
                return false;
            for (int k = 0; k < t; k++)
                status += sobe[roomId, i, j + k];
            if (status == 0)
                return true;
            else
                return false;
        }

        public void setLecturer(int EventId, int LecturerId) 
        {
            eventi[EventId, 0] = LecturerId;
        }
    }
}
