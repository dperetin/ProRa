using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace ProRa
{
    class Schedule : IComparable
    {
        public List<Course> CourseList = new List<Course>();
        public List<Classroom> ClassroomList = new List<Classroom>();
        public List<Group> GroupList = new List<Group>();
        public List<Lecturer> LecturerList = new List<Lecturer>();
        public List<Event> EventList = new List<Event>();
        List<Event> UnasignedEvents = new List<Event>();
        public int hardViolations = 0;
        double score;
        public double getScore() { return score; }
        public int view = 0;
        Random r = new Random();
        public int[,] matrica = new int[22,22];

        public void populateMatrix(string filename)
        {
            StreamReader myFile = new StreamReader(filename);
            string myString = myFile.ReadToEnd();

            string[] linije = Regex.Split(myString, "\r\n");
            int j = 0;
            foreach (string linija in linije)
            {
                string[] info = linija.Split('\t');
                for (int i = 0; i < this.CourseList.Count; i++)
                {
                    matrica[j, i] = Convert.ToInt32(info[i]);
                    
                }
                j++;
            }
            myFile.Close();
        }

        int IComparable.CompareTo(object obj)
        {
            Schedule c = (Schedule)obj;
            if (this.getScore() > c.getScore())
                return 1;
            if (this.getScore() < c.getScore())
                return -1;
            else
                return 0;
        }
        
        public void populateCourseList(string s)
        {
            int id = 0;
            StreamReader myFile = new StreamReader(s);
            string myString = myFile.ReadToEnd();
            
            string[] linije = Regex.Split(myString,"\r\n");

            foreach (string linija in linije)
            {
                string[] info = linija.Split(' ');
                Course tmp = new Course(info[0], info[3], Convert.ToInt32(info[2]), Convert.ToInt32(info[2]), id);
                id++;
                CourseList.Add(tmp);
                //Console.Write(info[1]);
            }
            myFile.Close();
        }

        public void populateClassroomList(string s)
        {
            StreamReader myFile = new StreamReader(s);
            string myString = myFile.ReadToEnd();
            
            string[] linije = Regex.Split(myString, "\r\n");

            foreach (string linija in linije)
            {
                string[] info = linija.Split(' ');
                Classroom tmp = new Classroom(info[0], Convert.ToInt32(info[1]), Convert.ToInt32(info[2]), Convert.ToInt32(info[3]));
                ClassroomList.Add(tmp);
                //Console.Write(info[1]);
            }
            myFile.Close();
        }

        public void populateGroupList(string s)
        {
            StreamReader myFile = new StreamReader(s);
            string myString = myFile.ReadToEnd();

            string[] linije = Regex.Split(myString, "\r\n");

            foreach (string linija in linije)
            {
                string[] info = linija.Split(' ');
                Group tmp = new Group(info[0], Convert.ToInt32(info[1]));
                GroupList.Add(tmp);
                //Console.Write(info[1]);
            }
            myFile.Close();
        }

        public void populateLecturerList(string s)
        {
            StreamReader myFile = new StreamReader(s);
            string myString = myFile.ReadToEnd();

            string[] linije = Regex.Split(myString, "\r\n");
            int ID = 0;
            foreach (string linija in linije)
            {
                
                string[] info = linija.Split(' ');
                int brKolegija = info.Length - 2;
                List<string> kolegiji = new List<string>();
                for (int i = 0; i < brKolegija; i++)
                {
                    kolegiji.Add(info[2 + i]);
                }

                Lecturer tmp = new Lecturer(ID, info[0], kolegiji, Convert.ToInt32(info[1]));
                ID++;
                LecturerList.Add(tmp);
                //Console.Write(info[1]);
            }
            myFile.Close();
        }

        public void populateEventList(string s)
        {
            StreamReader myFile = new StreamReader(s);
            string myString = myFile.ReadToEnd();

            string[] linije = Regex.Split(myString, "\r\n");
            int ID = 1;
            foreach (string linija in linije)
            {
                
                string[] info = linija.Split(' ');
                int brKolegija = info.Length - 4;
                List<string> grupe = new List<string>();
                for (int i = 0; i < brKolegija; i++)
                {
                    grupe.Add(info[4 + i]);
                }

                int sNo = 0;
                foreach (string g in grupe)
                {
                    sNo += findGroup(g).getSize();
                }

                Course tmpCourse = findCourse(info[0]);

                Event tmp = new Event(tmpCourse, grupe, Convert.ToInt32(info[1]), ID, Convert.ToInt32(info[2]), Convert.ToInt32(info[3]), sNo);
                ID++;
                EventList.Add(tmp);
                //Console.Write(info[1]);
            }
            myFile.Close();
            //UnasignedEvents = EventList;
            foreach (Event f in EventList)
                UnasignedEvents.Add(f);
        }

        public Group findGroup(string GroupName)
        {
            foreach (Group g in GroupList)
            {
                if (g.getName() == GroupName)
                    return g;
            }
            return null;
        }

        public Course findCourse(string CourseName)
        {
            foreach (Course c in CourseList)
            {
                if (c.getIsvu() == CourseName)
                    return c;
            }
            return null;
        }

        public Raspored timetabler()
        {
            int[] w = {1, 1, 10, 20, 20};
            Raspored ra = new Raspored(this);
            while (true)
            {
                foreach (Event it in UnasignedEvents)
                {
                    int studentNo = it.StudentNumber;
                    Course kolegij = it.getCourse();
                    int t = it.Duration;

                    int score = 0;

                    List<Classroom> sobe = new List<Classroom>();
                    foreach (Classroom j in ClassroomList)
                    {
                        if (j.canHost(it))
                            sobe.Add(j);
                    }

                    int tmp_score = 0;
                    foreach (Classroom soba in sobe)
                    {
                        //int[,] week = new int[5,12];
                        
				        for (int i = 0; i < 5; i ++)
                            for (int j = 0; j < 12; j++)
                            {
                                // za sad samo brojim koliko soba ima slobodnih termina!!
                                if (ra.IsRoomAvailable(soba.Id, i, j, t) == false)
                                    tmp_score++;
                            }
						      
                    }
                    it.Score = tmp_score;
                }
                int min = 0;// = 100000;
                Event minEvent = null;// = new Event();

                /*minEvent = UnasignedEvents.First();
                min = minEvent.getScore();
                foreach (Event it in UnasignedEvents)
                {
                    
                    if (it.getScore() < min)
                    {
                        min = it.getScore();
                        minEvent = it;
                    }
                }*/
                //////////////////////
                min = UnasignedEvents.First().Score;
                foreach (Event it in UnasignedEvents)
                {
                    if (it.Score < min)
                    {
                        min = it.Score;
                        //minEvent = it;
                    }
                    
                }
                List<Event> minimalniEventi = new List<Event>();
                int brMin = 0;
                foreach (Event it in UnasignedEvents)
                {
                    if (it.Score == min)
                    {
                        brMin++;
                        minimalniEventi.Add(it);
                    }
                }
                Random random = new Random();
                int slBroj = random.Next(0, brMin);

                foreach(Event it in minimalniEventi)
                {
                    if (slBroj == 0)
                    {
                        minEvent = it;
                    }
                    slBroj--;
                }
                //////////////////////

                


                string isvu = minEvent.getCourse().getIsvu();

                List<Lecturer> prof = new List<Lecturer>();

                foreach (Lecturer pi in LecturerList)
                {
                    if (pi.teaches(isvu) && pi.getType() == minEvent.getLType())
                        prof.Add(pi);
                }

                Lecturer profesor = null;
                int minLoad = 0;
                profesor = prof.First();
                minLoad = profesor.getLoad();
                foreach (Lecturer pi in prof)
                {
                    
                    if (pi.getLoad() <= minLoad)
                    {
                        minLoad = pi.getLoad();
                        profesor = pi;
                    }
                }
                getEventByID(minEvent.getID()).setLecturer(findLecturer(profesor.getID()));

                ra.SetLecturer(minEvent.getID(), profesor.getID());

                
                Course k = minEvent.getCourse();

                List<Classroom> predavaone = new List<Classroom>();
                foreach (Classroom j in ClassroomList)
                {
                    //if(j.getCapacity() >= minEvent.StudentNumber && j.getProjector() >= k.needsProjector() && j.getType() == k.getClassroomType())
                    if(j.canHost(minEvent))
                    {
                        predavaone.Add(j);
                    }
                }

                List<Place> mjesta = new List<Place>();

                foreach (Classroom tt in predavaone)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        for (int j = 0; j < 12; j++)
                        {
                            if (ra.IsRoomAvailable(tt.Id, i, j, minEvent.Duration) == false)
                                continue;
                            if (ra.IsLecturerAvailable(profesor.Id, i, j, minEvent.Duration) == false)
                                continue;
                            int free = 0;
                            foreach (string g in minEvent.getGroups())
                            {
                                if (ra.IsGroupAvailable(findGroup(g).Id, i, j, minEvent.Duration) == false)
                                {
                                    free++;
                                }
                                
                            }
                            if (free != 0)
                                continue;
                            Place tmp = new Place(tt, i, j);
                            //string s = "d";

                            int q1 = 0;
					
					        foreach (Event it in UnasignedEvents){
						    //Group *g = findGroup(it->getGroupName());
						    
						        Course c = it.getCourse();
						        if (tt.getCapacity() >= it.StudentNumber && c.getClassroomType() == tt.getType() && c.needsProjector() == tt.getProjector() )
							        q1++;
					        }
					        tmp.q[0] = q1;

                            int q2 = 0;
					
					        foreach (Classroom ci in predavaone) 
                            {
                                if (ra.IsRoomAvailable(ci.Id, i, j, minEvent.Duration) == false)
                                    q2++;
					        }
					        tmp.q[1] = q2;

                            int q3 = 0;
                            if (tmp.j + minEvent.Duration >= 10)
                                q3 = 1;
                            tmp.q[2] = q3;

                            int q4 = 0;
                            tmp.q[3] = q4;

                            int q5 = 0;
                            tmp.q[4] = q5;

					        /*for (int i = 0; i < 11; i++) {
						if (grupa->week[in][i] != 0 && grupa->week[in][i] != grupa->week[in][i + 1])
							q4++;
					}
					tmp.q[3] = q4;*/
                            for (int ia = 0; ia < 5; ia++)
                                tmp.score += (tmp.q[ia] * w[ia]);

                            mjesta.Add(tmp);
                        }
                    }
                }
                Place minPlace = mjesta.First();
                int mini = minPlace.score; // ovo nam treba :=
                /*foreach(Place it in mjesta)
                {
                    if (it.score < mini)
                    {
                        mini = it.score;
                        minPlace = it;
                    }
                }*/

                slBroj = random.Next(0, mjesta.Count-1);

                foreach (Place it in mjesta)
                {
                    if (slBroj == 0)
                    {
                       // mini = it.score;
                        minPlace = it;
                    }
                    slBroj--;
                }
                /////
                int ID = minEvent.getID();
                getEventByID(ID).setClassroom(getRoomByID(minPlace.soba.getID()));
               /* getEventByID(ID).setPlace(minPlace);*/
                getEventByID(ID).getCourse().setEvent(minPlace.i, minPlace.j, minEvent);

                ra.SetClassroom(ID, minPlace.soba.Id);


                Classroom b = getRoomByID(minPlace.soba.getID());

               // b.setEvent(minPlace.i, minPlace.j, minEvent);
                ra.SetClassroomEvent(b.Id, minPlace.i, minPlace.j, minEvent.Duration, minEvent.getID());
               // profesor.setEvent(minPlace.i, minPlace.j, minEvent);
                ra.SetLecturerEvent(profesor.Id, minPlace.i, minPlace.j, minEvent.Duration, minEvent.getID());
                profesor.incLoad();
                foreach (string g in minEvent.getGroups())
                {
                    //findGroup(g).setEvent(minPlace.i, minPlace.j, minEvent);
                    ra.SetGroupEvent(findGroup(g).Id, minPlace.i, minPlace.j, minEvent.Duration, minEvent.getID());
                }
                UnasignedEvents.Remove(minEvent);

                if (UnasignedEvents.Count == 0)
                    break;
               /* 
		

		list<Event>::iterator ita;
		for (ita = unasignedEvents.begin(); ita != unasignedEvents.end(); ita++)
		{
			if (ita->getID() == ID) {
				unasignedEvents.erase(ita);
				break;
			}
		}
		if(unasignedEvents.empty())
			break;*/
      

            }
            return ra;
        }

        public Event getEventByID(int ID) 
        {
            foreach (Event it in EventList)
            {
                if (it.getID() == ID)
                    return it;
            }
	        return null;
        }

        public Lecturer findLecturer(int sifra)
        {
	        //list<Lecturer>::iterator i;
	        foreach (Lecturer i in LecturerList)
		        if (i.getID() == sifra)
			        return i;
	        return null;
        }

        public Classroom getRoomByID(string ID) 
        {
	        foreach(Classroom it in ClassroomList)

        		if (it.getID() == ID)
			        return it;
	        return null;
        }
        public Classroom getRoomByNo(int ID)
        {
            foreach (Classroom it in ClassroomList)

                if (it.Id == ID)
                    return it;
            return null;
        }
       
        public Group GetGroupById(int i)
        {
            foreach (Group g in GroupList)
                if (g.Id == i)
                    return g;
            return null;
        }
    }
}
