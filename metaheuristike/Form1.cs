using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ProRa
{
    public partial class Form1 : Form
    {
        Schedule podaci = new Schedule();
        Schedule best = new Schedule();

        public Form1()
        {
            
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                podaci.populateCourseList(openFileDialog1.FileName);
                button3.Enabled = true;
                foreach (Course k in podaci.CourseList)
                {
                    string[] row = { k.getIsvu(), k.needsProjector().ToString(), k.getClassroomType().ToString(), k.getName() };
                    //row.
                    dataGridView1.Rows.Add(row);
                }
            }
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                podaci.populateEventList(openFileDialog1.FileName);
                button6.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                podaci.populateLecturerList(openFileDialog1.FileName);
                button4.Enabled = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                podaci.populateClassroomList(openFileDialog1.FileName);
                button5.Enabled = true;
                foreach (Classroom k in podaci.ClassroomList)
                {
                    string[] row = { k.getID(), k.getCapacity().ToString(), k.getProjector().ToString() , k.getType().ToString() };
                    //row.
                    dataGridView2.Rows.Add(row);
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                podaci.populateGroupList(openFileDialog1.FileName);
                button2.Enabled = true;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Raspored r = podaci.timetabler();
            r.generateHtml("dejan.html", podaci);
          
      /*      best = podaci.deepCopy();
            button7.Enabled = true;
       */     

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView4_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            best.view = 1;
            comboBox1.Items.Clear();
            comboBox1.BeginUpdate();
            foreach (Group g in best.GroupList)
            {
                comboBox1.Items.Add(g.getName());
            }
            comboBox1.EndUpdate();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            best.view = 2;
            comboBox1.Items.Clear();
            comboBox1.BeginUpdate();
            foreach (Lecturer g in best.LecturerList)
            {
                comboBox1.Items.Add(g.getName());
            }
            comboBox1.EndUpdate();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            best.view = 3;
            comboBox1.Items.Clear();
            comboBox1.BeginUpdate();
            foreach (Classroom g in best.ClassroomList)
            {
                comboBox1.Items.Add(g.getID());
            }
            comboBox1.EndUpdate();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (best.view == 1)
            {
                Object selectedItem = comboBox1.SelectedItem;
                string html = best.drawGroupSchedule(best.findGroup(selectedItem.ToString()));
                webBrowser1.DocumentText = html;
            }
            if (best.view == 2)
            {
                //Object selectedItem = comboBox1.SelectedItem;
                int selectedIndex = comboBox1.SelectedIndex;
                string html = best.drawLecturerSchedule(best.findLecturer(selectedIndex));
                webBrowser1.DocumentText = html;
            }
            if (best.view == 3)
            {
                Object selectedItem = comboBox1.SelectedItem;
                //int selectedIndex = comboBox1.SelectedIndex;
                string html = best.drawClassroomSchedule(best.getRoomByID(selectedItem.ToString()));
                webBrowser1.DocumentText = html;
            }
            
        }

        private void button9_Click(object sender, EventArgs e)
        {
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 500;
            progressBar1.Step = 1;
            
          
            backgroundWorker1.RunWorkerAsync();
            
        }
      
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            //label1.Text = "Dejan";
   /*         Schedule temp = new Schedule();
            temp = podaci.deepCopy();
            
            int[] bad0 = new int[7];
            bad0 = temp.evaluateSchedule();
            label3.Text = bad0[0].ToString();
            label4.Text = bad0[3].ToString();
            label5.Text = bad0[4].ToString();
            //temp.generateHtml("temp.html");
            best = temp.deepCopy();
            //Console.WriteLine("{0}", temp.getScore());
            
            int korak = 0;
            int br = 0;
            int promjenjeniEvent = 0;
            bool foo = false;
            while (br < 500)
            {
                label2.Text = br.ToString();
                progressBar1.PerformStep();
                if (foo) continue;
                bool stuck = true;
                korak++;
                
                //backgroundWorker1.ReportProgress(br);
                //label2.Text = br.ToString();
                br++;
                //Console.WriteLine("===============");
                Schedule eval = temp.deepCopy();
                Schedule localBest = podaci.deepCopy();
                localBest.evaluateSchedule();
                foreach (Event f in temp.EventList)
                {
                    //if (f.tabu != 0 && korak - f.tabu < 70) continue; 
                    int t = f.getDuration();
                    foreach (Classroom c in temp.ClassroomList)
                    {
                        if (c.canHost(f))
                        {
                            for (int i = 0; i < 5; i++)
                            {
                                for (int j = 0; j < 12; j++)
                                {


                                    if (c.isAvailable(i, j, t) && f.getLecturer().isAvailable(i, j, t))
                                    {
                                        bool indikator = false;
                                        foreach (string g in f.getGroups())
                                        {
                                            if (eval.findGroup(g).isAvailable(i, j, t) == false)
                                                indikator = true;
                                        }
                                        if (indikator)
                                            continue;

                                        //TU SMO DAKLE, na eval radim remove
                                        //e.getClassroom().removeEvent(e.getID());
                                        eval.getEventByID(f.getID()).getClassroom().removeEvent(f.getID());
                                        eval.getEventByID(f.getID()).getLecturer().removeEvent(f.getID());
                                        eval.getEventByID(f.getID()).getCourse().removeEvent(f.getPlace().i, f.getPlace().j, f.getDuration());
                                        foreach (string g in f.getGroups())
                                        {
                                            eval.findGroup(g).removeEvent(f.getID());
                                        }
                                        //Place p = new Place(eval.getEventByID(e.getID()).getClassroom(), i, j);
                                        Place p = new Place(eval.getRoomByID(c.getID()), i, j);
                                        eval.getEventByID(f.getID()).setPlace(p);
                                        //eval.getEventByID(e.getID()).getClassroom().setEvent(i, j, e);
                                        eval.getRoomByID(c.getID()).setEvent(i, j, f);
                                        eval.getEventByID(f.getID()).getLecturer().setEvent(i, j, f);
                                        eval.getEventByID(f.getID()).getCourse().setEvent(i, j, f);
                                        eval.getEventByID(f.getID()).setClassroom(eval.getRoomByID(c.getID()));
                                        foreach (string g in f.getGroups())
                                        {
                                            eval.findGroup(g).setEvent(i, j, f);
                                        }
                                        eval.evaluateSchedule();
                                        if (eval.getScore() > best.getScore())
                                        {
                                            best = eval.deepCopy();
                                            label1.Text = best.getScore().ToString();
                                            //string html = best.drawGroupSchedule(best.findGroup("MA1_1"));
                                            //webBrowser1.DocumentText = html;
                                            //Console.WriteLine("{0}", best.getScore());
                                            //best.generateHtml("best.html");
                                            promjenjeniEvent = f.getID();
                                            stuck = false;
                                        }
                                        if (eval.getScore() > localBest.getScore())
                                        {
                                            localBest = eval.deepCopy();
                                           
                                        }
                                        eval = temp.deepCopy();
                                    }
                                }
                            }
                        }
                    }

                }
                if (stuck)
                {
                    //best = localBest.deepCopy();
                    //label1.ForeColor = Color.Red;
                    foo = true;
					label2.Text = br.ToString();
                    int[] bad = new int[7];
                    bad = best.evaluateSchedule();
                    label10.Text = bad[0].ToString();
                    label11.Text = bad[3].ToString();
                    label12.Text = bad[4].ToString();
                }
                temp = best.deepCopy();
                temp.getEventByID(promjenjeniEvent).tabu = korak;
                //label2.Text = promjenjeniEvent.ToString();
                
            }*/
            //label2.Text = "ERROR" + br.ToString();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //string sadrzajDatoteke = podaci.generateHtml();
            saveFileDialog1.Title = "Spremi datoteku.";
            string temp = "raspored.html";
            
            saveFileDialog1.FileName = temp;
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName != "")
            {
                //TextWriter f = new StreamWriter(saveFileDialog1.FileName);
                best.generateHtml(saveFileDialog1.FileName);
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                podaci.populateMatrix(openFileDialog1.FileName);
                //button3.Enabled = true;
                //foreach (Course k in podaci.CourseList)
                //{
                   // string[] row = { k.getIsvu(), k.needsProjector().ToString(), k.getClassroomType().ToString(), k.getName() };
                    //row.
                   // dataGridView1.Rows.Add(row);
                //}
            }
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        

       
    }
}
