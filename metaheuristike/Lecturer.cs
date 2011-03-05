using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProRa
{
    class Lecturer
    {
        int ID;
	    string Name;
	    List<string> Kolegiji = new List<string>();
	    int Type; //profesor 1, asistent 0
	    int Load;
        public int[,] week = new int[5, 12];

        public Lecturer(int I, string N, List<string> K, int T)
        {
            ID = I;
            Name = N;
            Kolegiji = K;
            Type = T;
            Load = 0;
        }
        public bool teaches(string isvu) 
	    {
		    //list<string>::iterator it;
            foreach (string s in Kolegiji)
            {
                if (isvu == s)
                    return true;
            }
            return false;
		    
	    }
        public List<String> getCourses(){return Kolegiji;}
        public int getType() { return Type; }
        public int getLoad() { return Load; }
        public int getID() { return ID; }
   /*     public bool isAvailable(int i, int j, int t)
        {
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
        public void setEvent(int i, int j, Event e)
        {
            for (int k = 0; k < e.getDuration(); k++)
                week[i, j + k] = e.getID();
        }
        public void incLoad() { Load++; }
        public string getName(){return Name;}
        public void removeEvent(int eventID)
        {
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 12; j++)
                    if (this.week[i, j] == eventID)
                        this.week[i, j] = 0;
        }
        public int imaNastave(int i)
        {
            for (int j = 0; j < 12; j++)
            {
                if (week[i, j] != 0)
                    return 1;
                
            }
            return 0;
        }
        public int brojRupa(int day)
        {
            int rupe = 0, prvi = 0, zadnji = 0, suma = 0;
            for (int i = 0; i < 12; i++)
                if (week[day, i] != 0)
                {
                    prvi = i;
                    break;
                }
            for (int i = 0; i < 12; i++)
                if (week[day, i] != 0)
                {
                    zadnji = i;
                    suma++;
                }
            rupe = (zadnji - prvi) - suma + 1;
            return rupe;
        }

    }
}
