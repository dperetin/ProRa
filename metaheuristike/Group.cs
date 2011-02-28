using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProRa
{
    class Group
    {
        string Name;
	    int Size;
        public int[,] week = new int[5, 12];

        public Group(string N, int S)
        {
            Name = N;
            Size = S;

            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 12; j++)
                    week[i, j] = 0;
        }
        public string getName(){return Name;}
        public int getSize(){return Size;}
        public bool isAvailable(int i, int j, int t)
        {
            int status = 0;
            if (t + j > 13)
                return false;
            for (int k = 0; k < t; k++)
                status += week[i, j + k];
            if (status == 0)
                return true;
            else
                return false;
        }
        public void setEvent(int i, int j, Event e)
        {
            for (int k = 0; k < e.getDuration(); k++)
                week[i, j + k] = e.getID();
        }
        public void removeEvent(int eventID)
        {
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 12; j++)
                    if (this.week[i, j] == eventID)
                        this.week[i, j] = 0;
        }
        public int praznoPrije(int day)
        {
            int ret = 0;
            for(int j = 0; j < 12; j++)
            {
                if(week[day, j]==0)
                    ret++;
                else
                    return ret;
            }
            return ret;
        }
        public int brojRupa(int day)
        {
            int rupe = 0, prvi = 0, zadnji = 0, suma = 0;
		    for (int i = 0; i < 12; i++)
			    if (week[day, i] != 0) {
				    prvi = i;
				    break;
			    }
		    for (int i = 0; i < 12; i++)
			    if (week[day, i] != 0) {
				    zadnji = i;
				    suma++;
			    }
		    rupe = (zadnji - prvi) - suma + 1;		
		    return rupe;
	    }
        public int izaPet(int day)
        {
            int load = 0;
            for (int i = 9; i < 12; i++)
                if (week[day,i] != 0)
                    load++;
            return load;
        }
        public int dayLoad(int day)
        {
            int load = 0;
            for (int i = 0; i < 12; i++)
                if (week[day, i] != 0)
                    load++;
            return load;
        }
        public int prijeDeset(int day)
        {
            int load = 0;
            for (int i = 0; i < 2; i++)
                if (week[day,i] != 0)
                    load++;
            return load;
        }
    }
}
