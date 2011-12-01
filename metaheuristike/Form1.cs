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
        Raspored best;

        public Form1()
        {
            InitializeComponent();
            numericUpDown1.Value = 200;
            textBox1.Text = Convert.ToString(1);
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
                /*foreach (Course k in podaci.CourseList)
                {
                    string[] row = { k.getIsvu(), 
                                     k.needsProjector().ToString(), 
                                     k.getClassroomType().ToString(), 
                                     k.getName() };
                    
                    dataGridView1.Rows.Add(row);
                }*/
            }
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                podaci.populateEventList(openFileDialog1.FileName);
                button10.Enabled = true;
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
                /*foreach (Classroom k in podaci.ClassroomList)
                {
                    string[] row = { k.getID(), 
                                     k.getCapacity().ToString(), 
                                     k.getProjector().ToString(), 
                                     k.getType().ToString() };
                    
                    dataGridView2.Rows.Add(row);
                }*/
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
            best = podaci.timetabler();
            int[] bad0 = new int[7];
            bad0 = best.evaluateSchedule(podaci);
            label3.Text = bad0[0].ToString();
            label4.Text = bad0[3].ToString();
            label5.Text = bad0[1].ToString();
            label16.Text = bad0[4].ToString();
            button7.Enabled = true;

            best.view = 1;
            comboBox1.Items.Clear();
            comboBox1.BeginUpdate();
            foreach (Group g in podaci.GroupList)
            {
                comboBox1.Items.Add(g.getName());
            }
            comboBox1.EndUpdate();

            MessageBox.Show("Timetabler je završio",
        "Obavijest",
        MessageBoxButtons.OK);
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
            foreach (Group g in podaci.GroupList)
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
            foreach (Lecturer g in podaci.LecturerList)
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
            foreach (Classroom g in podaci.ClassroomList)
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
                Group g = podaci.findGroup(selectedItem.ToString());
                string html = best.drawGroupSchedule(g.Id, podaci);
                webBrowser1.DocumentText = html;
            }
            if (best.view == 2)
            {
                int selectedIndex = comboBox1.SelectedIndex;
                string html = best.drawLecturerSchedule(selectedIndex, podaci);
                webBrowser1.DocumentText = html;
            }
            if (best.view == 3)
            {
                Object selectedItem = comboBox1.SelectedItem;
                Classroom c = podaci.getRoomByID(selectedItem.ToString());
                string html = best.drawClassroomSchedule(c.Id, podaci);
                webBrowser1.DocumentText = html;
            }
            
        }

        private void button9_Click(object sender, EventArgs e)
        {
            progressBar1.Minimum = 0;
            progressBar1.Maximum = (int)numericUpDown1.Value;
            progressBar1.Step = 1;
            backgroundWorker1.RunWorkerAsync();   
        }
      
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            //label1.Text = "Dejan";
            Raspored temp = new Raspored(best);
            best.RemoveEvent(podaci, 4);
            temp.evaluateSchedule(podaci);
            //temp = podaci.deepCopy();
            
            int[] bad0 = new int[7];
            bad0 = temp.evaluateSchedule(podaci);
            label3.Text = bad0[0].ToString();
            label4.Text = bad0[3].ToString();
            label5.Text = bad0[1].ToString();
            label16.Text = bad0[4].ToString();
            //temp.generateHtml("temp.html", podaci);
            //best = temp.deepCopy();
            //Console.WriteLine("{0}", temp.getScore());

            int tb = podaci.EventList.Count / 10;
            int korak = 0;
            int br = 0;
            int promjenjeniEvent = 0;
            bool foo = false;
            double stop = Double.Parse(textBox1.Text);
            while (br < numericUpDown1.Value && best.Score < stop)
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
                Raspored eval = new Raspored(temp);//temp.deepCopy();
          //      Schedule localBest = podaci.deepCopy();
          //      localBest.evaluateSchedule();
                foreach (Event f in podaci.EventList)
                {
                    int eventId = f.Id;
                    int LecId = f.getLecturer().Id;
                    if (f.tabu != 0 && korak - f.tabu < tb) continue; 
                    int t = f.Duration;
                    foreach (Classroom c in podaci.ClassroomList)
                    {
                        //int Roo
                        if (c.canHost(f))
                        {
                            for (int i = 0; i < 5; i++)
                            {
                                for (int j = 0; j < 12; j++)
                                {
                                    

                                    //if (c.isAvailable(i, j, t) && f.getLecturer().isAvailable(i, j, t))
                                    if (eval.IsRoomAvailable(c.Id, i, j, t) && eval.IsLecturerAvailable(LecId, i, j, t))
                                    {
                                        bool indikator = false;
                                        foreach (string g in f.getGroups())
                                        {

                                            if (eval.IsGroupAvailable(podaci.findGroup(g).Id, i, j, t) == false)
                                                indikator = true;
                                        }
                                        if (indikator)
                                            continue;

                                        //TU SMO DAKLE, na eval radim remove
                                        
                                        //eval.getEventByID(f.getID()).getClassroom().removeEvent(f.getID());
                                        //eval.getEventByID(f.getID()).getLecturer().removeEvent(f.getID());
                                        //eval.getEventByID(f.getID()).getCourse().removeEvent(f.getPlace().i, f.getPlace().j, f.getDuration());

                                        //foreach (string g in f.getGroups())
                                        //{
                                        //    eval.findGroup(g).removeEvent(f.getID());
                                        //}
                                        
                                        eval.RemoveEvent(podaci, eventId);

                                       /* Place p = new Place(eval.getRoomByID(c.getID()), i, j);
                                        eval.getEventByID(f.getID()).setPlace(p);
                                        
                                        eval.getRoomByID(c.getID()).setEvent(i, j, f);
                                        eval.getEventByID(f.getID()).getLecturer().setEvent(i, j, f);
                                        eval.getEventByID(f.getID()).getCourse().setEvent(i, j, f);
                                        eval.getEventByID(f.getID()).setClassroom(eval.getRoomByID(c.getID()));
                                        foreach (string g in f.getGroups())
                                        {
                                            eval.findGroup(g).setEvent(i, j, f);
                                        }
                                        */
                                        eval.SetEvent(podaci, eventId, c.Id, i, j);
                                        bad0 = eval.evaluateSchedule(podaci);
                                        if (eval.Score >= best.Score)
                                        {
                                            label10.Text = bad0[0].ToString();
                                            label11.Text = bad0[3].ToString();
                                            label12.Text = bad0[1].ToString();
                                            label17.Text = bad0[4].ToString();
                                            best = new Raspored(eval);
                                            label1.Text = best.Score.ToString().Substring(0, Math.Min(10, best.Score.ToString().Length));
                                            //string html = best.drawGroupSchedule(best.findGroup("MA1_1"));
                                            //webBrowser1.DocumentText = html;
                                            //Console.WriteLine("{0}", best.getScore());
                                            //best.generateHtml("best.html");
                                            promjenjeniEvent = f.getID();
                                            //f.tabu = 10;
                                            stuck = false;
                                            //temp.generateHtml("stuck.html", podaci);
                                        }
                                  /*      if (eval.getScore() > localBest.getScore())
                                        {
                                            localBest = eval.deepCopy();
                                           
                                        }*/
                                        eval = new Raspored(temp);
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
					//label2.Text = br.ToString();
                    int[] bad = new int[7];
                    bad = best.evaluateSchedule(podaci);
                    label10.Text = bad0[0].ToString();
                    label11.Text = bad0[3].ToString();
                    label12.Text = bad0[1].ToString();
                    label17.Text = bad0[4].ToString();
                    //temp.generateHtml("stuck.html", podaci);
                }
                temp = new Raspored(best);
                podaci.getEventByID(promjenjeniEvent).tabu = korak;
                podaci.getEventByID(promjenjeniEvent).F++;
                //label2.Text = promjenjeniEvent.ToString();
                
            }
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
                best.generateHtml(saveFileDialog1.FileName, podaci);
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
            button6.Enabled = true;
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        

       
    }
}
