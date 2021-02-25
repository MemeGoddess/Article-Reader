using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArticleReader
{
    class vg247 : BaseArticle
    {
        public Color BackColorA = Color.Black;
        public vg247(string URL, Panel panel1)
        {
            panel1.BackColor = BackColorA;
            panel1.Controls.Add(this);
            ReadText = "";
            WebClient Web = new WebClient();
            string Data = Web.DownloadString(URL);
            string Title = Data.Substring(Data.IndexOf("<h1>") + 4);
            Title = Title.Substring(0, Title.IndexOf("<"));
            Title = RemoveHashCode(Title);
            Data = Data.Substring(Data.IndexOf("<section>") + "<section".Length);
            Data = Data.Substring(0, Data.IndexOf("<p class=\"affiliate-disclaimer"));
            Data = Data.Substring(Data.IndexOf("<p>") + 3);
            while (Data.Contains("<span"))
            {
                int start = Data.IndexOf("<span");
                int end = Data.IndexOf("</span>") + "</span>".Length;
                string temp = Data.Substring(0, start);
                temp += Data.Substring(end);
                Data = temp;
            }
            while (Data.Contains("<p class=\"quote"))
            {
                int start = Data.IndexOf("<p class=\"quote");
                string IndexTemp = Data.Substring(start);
                int end = IndexTemp.IndexOf("</p>") + "</p>".Length  + start;
                string temp = Data.Substring(0, start);
                temp += Data.Substring(end);
                Data = temp;
            }
            while (Data.Contains("<p class=\"quote"))
            {
                int start = Data.IndexOf("<p class=\"quote");
                string IndexTemp = Data.Substring(start);
                int end = IndexTemp.IndexOf("</p>") + "</p>".Length + start;
                string temp = Data.Substring(0, start);
                temp += Data.Substring(end);
                Data = temp;
            }
            string[] SplitA = Data.Split(new string[] { "<p>", "</p>" }, StringSplitOptions.RemoveEmptyEntries);
            Data = "";
            foreach (string a in SplitA)
            {
                Data += a + "";
            }
            string[] SplitB = Data.Split(new string[] { "<em>", "</em>" }, StringSplitOptions.RemoveEmptyEntries);
            Data = "";
            foreach (string a in SplitB)
            {
                Data += a + "";
            }
            //<div class=\"embedbox center
            while (Data.Contains("<div class=\"embedbox center"))
            {
                int start = Data.IndexOf("<div class=\"embedbox center");
                string IndexTemp = Data.Substring(start);
                int end = IndexTemp.IndexOf(">") + ">".Length + start;
                string temp = Data.Substring(0, start);
                temp += Data.Substring(end);
                Data = temp;
            }
            string ImageContent = Data;
            while (Data.Contains("<img"))
            {

                int start = Data.IndexOf("<img");
                int startImage = ImageContent.IndexOf("<img");
                string Link = Data.Substring(start);
                Link = Link.Substring(Link.IndexOf("src") + "src=\"".Length);
                Link = Link.Substring(0, Link.IndexOf("\""));

                string IndexMain = Data.Substring(start);
                int end = IndexMain.IndexOf("/>") + "/>".Length;
                string IndexImage = ImageContent.Substring(startImage);
                int endImage = IndexImage.IndexOf("/>") + "/>".Length;
                string ImageTemp = ImageContent.Substring(0, startImage);
                string temp = Data.Substring(0, start);

                if (Link.EndsWith(".jpg"))
                {
                    ImageTemp += "\n\n(Img:" + Link + ")";
                    ImageTemp += "\n\n" + IndexImage.Substring(endImage);
                }
                else
                {
                    string NameTemp = Data.Substring(start);
                    NameTemp = NameTemp.Substring(NameTemp.IndexOf(">") + 1);
                    NameTemp = NameTemp.Substring(0, NameTemp.IndexOf("<"));
                    ImageTemp += NameTemp;
                    ImageTemp += Data.Substring(endImage);
                    temp += NameTemp;
                }
                temp += IndexMain.Substring(end);
                Data = temp;
                ImageContent = ImageTemp;
            }//<br />
            string[] SplitC = Data.Split(new string[] { "<br />", "</div>" }, StringSplitOptions.RemoveEmptyEntries);
            Data = "";
            foreach (string a in SplitC)
            {
                Data += a + "";
            }
            string[] SplitD = ImageContent.Split(new string[] { "<br />", "</div>" }, StringSplitOptions.RemoveEmptyEntries);
            ImageContent = "";
            foreach (string a in SplitD)
            {
                ImageContent += a + "";
            }
            while (Data.Contains("<a"))
            {

                int start = Data.IndexOf("<a");
                int startImage = ImageContent.IndexOf("<a");
                string Link = Data.Substring(start);
                Link = Link.Substring(Link.IndexOf("href") + 6);
                Link = Link.Substring(0, Link.IndexOf("\""));

                int end = Data.IndexOf("</a>") + "</a>".Length;
                int endImage = Data.IndexOf("</a>") + "</a>".Length;
                string ImageTemp = ImageContent.Substring(0, startImage);
                string temp = Data.Substring(0, start);

                if (Link.EndsWith(".jpg"))
                {
                    ImageTemp += "\n\n(Img:" + Link + ")";
                    ImageTemp += "\n\n" + Data.Substring(endImage);
                }
                else
                {
                    string NameTemp = Data.Substring(start);
                    NameTemp = NameTemp.Substring(NameTemp.IndexOf(">") + 1);
                    NameTemp = NameTemp.Substring(0, NameTemp.IndexOf("<"));
                    ImageTemp += NameTemp;
                    ImageTemp += Data.Substring(endImage);
                    temp += NameTemp;
                }
                temp += Data.Substring(end);
                Data = temp;
                ImageContent = ImageTemp;
            }
            Data = RemoveHashCode(Data);
            ImageContent = RemoveHashCode(ImageContent);
            BackColor = Color.White;
            Location = new Point(100, 0);
            Size = new Size(600, 450);
            int Offset = 50;
            Boolean Override = false;
            AutoScroll = true;
            Boolean LastSpace = false;
            //Text.Select(10, 10);
            //Text.SelectionColor = Color.FromArgb(100, 255, 0, 0);
            int WordCount = 0;
            ReadText = Title + "\n" + Data;
            ImageContent = Title + "\n" + ImageContent;
            while(ReadText.Contains("\n\n"))
            {
                string[] SplitE = ReadText.Split(new string[] { "\n\n"}, StringSplitOptions.RemoveEmptyEntries);
                ReadText = "";
                foreach (string a in SplitE)
                {
                    ReadText += a + "\n";
                }
            }
            foreach (string a in ImageContent.Split('\n'))
            {
                if (a.StartsWith("(Img:"))
                {
                    PictureBox b = new PictureBox();
                    b.Size = new Size(540, 300);
                    b.SizeMode = PictureBoxSizeMode.StretchImage;
                    var request = WebRequest.Create(a.Substring(5, a.Length - 6));

                    using (var response = request.GetResponse())
                    using (var stream = response.GetResponseStream())
                    {
                        b.Image = Bitmap.FromStream(stream);
                    }
                    Controls.Add(b);
                    b.Location = new Point(30, Offset);
                    Offset += 320;
                    Override = true;
                    LastSpace = false;
                    b.MouseEnter += b_MouseEnter;
                }
                else
                {
                    if (!LastSpace || a != "")
                    {
                        Boolean FoundText = false;
                        foreach (char c in a)
                        {
                            if (c != '\t')
                            {
                                FoundText = true;
                            }
                        }
                        if (FoundText)
                        {
                            RichTextBox Text = new RichTextBox();
                            Text.Location = new Point(30, Offset);
                            Controls.Add(Text);
                            Text.ReadOnly = true;
                            Text.BorderStyle = BorderStyle.None;
                            Text.BackColor = Color.White;
                            Text.Text = a;
                            //Text.Select(10, 10);
                            //Text.SelectionColor = Color.FromArgb(100, 255, 0, 0);

                            Text.Size = new Size(540, Text.Size.Height);
                            if (Text.Text == "")
                            {
                                LastSpace = true;
                            }
                            using (Graphics g = CreateGraphics())
                            {
                                Text.Height = (int)g.MeasureString(Text.Text,
                                    Text.Font, Text.Width).Height + 5;
                            }
                            Offset += Text.Size.Height + 10;
                            Text.MouseEnter += b_MouseEnter;
                            WordCount = 0;
                            foreach (char word in a)
                            {

                                WordCount++;
                            }
                            Counts[LabelCount] = WordCount;
                            LabelCount++;
                        }
                    }
                }
            }
            Synth.SpeakAsync(ReadText);
        }

        void b_MouseEnter(object sender, EventArgs e)
        {
            Focus();
        }

        
    }
}
