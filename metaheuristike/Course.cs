using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProRa
{
    class Course
    {
        string Name;
        string Isvu;
        int ClassroomType;
        int Projector;
        //int LecturerID;
        int CourseID;
        //static int ukupnoKolegija = 0;
        public int[,] week = new int[5, 12];


        

        public Course(string I, string N, int C, int P, int id)
        {
            CourseID = id;
            //ukupnoKolegija++;
            Name = N;
            Isvu = I;
            ClassroomType = C;
            Projector = P;
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 12; j++)
                    week[i, j] = 0;
        }
        public string getIsvu() {return Isvu;}
        public int needsProjector(){return Projector;}
        public int getClassroomType(){return ClassroomType;}
        public string getName(){return Name;}
        public int getID() { return CourseID; }
        public int brojPreklapanja(Course k) 
        {
            int r = 0;
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    if (week[i, j] > 0 && k.week[i, j] > 0)
                        r++;
                }
            }
            return r;
        }
        public void setEvent(int i, int j, Event e)
        {
            for (int k = 0; k < e.getDuration(); k++)
                week[i, j + k]++;
        }
        public void removeEvent(int i, int j, int t)
        {
            for (int k= 0; k < t; k++)
                
                        this.week[i, j + k]--;
        }
  /*      public bool isAvailable(int i, int j, int t)
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
        }*/
    }
}
