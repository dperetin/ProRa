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
        public Schedule krizaj(Schedule y)
        {
            Schedule raspored = new Schedule();
            foreach (Course c in CourseList)
            {
                Course temp_kolegij = new Course(c.getIsvu(), c.getName(), c.getClassroomType(), c.needsProjector(), c.getID());
                raspored.CourseList.Add(temp_kolegij);
            }
            foreach (Classroom c in ClassroomList)
            {
                Classroom temp = new Classroom(c.getID(), c.getCapacity(), c.getProjector(), c.getType());
                raspored.ClassroomList.Add(temp);
            }
            foreach (Group c in GroupList)
            {
                Group temp = new Group(c.getName(), c.getSize());
                raspored.GroupList.Add(temp);
            }
            foreach (Lecturer c in LecturerList)
            {
                Lecturer temp = new Lecturer(c.getID(), c.getName(), c.getCourses(), c.getType());
                raspored.LecturerList.Add(temp);
            }
            /// TU SMO ///
            Random x = new Random();
            int a = x.Next(0, EventList.Count);
            int b = x.Next(a, EventList.Count);
            foreach (Event c in EventList)
            {
                Event temp = new Event(raspored.findCourse(c.getCourse().getIsvu()), c.getGroups(), c.getDuration(), c.getID(), c.getLType(), c.getCType(), c.getStudentNumber());
                if (a > 0)
                {
                    temp.setPlace(c.getPlace());
                    temp.setLecturer(c.getLecturer());
                    temp.setClassroom(c.getClassroom());
                    if (temp.getClassroom().isAvailable(temp.getPlace().i, temp.getPlace().j, temp.getDuration()))
                    {
                        temp.getClassroom().setEvent(temp.getPlace().i, temp.getPlace().j, temp);
                    }
                    else
                    {
                        //temp.getClassroom().setEvent(temp.getPlace().i, temp.getPlace().j, temp);
                        raspored.hardViolations++;
                    }
                    if (temp.getLecturer().isAvailable(temp.getPlace().i, temp.getPlace().j, temp.getDuration()))
                    {
                        temp.getLecturer().setEvent(temp.getPlace().i, temp.getPlace().j, temp);
                    }
                    else
                    {
                        //temp.getLecturer().setEvent(temp.getPlace().i, temp.getPlace().j, temp);
                        raspored.hardViolations++;
                    }
                    foreach (string g in temp.getGroups())
                    {
                        if (raspored.findGroup(g).isAvailable(temp.getPlace().i, temp.getPlace().j, temp.getDuration()))
                        {
                            raspored.findGroup(g).setEvent(temp.getPlace().i, temp.getPlace().j, temp);
                        }
                        else
                        {
                            //raspored.findGroup(g).setEvent(temp.getPlace().i, temp.getPlace().j, temp);
                            raspored.hardViolations++;
                        }
                    }
                    a--;
                }
                else 
                {
                    temp.setPlace(y.getEventByID(c.getID()).getPlace());
                    temp.setLecturer(y.getEventByID(c.getID()).getLecturer());
                    temp.setClassroom(y.getEventByID(c.getID()).getClassroom());
                    if (temp.getClassroom().isAvailable(temp.getPlace().i, temp.getPlace().j, temp.getDuration()))
                    {
                        temp.getClassroom().setEvent(temp.getPlace().i, temp.getPlace().j, temp);
                    }
                    else
                    {
                        //temp.getClassroom().setEvent(temp.getPlace().i, temp.getPlace().j, temp);
                        raspored.hardViolations++;
                    }
                    if (temp.getLecturer().isAvailable(temp.getPlace().i, temp.getPlace().j, temp.getDuration()))
                    {
                        temp.getLecturer().setEvent(temp.getPlace().i, temp.getPlace().j, temp);
                    }
                    else
                    {
                        //temp.getLecturer().setEvent(temp.getPlace().i, temp.getPlace().j, temp);
                        raspored.hardViolations++;
                    }
                    foreach (string g in temp.getGroups())
                    {
                        if (raspored.findGroup(g).isAvailable(temp.getPlace().i, temp.getPlace().j, temp.getDuration()))
                        {
                            raspored.findGroup(g).setEvent(temp.getPlace().i, temp.getPlace().j, temp);
                        }
                        else
                        {
                            //raspored.findGroup(g).setEvent(temp.getPlace().i, temp.getPlace().j, temp);
                            raspored.hardViolations++;
                        }
                    }
                }
                raspored.EventList.Add(temp);
            }
           /* foreach (Event c in raspored.EventList)
                raspored.UnasignedEvents.Add(c);*/
            // preci po svima i updejtat week

            return raspored;
        }

       /* public Schedule copy()
        {
            Schedule raspored = new Schedule();
            foreach (Course c in CourseList)
            {
                Course temp_kolegij = new Course(c.getIsvu(), c.getName(), c.getClassroomType(), c.needsProjector());
                raspored.CourseList.Add(temp_kolegij);           
            }
            foreach (Classroom c in ClassroomList)
            {
                Classroom temp = new Classroom(c.getID(), c.getCapacity(), c.getProjector(), c.getType());
                raspored.ClassroomList.Add(temp);
            }
            foreach (Group c in GroupList)
            {
                Group temp = new Group(c.getName(), c.getSize());
                raspored.GroupList.Add(temp);
            }
            foreach (Lecturer c in LecturerList)
            {
                Lecturer temp = new Lecturer(c.getID(), c.getName(), c.getCourses(), c.getType());
                raspored.LecturerList.Add(temp);
            }
            foreach (Event c in EventList)
            {
                Event temp = new Event(raspored.findCourse(c.getCourse().getIsvu()), c.getGroups(), c.getDuration(), c.getID(), c.getLType(), c.getCType(), c.getStudentNumber());
                raspored.EventList.Add(temp);
            }
            foreach (Event c in raspored.EventList)
                raspored.UnasignedEvents.Add(c);
            return raspored;

        }*/
        public Schedule deepCopy()
        {

            Schedule raspored = new Schedule();
            raspored.matrica = matrica;
            foreach (Course c in CourseList)
            {
                Course temp_kolegij = new Course(c.getIsvu(), c.getName(), c.getClassroomType(), c.needsProjector(), c.getID());
                raspored.CourseList.Add(temp_kolegij);
            }
            foreach (Classroom c in ClassroomList)
            {
                Classroom temp = new Classroom(c.getID(), c.getCapacity(), c.getProjector(), c.getType());
                raspored.ClassroomList.Add(temp);
            }
            foreach (Group c in GroupList)
            {
                Group temp = new Group(c.getName(), c.getSize());
                raspored.GroupList.Add(temp);
            }
            foreach (Lecturer c in LecturerList)
            {
                Lecturer temp = new Lecturer(c.getID(), c.getName(), c.getCourses(), c.getType());
                raspored.LecturerList.Add(temp);
            }
            foreach (Event c in EventList)
            {
                Event temp = new Event(raspored.findCourse(c.getCourse().getIsvu()), c.getGroups(), c.getDuration(), c.getID(), c.getLType(), c.getCType(), c.getStudentNumber());
                temp.tabu = c.tabu;
                Place p = new Place(c.getPlace().soba, c.getPlace().i, c.getPlace().j);
                temp.setPlace(p);
                temp.setLecturer(raspored.findLecturer(c.getLecturer().getID()));
                temp.setClassroom(raspored.getRoomByID(c.getClassroom().getID()));
                Place a = temp.getPlace();
                temp.getClassroom().setEvent(a.i, a.j, temp);
                temp.getLecturer().setEvent(a.i, a.j, temp);
                temp.getCourse().setEvent(a.i, a.j, temp);
                foreach(string g in temp.getGroups())
                    raspored.findGroup(g).setEvent(a.i, a.j, temp);
                raspored.EventList.Add(temp);
            }
            foreach (Event c in raspored.EventList)
                raspored.UnasignedEvents.Add(c);


            raspored.score = score;
            raspored.view = view;
            return raspored;

        }
       /* public Schedule() { }
        public Schedule(Schedule tmp)
        {
            CourseList = tmp.CourseList;
            ClassroomList = tmp.ClassroomList;
            GroupList = tmp.GroupList;
            LecturerList = tmp.LecturerList;
            EventList = tmp.EventList;
            UnasignedEvents = tmp.UnasignedEvents;
        }*/

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

        public void timetabler()
        {
            int[] w = {1, 1, 10, 20, 20};
            Raspored ra = new Raspored(this);
            while (true)
            {
                foreach (Event it in UnasignedEvents)
                {
                    int studentNo = it.getStudentNumber();
                    Course kolegij = it.getCourse();
                    int t = it.getDuration();

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
                                if (ra.isRoomAvailable(soba.Id, i, j, t) == false)
                                    tmp_score++;
                            }
						      
                    }
                    it.setScore(tmp_score);
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
                min = UnasignedEvents.First().getScore();
                foreach (Event it in UnasignedEvents)
                {
                    if (it.getScore() < min)
                    {
                        min = it.getScore();
                        //minEvent = it;
                    }
                    
                }
                List<Event> minimalniEventi = new List<Event>();
                int brMin = 0;
                foreach (Event it in UnasignedEvents)
                {
                    if (it.getScore() == min)
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

                /*
                 Lecturer *profesor;
        list<Lecturer *>::iterator pi2 = prof.begin();
        int minLoad = (*pi2)->getLoad();
        profesor = *pi2;
        for (; pi2 != prof.end(); pi2++) {
            if ((*pi2)->getLoad() <= minLoad) {
                minLoad = (*pi2)->getLoad();
                profesor = *pi2;
            }
        }*/

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

                //Group grupa = findGroup(*grupe.begin());
                Course k = minEvent.getCourse();

                List<Classroom> predavaone = new List<Classroom>();
                foreach (Classroom j in ClassroomList)
                {
                    if(j.getCapacity() >= minEvent.getStudentNumber() && j.getProjector() >= k.needsProjector() && j.getType() == k.getClassroomType())
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
                            if (tt.isAvailable(i, j, minEvent.getDuration()) == false)
                                continue;
                            if (profesor.isAvailable(i, j, minEvent.getDuration()) == false)
                                continue;
                            int free = 0;
                            foreach (string g in minEvent.getGroups())
                            {
                                if (findGroup(g).isAvailable(i, j, minEvent.getDuration()) == false)
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
						        if (tt.getCapacity() >= it.getStudentNumber() && c.getClassroomType() == tt.getType() && c.needsProjector() == tt.getProjector() )
							        q1++;
					        }
					        tmp.q[0] = q1;

                            int q2 = 0;
					
					        foreach (Classroom ci in predavaone) 
                            {
                                if (ci.isAvailable(i, j, minEvent.getDuration()) == false)
                                    q2++;
					        }
					        tmp.q[1] = q2;

                            int q3 = 0;
                            if (tmp.j + minEvent.getDuration() >= 10)
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
                getEventByID(ID).setPlace(minPlace);
                getEventByID(ID).getCourse().setEvent(minPlace.i, minPlace.j, minEvent);
                Classroom b = getRoomByID(minPlace.soba.getID());

                b.setEvent(minPlace.i, minPlace.j, minEvent);
                profesor.setEvent(minPlace.i, minPlace.j, minEvent);
                profesor.incLoad();
                foreach (string g in minEvent.getGroups())
                {
                    findGroup(g).setEvent(minPlace.i, minPlace.j, minEvent);
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

        public void generateHtml(string s)
        {
	         TextWriter f = new StreamWriter(s);

	       // f.open(s, ifstream::out);

			 f.WriteLine("<html><head><style type='text/css'>body{margin:50px 0px; padding:0px;	text-align:center;}table, td, th{	border: 1px solid black;	border-collapse:collapse;}td{font-size:10pt; text-align:center;}</style></head>");// << "<html>" << endl;
	        f.WriteLine("<body>");// <<  << endl;

	        //list<Group>::iterator it;
        	
	       // for (it = groupList.begin(); it != groupList.end(); it++) {
            foreach(Group it in GroupList) 
            {
		        f.WriteLine("<h3>{0}</h3>",it.getName() );
		        f.WriteLine("<table border='1'>");
		        f.WriteLine("<tr><th WIDTH='80'>SAT</th><th WIDTH='150'>PONEDJELJAK</th><th WIDTH='150'>UTORAK</th><th WIDTH='150'>SRIJEDA</th><th WIDTH='150'>CETVRTAK</th><th WIDTH='150'>PETAK</th></tr>");

		        for (int i = 0; i < 12; i++) {
			        //f << "<tr>"<< "<td WIDTH='70'>" << i + 8 << "<sup>00</sup> - " << i + 9 << "<sup>00</sup>" << "</td>" << endl;
                    f.WriteLine("<tr><th WIDTH='80'>{0}<sup>00</sup> - {1}<sup>00</sup></td>",i + 8,i + 9);
			        for (int j = 0; j < 5; j++) {
				        if (it.week[j,i] == 0)
					        //f << "<td WIDTH='150'>&nbsp;</td>" << endl;
                            f.WriteLine("<td WIDTH='150'>&nbsp;</td>");
				        else {
							string tip;
							

					        Event t = getEventByID(it.week[j,i]);
							if (t.getLType() == 1) tip = "(p)";
							else tip = "(v)";
					        if (i == 0 || it.week[j,i - 1] != it.week[j,i]) {
						        //f << "<td rowspan='" << t->getDuration() << "' WIDTH='150'>" << t->getCourse()->getName() << "  "  << t->soba  << "  "  << t->getLecturer() << "</td>" << endl;
					            f.WriteLine("<td rowspan='{0}' WIDTH='150'>{1} {2} <br /> {3}  {4}</td>",t.getDuration(),t.getCourse().getName(), tip, t.getClassroom().getID(),t.getLecturer().getName());
                            }
				        }
			        }
			        f.WriteLine("</tr>");//f << "</tr>" << endl;
		        }
		        f.WriteLine("</table>");//f << "</table>" << endl;
	        }
	        f.WriteLine("</body>");//f << "</body>" << endl;
	        f.WriteLine("</html>");//f << "</html>" << endl;
	        f.Close();
        }

        public int[] evaluateSchedule()
        {
	        score = 0;
	        double suma = 0;
	        double[] w = {10, 1, 1, 7, 10, 0, 1};
	        int[] s = {0, 0, 0, 0, 0, 0, 0};
	        //list<Group>::iterator it;
	        //for (it = groupList.begin(); it != groupList.end(); it++) {
		     foreach(Group it in GroupList) 
             {
                for (int i = 0; i < 5; i++) {
			        int load = it.dayLoad(i);
			        if (load > 6) 
				        s[0] += (load - 6);
			        //else if (load < 3)
				        //s[0] += load;
			        //s[1] += it.prijeDeset(i);
			        s[2] += it.izaPet(i);
			        s[3] += it.brojRupa(i);
                    //s[5] += it.praznoPrije(i);
		        }
	        }
             
	        foreach(Lecturer l in LecturerList)
            {
                int brojZauaetihDana = 0;
                for(int i = 0; i < 5; i++)
                {
                    brojZauaetihDana += l.imaNastave(i);
                    s[6] += l.brojRupa(i);
                }
                if (brojZauaetihDana > 1)
                s[5] += brojZauaetihDana;
                
            }

           /* for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    foreach (Classroom a in ClassroomList)
                    {
                        foreach (Classroom b in ClassroomList)
                        {
                            string x = a.getID();
                            string y = b.getID();
                            if (x != y)
                            {
                                Event e1 = getEventByID(a.week[i,j]);
                                Event e2 = getEventByID(b.week[i,j]);
                                if(e1 != null && e2!=null)
                                if (matrica[e1.getCourse().getID(), e2.getCourse().getID()] == 0)
                                    s[6]++;
                            }
                        }
                    }
                }
            }*/
            foreach (Course c in CourseList)
            {
                foreach(Course k in CourseList)
                {
                    if (matrica[c.getID(), k.getID()] == 0)
                    {
                        s[4] += c.brojPreklapanja(k);
                                    
                    }
                }
                           
            }
            
            
                

            for (int i = 0; i < 7; i++)
                suma += (w[i] * s[i]);
            score = 1 / (1 + suma);
            return s;


            	
        }
        public string drawGroupSchedule(Group G)
        {
			string s = "<html><head><style type='text/css'>body{margin:50px 0px; padding:0px;	text-align:center;}table, td, th{	border: 1px solid black;	border-collapse:collapse;}td{font-size:10pt; text-align:center;} td.full{background-color=#CCCCFF}</style></head><body><h3>";
            s += (G.getName() + "</h3><table border='2'>");
            s += "<tr><th WIDTH='70'>SAT</th><th WIDTH='150'>PONEDJELJAK</th><th WIDTH='150'>UTORAK</th><th WIDTH='150'>SRIJEDA</th><th WIDTH='150'>CETVRTAK</th><th WIDTH='150'>PETAK</th></tr>";
            for (int i = 0; i < 12; i++)
            {
				s += ("<tr style='height:40px'><td WIDTH='70'>" + (i + 8).ToString() + "<sup>00</sup> - " + (i + 9).ToString() + "<sup>00</sup></td>");
                for (int j = 0; j < 5; j++)
                {
                    if (G.week[j, i] == 0)
                        s += "<td WIDTH='150'>&nbsp;</td>";
                    else
                    {
						string tip;


						Event t = getEventByID(G.week[j, i]);
						if (t.getLType() == 1) tip = "(p)";
						else tip = "(v)";
                        if (i == 0 || G.week[j, i - 1] != G.week[j, i])
                        {
                            s += ("<td class='full' rowspan='" + t.getDuration().ToString() + "' WIDTH='150'>" + t.getCourse().getName() + " " +tip + "<br />" + t.getClassroom().getID() + "  " + t.getLecturer().getName() + "</td>");
                        }

                    }

                }
                s += "</tr>";
            }

            s += "</table></body></html>";
            return s;
        }

        public string drawLecturerSchedule(Lecturer G)
        {
			string s = "<html><head><style type='text/css'>body{margin:50px 0px; padding:0px;	text-align:center;}table, td, th{	border: 1px solid black;	border-collapse:collapse;}td{font-size:10pt; text-align:center;} td.full{background-color=#CCCCFF}</style></head><body><h3>";
            s += (G.getName() + "</h3><table border='2'>");
            s += "<tr><th WIDTH='70'>SAT</th><th WIDTH='150'>PONEDJELJAK</th><th WIDTH='150'>UTORAK</th><th WIDTH='150'>SRIJEDA</th><th WIDTH='150'>CETVRTAK</th><th WIDTH='150'>PETAK</th></tr>";
            for (int i = 0; i < 12; i++)
            {
				s += ("<tr style='height:40px'><td WIDTH='70'>" + (i + 8).ToString() + "<sup>00</sup> - " + (i + 9).ToString() + "<sup>00</sup></td>");
                for (int j = 0; j < 5; j++)
                {
                    if (G.week[j, i] == 0)
                        s += "<td WIDTH='150'>&nbsp;</td>";
                    else
                    {
                        Event t = getEventByID(G.week[j, i]);
                        if (i == 0 || G.week[j, i - 1] != G.week[j, i])
                        {
							s += ("<td class='full' rowspan='" + t.getDuration().ToString() + "' WIDTH='150'>" + t.getCourse().getName() + "<br /> " + t.getClassroom().getID() + "</td>");
                        }

                    }

                }
                s += "</tr>";
            }

            s += "</table></body></html>";
            return s;
        }
        public string drawClassroomSchedule(Classroom G)
        {
			string s = "<html><head><style type='text/css'>body{margin:50px 0px; padding:0px;	text-align:center;}table, td, th{	border: 1px solid black;	border-collapse:collapse;}td{font-size:10pt; text-align:center;} td.full{background-color=#CCCCFF}</style></head><body><h3>";
            s += (G.getID() + "</h3><table border='2'>");
            s += "<tr><th WIDTH='70'>SAT</th><th WIDTH='150'>PONEDJELJAK</th><th WIDTH='150'>UTORAK</th><th WIDTH='150'>SRIJEDA</th><th WIDTH='150'>CETVRTAK</th><th WIDTH='150'>PETAK</th></tr>";








            for (int i = 0; i < 12; i++)
            {
				s += ("<tr style='height:40px'><td WIDTH='70'>" + (i + 8).ToString() + "<sup>00</sup> - " + (i + 9).ToString() + "<sup>00</sup></td>");
                for (int j = 0; j < 5; j++)
                {
                    if (G.week[j, i] == 0)
                        s += "<td WIDTH='150'>&nbsp;</td>";
                    else
                    {
                        Event t = getEventByID(G.week[j, i]);
                        if (i == 0 || G.week[j, i - 1] != G.week[j, i])
                        {
							s += ("<td  class='full'rowspan='" + t.getDuration().ToString() + "' WIDTH='150'>" + t.getCourse().getName() + " " + "<br />" + t.getLecturer().getName() + "</td>");
                        }
























                    }






                }
                s += "</tr>";
            }

            s += "</table></body></html>";
            return s;

        }

        public void tabuSearch()
        {
            Schedule temp = new Schedule();
            temp = this.deepCopy();
            temp.evaluateSchedule();
            //temp.generateHtml("temp.html");
            Schedule best = temp.deepCopy();
            //Console.WriteLine("{0}", temp.getScore());
            while (true)
            {
                //Console.WriteLine("===============");
                Schedule eval = temp.deepCopy();
                foreach (Event e in temp.EventList)
                {
                    int t = e.getDuration();
                    foreach (Classroom c in temp.ClassroomList)
                    {
                        if (c.canHost(e))
                        {
                            for (int i = 0; i < 5; i++)
                            {
                                for (int j = 0; j < 12; j++)
                                {


                                    if (c.isAvailable(i, j, t) && e.getLecturer().isAvailable(i, j, t))
                                    {
                                        bool indikator = false;
                                        foreach (string g in e.getGroups())
                                        {
                                            if (eval.findGroup(g).isAvailable(i, j, t) == false)
                                                indikator = true;
                                        }
                                        if (indikator)
                                            continue;

                                        //TU SMO DAKLE, na eval radim remove
                                        //e.getClassroom().removeEvent(e.getID());
                                        eval.getEventByID(e.getID()).getClassroom().removeEvent(e.getID());
                                        eval.getEventByID(e.getID()).getLecturer().removeEvent(e.getID());
                                        foreach (string g in e.getGroups())
                                        {
                                            eval.findGroup(g).removeEvent(e.getID());
                                        }
                                        //Place p = new Place(eval.getEventByID(e.getID()).getClassroom(), i, j);
                                        Place p = new Place(eval.getRoomByID(c.getID()), i, j);
                                        eval.getEventByID(e.getID()).setPlace(p);
                                        //eval.getEventByID(e.getID()).getClassroom().setEvent(i, j, e);
                                        eval.getRoomByID(c.getID()).setEvent(i, j, e);
                                        eval.getEventByID(e.getID()).getLecturer().setEvent(i, j, e);
                                        eval.getEventByID(e.getID()).setClassroom(eval.getRoomByID(c.getID()));
                                        foreach (string g in e.getGroups())
                                        {
                                            eval.findGroup(g).setEvent(i, j, e);
                                        }
                                        eval.evaluateSchedule();
                                        if (eval.getScore() > best.getScore())
                                        {
                                            best = eval.deepCopy();
                                            //Form1.label1.text = "3";
                                            //Console.WriteLine("{0}", best.getScore());
                                            //best.generateHtml("best.html");
                                        }
                                        eval = temp.deepCopy();
                                    }
                                }
                            }
                        }
                    }

                }
                temp = best.deepCopy();
            }
        }
    }
}
