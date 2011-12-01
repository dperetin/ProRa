using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

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
        //int[, ,] kolegiji;
        int[,] eventi;
        double score;
        public int view;

        public Raspored(Raspored r)
        {
            sobe = (int[,,])r.sobe.Clone();
            profesori = (int[, ,])r.profesori.Clone();
            kolegiji = (int[, ,])r.kolegiji.Clone();
            grupe = (int[, ,])r.grupe.Clone();
            eventi = (int[, ])r.eventi.Clone();
            //kolegiji = (int[,])r.eventi.Clone();
            score = r.score;
            view = r.view;
        }

        public double Score
        {
            get { return score; }
        }

        public Raspored(Schedule podaci)
        {
            sobe = new int[podaci.ClassroomList.Count,5,12];
            profesori = new int[podaci.LecturerList.Count, 5, 12];
            kolegiji = new int[podaci.CourseList.Count, 5, 12];
            grupe = new int[podaci.GroupList.Count, 5, 12];
            eventi = new int[podaci.EventList.Count + 1, 4];
            score = 0;
            view = 0;
        }
        public int brojNula(int id, int day)
        {
            int load = 0;
            for (int i = 0; i < 12; i++)
                if (grupe[id, day, i] == 0)
                    load++;
                else return load;
            return load;
        }
        public bool IsRoomAvailable(int roomId, int i, int j, int t)
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
        public int brojPreklapanja(int k, int c)
        {
            
            int r = 0;
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    if (kolegiji[k, i, j] > 0 && kolegiji[c, i, j] > 0)
                        r++;
                }
            }
            return r;
        }
        public bool IsLecturerAvailable(int lecId, int i, int j, int t)
        {
            int status = 0;
            if (t + j > 12)
                return false;
            for (int k = 0; k < t; k++)
                status += profesori[lecId, i, j + k];
            if (status == 0)
                return true;
            else
                return false;
        }

        public bool IsGroupAvailable(int grpId, int i, int j, int t)
        {
            int status = 0;
            if (t + j > 12)
                return false;
            for (int k = 0; k < t; k++)
                status += grupe[grpId, i, j + k];
            if (status == 0)
                return true;
            else
                return false;
        }

        public void SetLecturer(int EventId, int LecturerId) 
        {
            eventi[EventId, 0] = LecturerId;
        }
        public void SetClassroom(int EventId, int ClassroomId)
        {
            eventi[EventId, 1] = ClassroomId;
        }
        public void SetClassroomEvent(int ClassroomId, int i, int j, int t, int EventId)
        {
            eventi[EventId, 2] = i;
            eventi[EventId, 3] = j;
            for (int k = 0; k < t; k++)
                sobe[ClassroomId, i, j + k] = EventId;
        }
        public void SetLecturerEvent(int LecId, int i, int j, int t, int EventId)
        {
            for (int k = 0; k < t; k++)
                profesori[LecId, i, j + k] = EventId;
        }
        public void SetGroupEvent(int GroupId, int i, int j, int t, int EventId)
        {
            for (int k = 0; k < t; k++)
                grupe[GroupId, i, j + k] = EventId;
        }
        public void SetCourseEvent(int CourseId, int i, int j, int t, int eventId)
        {
            for (int k = 0; k < t; k++)
                kolegiji[CourseId, i, j + k]++;
        }
        
        public void generateHtml(string s, Schedule r)
        {
            TextWriter f = new StreamWriter(s);

            // f.open(s, ifstream::out);

            f.WriteLine("<html><head><style type='text/css'>body{margin:50px 0px; padding:0px;	text-align:center;}table, td, th{	border: 1px solid black;	border-collapse:collapse;}td{font-size:10pt; text-align:center;}</style></head>");// << "<html>" << endl;
            f.WriteLine("<body>");// <<  << endl;

            //list<Group>::iterator it;

            // for (it = groupList.begin(); it != groupList.end(); it++) {
            for (int id = 0; id < r.GroupList.Count; id++)
            {
                Group it = r.GetGroupById(id);
                f.WriteLine("<h3>{0}</h3>", it.getName());
                f.WriteLine("<table border='1'>");
                f.WriteLine("<tr><th WIDTH='80'>SAT</th><th WIDTH='150'>PONEDJELJAK</th><th WIDTH='150'>UTORAK</th><th WIDTH='150'>SRIJEDA</th><th WIDTH='150'>CETVRTAK</th><th WIDTH='150'>PETAK</th></tr>");

                for (int i = 0; i < 12; i++)
                {
                    //f << "<tr>"<< "<td WIDTH='70'>" << i + 8 << "<sup>00</sup> - " << i + 9 << "<sup>00</sup>" << "</td>" << endl;
                    f.WriteLine("<tr><th WIDTH='80'>{0}<sup>00</sup> - {1}<sup>00</sup></td>", i + 8, i + 9);
                    for (int j = 0; j < 5; j++)
                    {
                        if (grupe[id, j, i] == 0)
                            //f << "<td WIDTH='150'>&nbsp;</td>" << endl;
                            f.WriteLine("<td WIDTH='150'>&nbsp;</td>");
                        else
                        {
                            string tip;


                            Event t = r.getEventByID(grupe[id, j, i]);
                            if (t.getLType() == 1) tip = "(p)";
                            else tip = "(v)";
                            if (i == 0 || grupe[id, j, i - 1] != grupe[id, j, i])
                            {
                                //f << "<td rowspan='" << t->getDuration() << "' WIDTH='150'>" << t->getCourse()->getName() << "  "  << t->soba  << "  "  << t->getLecturer() << "</td>" << endl;
                                f.WriteLine("<td rowspan='{0}' WIDTH='150'>{1} {2} <br /> {3}  {4}</td>", t.Duration, t.getCourse().getName(), tip, eventi[t.getID(), 1], r.findLecturer(eventi[t.getID(), 0]).getName());
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
        public int[] evaluateSchedule( Schedule r)
        {
            score = 0;
            double suma = 0;
            double[] w = { 2, 1, 1, 7, 10};
            int[] s = { 0, 0, 0, 0, 0};
            //list<Group>::iterator it;
            //for (it = groupList.begin(); it != groupList.end(); it++) {
            //foreach (Group it in GroupList)
            for (int id = 0; id < r.GroupList.Count; id++)
            {
                for (int i = 0; i < 5; i++)
                {
                    int load = GroupDayLoad(id, i);
                    if (load > 6)
                        s[0] += (load - 6);
                   // else if (load < 3)
                    //s[0] += load;
                    s[1] += brojNula(id, i);
                    s[2] += IzaPet(id, i);
                    s[3] += GroupBrojRupa(id, i);
                    //s[5] += it.praznoPrije(i);
                }
            }

/*            foreach (Lecturer l in LecturerList)
            {
                int brojZauaetihDana = 0;
                for (int i = 0; i < 5; i++)
                {
                    brojZauaetihDana += l.imaNastave(i);
                    s[6] += l.brojRupa(i);
                }
                if (brojZauaetihDana > 1)
                    s[5] += brojZauaetihDana;

            }*/

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
            for(int  c = 0; c < r.CourseList.Count; c++)
            {
                for(int  k = c+1; k < r.CourseList.Count; k++)
                {
                    if (r.matrica[c, k] == 0)
                    {
                        s[4] += brojPreklapanja(c, k);

                    }
                }

            }




            for (int i = 0; i < 5; i++)
                suma += (w[i] * s[i]);
            score = 1 / (1 + suma);
            return s;



        }
        public int IzaPet(int id, int day)
        {
            int load = 0;
            for (int i = 9; i < 12; i++)
                if (grupe[id, day, i] != 0)
                    load++;
            return load;
        }
        public int GroupDayLoad(int id, int day)
        {
            int load = 0;
            for (int i = 0; i < 12; i++)
                if (grupe[id, day, i] != 0)
                    load++;
            return load;
        }
        public int GroupBrojRupa(int id, int day)
        {
            int rupe = 0, prvi = 0, zadnji = 0, suma = 0;
            for (int i = 0; i < 12; i++)
                if (grupe[id, day, i] != 0)
                {
                    prvi = i;
                    break;
                }
            for (int i = 0; i < 12; i++)
                if (grupe[id, day, i] != 0)
                {
                    zadnji = i;
                    suma++;
                }
            rupe = (zadnji - prvi) - suma + 1;
            return rupe;
        }


        /* Removes an Event from Scedules of all the groups, lecturers
         * and classrooms assosciated with that event */ 
        public void RemoveEvent(Schedule s, int EventId)
        {
            Event e = s.getEventByID(EventId);
            //e.getClassroom().week[]--;
            int t = e.Duration;
            int i = eventi[EventId, 2]; 
            int j = eventi[EventId, 3];
            int c = eventi[EventId, 1];
            int l = eventi[EventId, 0];
            
            foreach (string g in s.getEventByID(EventId).getGroups())
            {
                int gId = s.findGroup(g).Id;
                for (int k = 0; k < t; k++)
                {
                    grupe[gId, i, j + k] = 0;
                }
            }
            for (int k = 0; k < t; k++)
            {
                sobe[c, i, j + k] = 0;
            }
            for (int k = 0; k < t; k++)
            {
                profesori[l, i, j + k] = 0;
            }
            for (int k = 0; k < t; k++)
            {
                kolegiji[e.getCourse().getID(), i, j + k] --;
            }
            //e.getCourse().removeEvent(i, j, t);
            
            eventi[EventId, 2] = 0;
            eventi[EventId, 3] = 0;
        }
        public void SetEvent(Schedule s, int EventId, int classroomId, int i, int j)
        {
            Event e = s.getEventByID(EventId);
            int t = e.Duration;
            eventi[EventId, 2] = i;
            eventi[EventId, 3] = j;
            eventi[EventId, 1] = classroomId;
            int l = eventi[EventId, 0];

            foreach (string g in s.getEventByID(EventId).getGroups())
            {
                int gId = s.findGroup(g).Id;
                for (int k = 0; k < t; k++)
                {
                    grupe[gId, i, j + k] = EventId;
                }
            }
            for (int k = 0; k < t; k++)
            {
                sobe[classroomId, i, j + k] = EventId;
            }
            for (int k = 0; k < t; k++)
            {
                profesori[l, i, j + k] = EventId;
            }
            //e.getCourse().setEvent(i, j, e);
            for (int k = 0; k < t; k++)
            {
                kolegiji[e.getCourse().getID(), i, j + k]++;
            }
        }
        public string drawGroupSchedule(int g, Schedule p)
        {
            Group G = p.GetGroupById(g);
            string s = "<html><head><style type='text/css'>body{margin:50px 0px; padding:0px;	text-align:center;}table, td, th{	border: 1px solid black;	border-collapse:collapse;}td{font-size:10pt; text-align:center;} td.full{background-color=#CCCCFF}</style></head><body><h3>";
            s += (G.getName() + "</h3><table border='2'>");
            s += "<tr><th WIDTH='70'>SAT</th><th WIDTH='150'>PONEDJELJAK</th><th WIDTH='150'>UTORAK</th><th WIDTH='150'>SRIJEDA</th><th WIDTH='150'>CETVRTAK</th><th WIDTH='150'>PETAK</th></tr>";
            for (int i = 0; i < 12; i++)
            {
                s += ("<tr style='height:40px'><td WIDTH='70'>" + (i + 8).ToString() + "<sup>00</sup> - " + (i + 9).ToString() + "<sup>00</sup></td>");
                for (int j = 0; j < 5; j++)
                {
                    if (grupe[g, j, i] == 0)
                        s += "<td WIDTH='150'>&nbsp;</td>";
                    else
                    {
                        string tip;


                        Event t = p.getEventByID(grupe[g, j, i]);
                        if (t.getLType() == 1) tip = "(p)";
                        else tip = "(v)";
                        if (i == 0 || grupe[g, j, i - 1] != grupe[g, j, i])
                        {
                            s += ("<td class='full' rowspan='" + t.Duration.ToString() + "' WIDTH='150'>" + t.getCourse().getName() + " " + tip + "<br />" + p.getRoomByNo(eventi[t.Id, 1]).getID()/*t.getClassroom().getID()*/ + "  " + p.findLecturer(eventi[t.Id, 0]).getName()/*t.getLecturer().getName()*/ + "</td>");
                        }

                    }

                }
                s += "</tr>";
            }

            s += "</table></body></html>";
            return s;
        }

        public string drawLecturerSchedule(int g, Schedule p)
        {
            Lecturer G = p.findLecturer(g);
            string s = "<html><head><style type='text/css'>body{margin:50px 0px; padding:0px;	text-align:center;}table, td, th{	border: 1px solid black;	border-collapse:collapse;}td{font-size:10pt; text-align:center;} td.full{background-color=#CCCCFF}</style></head><body><h3>";
            s += (G.getName() + "</h3><table border='2'>");
            s += "<tr><th WIDTH='70'>SAT</th><th WIDTH='150'>PONEDJELJAK</th><th WIDTH='150'>UTORAK</th><th WIDTH='150'>SRIJEDA</th><th WIDTH='150'>CETVRTAK</th><th WIDTH='150'>PETAK</th></tr>";
            for (int i = 0; i < 12; i++)
            {
                s += ("<tr style='height:40px'><td WIDTH='70'>" + (i + 8).ToString() + "<sup>00</sup> - " + (i + 9).ToString() + "<sup>00</sup></td>");
                for (int j = 0; j < 5; j++)
                {
                    if (profesori[g, j, i] == 0)
                        s += "<td WIDTH='150'>&nbsp;</td>";
                    else
                    {
                        Event t = p.getEventByID(profesori[g, j, i]);
                        if (i == 0 || profesori[g, j, i - 1] != profesori[g, j, i])
                        {
                            s += ("<td class='full' rowspan='" + t.Duration.ToString() + "' WIDTH='150'>" + t.getCourse().getName() + "<br /> " + p.getRoomByNo(eventi[t.Id, 1]).getID()/*t.getClassroom().getID()*/ + "</td>");
                        }

                    }

                }
                s += "</tr>";
            }

            s += "</table></body></html>";
            return s;
        }
        public string drawClassroomSchedule(int g, Schedule p)
        {
            Classroom G = p.getRoomByNo(g);
            string s = "<html><head><style type='text/css'>" + 
                       "body{margin:50px 0px; padding:0px;	text-align:center;}" + 
                       "table, td, th{	border: 1px solid black;	border-collapse:collapse;}" + 
                       "td{font-size:10pt; text-align:center;}" + 
                       " td.full{background-color=#CCCCFF}</style></head><body><h3>";
            s += (G.getID() + "</h3><table border='2'>");
            s += "<tr><th WIDTH='70'>SAT</th><th WIDTH='150'>PONEDJELJAK</th>" + 
                 "<th WIDTH='150'>UTORAK</th><th WIDTH='150'>SRIJEDA</th>" + 
                 "<th WIDTH='150'>CETVRTAK</th><th WIDTH='150'>PETAK</th></tr>";

            for (int i = 0; i < 12; i++)
            {
                s += ("<tr style='height:40px'><td WIDTH='70'>" + 
                     (i + 8).ToString() + "<sup>00</sup> - " + 
                     (i + 9).ToString() + "<sup>00</sup></td>");

                for (int j = 0; j < 5; j++)
                {
                    if (sobe[g, j, i] == 0)
                        s += "<td WIDTH='150'>&nbsp;</td>";
                    else
                    {
                        Event t = p.getEventByID(sobe[g, j, i]);
                        if (i == 0 || sobe[g, j, i - 1] != sobe[g, j, i])
                        {
                            s += ("<td  class='full'rowspan='" + 
                                 t.Duration.ToString() + "' WIDTH='150'>" + 
                                 t.getCourse().getName() + " " + "<br />" + 
                                 p.findLecturer(eventi[t.Id, 0]).getName()/*t.getLecturer().getName()*/ + 
                                 "</td>");
                        }
                    }
                }
                s += "</tr>";
            }
            s += "</table></body></html>";
            return s;
        }
    }
}
