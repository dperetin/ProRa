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
        int[,] eventi;

        public Raspored(Schedule podaci)
        {
            sobe = new int[podaci.ClassroomList.Count,5,12];
            profesori = new int[podaci.LecturerList.Count, 5, 12];
            kolegiji = new int[podaci.CourseList.Count, 5, 12];
            grupe = new int[podaci.GroupList.Count, 5, 12];
            eventi = new int[podaci.EventList.Count + 1, 3];
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
                                f.WriteLine("<td rowspan='{0}' WIDTH='150'>{1} {2} <br /> {3}  {4}</td>", t.getDuration(), t.getCourse().getName(), tip, eventi[t.getID(), 1], r.findLecturer(eventi[t.getID(), 0]).getName());
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
    }
}
