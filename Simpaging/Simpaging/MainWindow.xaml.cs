using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;

namespace Simpaging
{
    /// This program simulates the paging and memory allocation done by an OS.
    /// Interaction logic for MainWindow.xaml
    /// By Lanndon Rose
    public partial class MainWindow : Window
    {
        //instantiation of needed variables
        int pgSize = 512;
        string[] PageT;
        string[] TextArray;
        List<int> freeFrames;
        int step;

        //resets progrma
        public void Zero()
        {
            PageT = new string[8];
            freeFrames = new List<int>();
            step = 0;
        }


        //determines number of pages a process needs
        public int NumberPages(int temp)
        {
            int numPg = 1;
            while ((pgSize * numPg) < temp)
            {
                numPg++;
            }
            return numPg;
        }

        //allocates memory to a process 
        public void AllocateFrame(string[] readfile, int x)
        {


            string[] txtArray = readfile;
            string[] parts = {"!"};
            while (parts.Contains("!") )

            {
                try
                {
                    if (x < txtArray.Length)
                    {
                        parts = txtArray[x].Split(new[] { ' ' });
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show("End of File, load another file to start.");
                        Zero();
                        Next.IsEnabled = false;
                        Start.IsEnabled = false;
                        break;
                    }
                }
                catch (NullReferenceException e)
                {
                    System.Windows.Forms.MessageBox.Show("No File loaded.");
                    Console.WriteLine(e.Message);
                    return;
                }
            }

            int PID = 0;
            int TextSize = 0;
            int dataSize = 0;
            int procPages = (NumberPages(TextSize)) + (NumberPages(dataSize));

            if (!parts.Contains("Halt"))
            {


                try
                {
                    PID = Int32.Parse(parts[0]);
                    TextSize = Int32.Parse(parts[1]);
                    dataSize = Int32.Parse(parts[2]);
                }
                catch (FormatException e)
                {
                    Console.WriteLine(e.Message);
                }

                int t = 0;
                int d = 0;
                //if process frames are less than free frames, then allocate
                if (procPages <= freeFrames.Count )
                {
                    int k = 0;
                    int j = 0;
                    for (int s = 0; s < NumberPages(TextSize); s++)
                    {
                        PageT[freeFrames[0]] = "P" + PID + " Text Page: " + t;
                        if (k < 1)
                        {
                            PageBox.Items.Add("Process" + PID + " Text " + t + "  Frame  " + freeFrames[0]);
                            k++;
                        }
                        else
                        {
                            PageBox.Items.Add("Process" + PID + "        " + t + "  Frame  " + freeFrames[0]);
                        }
                        t++;
                        freeFrames.RemoveAt(0);
                    }

                    for (int s = 0; s < NumberPages(dataSize); s++)
                    {
                        PageT[freeFrames[0]] = "P" + PID + " Data Page: " + d;
                        if (j < 1)
                        {
                            PageBox.Items.Add("Process" + PID + " Data " + d + "  Frame  " + freeFrames[0]);
                            j++;
                        }
                        else
                        {
                            PageBox.Items.Add("Process" + PID + "        " + d + "  Frame  " + freeFrames[0]);
                        }

                        d++;
                        freeFrames.RemoveAt(0);
                    }

                }
                //if process pages are greater than free frames, print message
                else
                {
                    if (procPages != 0)
                    {
                        System.Windows.MessageBox.Show("Not Enough Free Frames");
                    }
                }

            }
            else if(parts == null)
            {
                System.Windows.MessageBox.Show("Done.");
            }
            else
            {
                PID = Int32.Parse(parts[0]);
                ClearMem(PID.ToString(), procPages);
            }
        }

        //getter for page table
        public string[] GetFrameArray()
        {
            return PageT;
        }


        //clears page table and adds frames back to free frame list
        public void FreshPageT()
        {
            Array.Clear(PageT, 0, PageT.Length);
            freeFrames.Clear();
            for (int x = 0; x < PageT.Length; x++)
            {

                PageT[x] = null;
                freeFrames.Add(x);
            }
        }


        //updates table when memory has been allocated or deallocated
        public void UpdateTable(int x)
        {
            box0.Text = PageT[0];
            box1.Text = PageT[1];
            box2.Text = PageT[2];
            box3.Text = PageT[3];
            box4.Text = PageT[4];
            box5.Text = PageT[5];
            box6.Text = PageT[6];
            box7.Text = PageT[7];
            freeFramesTB.Text = Convert.ToString(freeFrames.Count);

            //error handling
            if (x < TextArray.Length)
            {
                fileLine.Text = TextArray[x];
            }
        }


        //called when memory needs to be deallocated
        public void ClearMem(string PID, int proc_pages)
        {
            for (int a = 0; a < PageT.Length; a++)
            {
                if (PageT[a].Contains("P" + PID))
                {
                    PageT[a] = " ";
                    freeFrames.Add(a);
                }
            }
            //clear page table when process halts
            for (int n = PageBox.Items.Count - 1; n >= 0; --n)
            {
                string removelistitem = "Process" + PID;
                if (PageBox.Items[n].ToString().Contains(removelistitem))
                {
                    PageBox.Items.RemoveAt(n);
                }
            }

            //updates memory table
            UpdateTable(step);
        }

        //method for error handling
        private static void Print(int s)
        {
            Console.WriteLine(s);
        }


        //starts the components for the GUI
        public MainWindow()
        {
            InitializeComponent();

            //makes so memory boxes are not editable
            Next.IsEnabled = false;
            Start.IsEnabled = false;
            box0.IsReadOnly = true;
            box1.IsReadOnly = true;
            box2.IsReadOnly = true;
            box3.IsReadOnly = true;
            box4.IsReadOnly = true;
            box5.IsReadOnly = true;
            box6.IsReadOnly = true;
            box7.IsReadOnly = true;
            freeFramesTB.IsReadOnly = true;
        }


        //loads file into an array
        private void LoadBTN(object sender, RoutedEventArgs e)
        {
            //found this here
            //https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.openfiledialog?view=netframework-4.7.1
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            TextArray = System.IO.File.ReadAllLines(openFileDialog1.FileName);
                            Start.IsEnabled = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }           
        }

        private void NextStep(object sender, RoutedEventArgs e)
        {
            AllocateFrame(TextArray, step);
            UpdateTable(step);
            step++;
        }


        //sets up program when the start button is clicked
        private void Start_Click(object sender, RoutedEventArgs e)
        {
            Zero();
            FreshPageT();
            Next.IsEnabled = true;
            AllocateFrame(TextArray, step);
            UpdateTable(step);
            step++;
        }


        //exits when quit it clicked
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Environment.Exit(1);
        }


    }
}
