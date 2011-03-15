using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProRa
{
    class Classroom
    {
        static int Count = 0;
        int countID;
        string ID;
	    int Capacity;
	    int Projector; // 0 nema, 1 ima
	    int Type;      // 0 predavaona, 1 praktikum
        public int[,] week = new int[5, 12];

        public int Id
        {
            get { return countID; }
        }

        public Classroom(string I, int C, int P, int T)
        {
            countID = Count;
            Count++;
            ID = I;
            Capacity = C;
            Projector = P;
            Type = T;

            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 12; j++)
                    week[i, j] = 0;
        }
        public bool canHost(Event e) {
		    if (e.StudentNumber <= Capacity && e.getCType() == Type /*&& e.getCourse().needsProjector() <= Projector*/)
			    return true;
		    return false;
		
	    }
  /*      public bool isAvailable(int i, int j, int t) {
		    int status = 0;
            if (t + j > 12)
                return false;
		    for (int k = 0; k < t; k++)
			    status += week[i, j + k];
		    if (status == 0)
			    return true;
		    else
			    return false;
	    }*/
        public int getCapacity(){return Capacity;}
        public int getProjector() { return Projector; }
        public int getType() { return Type; }
        public string getID() { return ID; }
        public void setEvent(int i, int j, Event e){
		    for (int k = 0; k < e.Duration; k++)
			    week[i, j + k] = e.getID();
	    }
        public void removeEvent(int eventID){
		    for (int i = 0; i < 5; i++)
			    for (int j = 0; j < 12; j++)
				    if (this.week[i,j] == eventID)
					    this.week[i,j] = 0;
	    }
        
    }
}
