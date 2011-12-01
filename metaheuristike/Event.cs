using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProRa
{
    class Event
    {
        Place termin;
	    int eventID;
	    Course kolegij;
	    List<string> grupe;
	    int duration;
        int f;
        public int F {
            get { return f; }
            set { f = value; }
        }
        public int tabu = 0;
	    Classroom C;

        public int Duration
        {
            get { return duration; }
        }

        public int Id
        {
            get { return eventID; }
        }

	    int score;
	    int L_type;
	    int C_type = 0;
	    int studentNumber;

        public int StudentNumber
        {
            get { return studentNumber;}
        }
	    Lecturer Lec;
        public Event(Course K, List<string> G, int Trajanje, int id, int L, int C, int sNo)
        {
            kolegij = K;
            grupe = G;
            duration = Trajanje;
            eventID = id;
            L_type = L;
            C_type = C;
            studentNumber = sNo;
        }
        public void setPlace(Place p) { termin = p; }
        public Place getPlace() { return termin; }
        //public int getStudentNumber(){return studentNumber;}
        public Course getCourse (){return kolegij;}
        public int getCType(){return C_type;}
        //public int getDuration(){return Duration;}
        //public void setScore(int n){score = n;}
        //public int getScore(){return score;}
        public int getLType(){return L_type;}
        public int getID() { return eventID; }
        public void setLecturer(Lecturer l) { Lec = l; }
        public void setClassroom(Classroom c) { C = c; }
        public List<string> getGroups() { return grupe; }
        public Lecturer getLecturer(){return Lec;}
	
	    public Classroom getClassroom(){return C;}

        public int Score 
        {
            get { return score; }
            set { score = value; }
        }

    }
}
