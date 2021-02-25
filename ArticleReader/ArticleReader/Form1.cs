using Fusionbird.FusionToolkit.FusionTrackBar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace ArticleReader
{
    public partial class Form1 : Form
    {
        BaseMenuSystem Menu; 
        FusionTrackBar track2 = new FusionTrackBar();
        public Form1()
        {
            int plus = 0;
            InitializeComponent();
            
            this.MaximizeBox = false;
            Menu = new BaseMenuSystem(panel1);
            //Menu.AddItem(new thejimquisitionMenu(Menu, this)); //This has been disabled in 2021, as the website is down
            Menu.AddItem(new ePubMenu(Menu, this));
            Menu.AddItem(new TemplateMenu(Menu, this));
            Menu.AddItem(new RSSFeedMenu(Menu, this));
            //--- New 2021 code added for Demo purposes ---//

            Menu.AddItem(new DemoMenu(Menu, this));

            //--- End ---//
            
            Controls.Add(Menu);
            Menu.BringToFront();
            //BackgroundImage = Image.FromFile(@"D:\Projects\Assets\ArticleReader\Background.png");
            //BackgroundImage = Image.FromFile(@"D:\Projects\Assets\ArticleReader\SimpleBackground2.png");
            BackgroundImageLayout = ImageLayout.Stretch;
            track2.Location = trackBar1.Location;
            textBox1.Parent = this;
            Controls.Add(track2);
            track2.AutoSize = false;
            track2.Size = trackBar1.Size;
            track2.BackColor = Color.Transparent;
            track2.Scroll += trackBar1_Scroll;
            trackBar1.Visible = false;
            textBox1.BackColor = Color.FromArgb(132, 140, 153);
            button3.Focus();
            Panel Border = new Panel();
            Border.Size = new Size(textBox1.Size.Width + 2, 15);
            Controls.Add(Border);
            Border.Location = new Point(textBox1.Location.X - 1, textBox1.Location.Y + 2);
            Border.BackColor = Color.Transparent;
            Border.Controls.Add(textBox1);
            textBox1.Location = new Point(1, 1);
            textBox1.BorderStyle = BorderStyle.None;
            Border.Paint += Border_Paint;
            textBox1.Size = new Size(Border.Size.Width - 2, Border.Size.Height - 2);
            textBox1.ForeColor = Color.White;
            button3.FlatAppearance.BorderColor = Color.FromArgb(28, 109, 240);
            button3.FlatStyle = FlatStyle.Flat;
            button3.BackColor = Color.Transparent;
            button3.ForeColor = Color.FromArgb(230, 230, 230);
            button2.FlatAppearance.BorderColor = Color.FromArgb(28, 109, 240);
            button2.FlatStyle = FlatStyle.Flat;
            button2.BackColor = Color.Transparent;
            button2.ForeColor = Color.FromArgb(230, 230, 230);
            button1.FlatAppearance.BorderColor = Color.FromArgb(28, 109, 240);
            button1.FlatStyle = FlatStyle.Flat;
            button1.BackColor = Color.Transparent;
            button1.ForeColor = Color.FromArgb(230, 230, 230);
            track2.Value = 5;
            track2.LargeChange = 1;
            if (!File.Exists("Templates.txt"))
            {
                File.Create("Templates.txt");
                Thread.Sleep(500);
                Process.Start(System.Reflection.Assembly.GetExecutingAssembly().Location);
                this.Close();
            }
        }

        void Border_Paint(object sender, PaintEventArgs e)
        {
            Panel pan = (Panel)sender;
            Pen Pen = new Pen(new SolidBrush(Color.FromArgb(28, 109,240)));
            e.Graphics.DrawLine(Pen, 0, 0, pan.Size.Width - 1, 0);
            e.Graphics.DrawLine(Pen, 0, 0, 0, pan.Size.Height - 1);
            e.Graphics.DrawLine(Pen, 0, pan.Size.Height - 1, pan.Size.Width - 1, pan.Size.Height - 1);
            e.Graphics.DrawLine(Pen, pan.Size.Width - 1, 0, pan.Size.Width - 1, pan.Size.Height - 1);
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            panel1.MouseEnter += panel1_MouseEnter;
            button3_Click(button3, e);
        }

        void panel1_MouseEnter(object sender, EventArgs e)
        {
            try
            {
                Article.Focus();
            }
            catch (Exception e1)
            {

            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            button1.NotifyDefault(false);
            Boolean Built = false;
            panel1.BringToFront();
            track2.Value = 5;
            if (Article != null)
            {
                Article.Synth.Dispose();
                Article.Dispose();
            }
            if (textBox1.Text != "")
            {
                string ReadText = "";
                string Website = textBox1.Text;
                if (Website.StartsWith("Http://") || Website.StartsWith("Https://"))
                {
                    Website = Website.Substring(Website.IndexOf("//"));
                }
                if (Website.Contains("/"))
                {
                    Website = Website.Substring(Website.IndexOf("/") + 2);
                }
                if (Website.StartsWith("www."))
                {
                    Website = Website.Substring(Website.IndexOf("www.") + "www.".Length);
                }
                if (Website.Contains("."))
                {
                    Website = Website.Substring(0, Website.IndexOf("."));
                }
                switch (Website)
                {
                    case "thejimquisition":
                        Built = true;
                        Article = (thejimquisition)new thejimquisition(textBox1.Text, panel1);
                        Article.Speed = track2.Value - 5;
                        Synth = Article.Synth;
                        break;
                    case "vg247":
                        Built = true;
                        Article = (vg247)new vg247(textBox1.Text, panel1);
                        Article.Speed = track2.Value - 5;
                        Synth = Article.Synth;
                        break;
                }
                if (!Built)
                {
                    Boolean TemplateFound = false;
                    if (File.Exists("Templates.txt"))
                    {
                        foreach (string a in File.ReadAllLines("Templates.txt"))
                        {
                            string Web = a.Split(',')[0];
                            if (Web == Website)
                            {
                                TemplateFound = true;
                                Article = (TemplateArticle)new TemplateArticle(textBox1.Text, panel1, a);
                                Article.Speed = track2.Value - 5;
                                Synth = Article.Synth;
                                break;
                            }
                        }
                    }
                    if (!TemplateFound)
                    {
                        button3_Click(button3, e);
                        MessageBox.Show("Unable to find Template of Article, create a Template in the Template Manager!", "Template Missing!");
                    }
                }
            }
            button2.Text = "Pause";
        }
        public BaseArticle Article;
        public SpeechSynthesizer Synth;
        private void button2_Click(object sender, EventArgs e)
        {
            button2.NotifyDefault(false);
            try
            {
                if (Synth != null)
                {
                    if (Synth.State == SynthesizerState.Speaking)
                    {
                        Synth.Pause();
                        button2.Text = "Play";
                    }
                    else if (Synth.State == SynthesizerState.Paused)
                    {
                        Synth.Resume();
                        button2.Text = "Pause";
                    }
                }
            }
            catch (Exception e1)
            {

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            button3.NotifyDefault(false);
            if (Article != null)
            {
                Article.Dispose();
            }
            if (Menu != null)
            {
                //Menu.Dispose();
            }
            //Menu = new BaseMenuSystem(panel1);
            //Controls.Add(Menu);
            Menu.BringToFront();
            if (Synth != null)
            {
                Synth.Dispose();
            }
            button2.Text = "Pause";
            foreach (Control a in Menu.Controls)
            {

                try
                {
                    BaseMenuItem b = (BaseMenuItem)a;
                    if (b.ItemMenu)
                    {

                    }
                }
                catch (Exception e1)
                {
                    a.Dispose();
                }
            }
            //Menu.AddItem(new thejimquisitionMenu(Menu, this));
           // Menu.AddItem(new ePubMenu(Menu, this));
            //Menu.AddItem(new TemplateMenu(Menu, this));
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        int Previous = 5;
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (Article != null)
            {
                Article.Speed = track2.Value - 5;
            }
            else
            {
                track2.Value = Previous;
            }
        }
    }
    public partial class CustomTextBox : TextBox
    {
        public CustomTextBox()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.UserPaint, true);
            BackColor = Color.Transparent;
        }
    }
}
