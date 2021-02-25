using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace ArticleReader
{
    class thejimquisition : BaseArticle
    {
        
        public Color BackColorA = Color.FromArgb(38, 38, 38);
        
        public thejimquisition(string URL, Panel panel1)
        {
            panel1.BackColor = Color.Transparent;
            panel1.Controls.Add(this);
            ReadText = "";
            WebClient Web = new WebClient();
            string Data = Web.DownloadString(URL);
            string ElementTag = Data.Substring(0, Data.IndexOf("class=\"entry-title \""));
            ElementTag = ElementTag.Substring(ElementTag.LastIndexOf("<") + 1);
            ElementTag = ElementTag.Substring(0, ElementTag.IndexOf(" "));
            string Title = Data.Substring(Data.IndexOf("class=\"entry-title \"") + "class=\"entry-title \"".Length + 4);
            Title = Title.Substring(0, Title.IndexOf(ElementTag) - 2);
            HttpUtility.HtmlDecode(Title);
            string[] Split = Title.Split(new string[] { "&#8217;", "<i>", "</i>" }, StringSplitOptions.RemoveEmptyEntries);
            Title = "";
            foreach (string a in Split)
            {
                Title += a;
            }
            Title = Title.Replace("&#8211;", "-");
            Title = RemoveHashCode(Title);
            ReadText += Title + ":";

            //Content

            string Content1 = Data.Substring(Data.IndexOf("class=\"entry-content\"") + "class=\"entry-content\"".Length + 2);
            Content1 = Content1.Substring(0, Content1.IndexOf("</div>"));
            string Content = "";

            foreach (char a in Content1)
            {
                if (a != 'Â')
                {
                    Content += a;
                }
            }

            string[] SplitC = Content.Split(new string[] { "<i>", "</i>", "<em>", "</em>" }, StringSplitOptions.RemoveEmptyEntries);
            Content = "";
            foreach (string a in SplitC)
            {
                Content += a;
            }
            string[] SplitD = Content.Split(new string[] { "&#8217;", "&#8216;" }, StringSplitOptions.RemoveEmptyEntries);
            Content = "";
            foreach (string a in SplitD)
            {
                Content += a + "'";
            }
            Content = Content.Substring(0, Content.Length - 1);
            string[] SplitE = Content.Split(new string[] { "&#8220;", "&#8221;" }, StringSplitOptions.RemoveEmptyEntries);
            Content = "";
            foreach (string a in SplitE)
            {
                Content += a + "\"";
            }//&#8212;
            Content = Content.Substring(0, Content.Length - 1);
            string[] SplitL = Content.Split(new string[] { "&#8212;" }, StringSplitOptions.RemoveEmptyEntries);
            Content = "";
            foreach (string a in SplitL)
            {
                Content += a + "-";
            }
            Content = Content.Replace("&#8211;", "-");
            Content = Content.Replace("&#8230;", "...");
            string Rating = "";
            Rating = Content.Substring(Content.LastIndexOf("<strong") + "<strong>".Length);
            Rating = Rating.Substring(0, Rating.IndexOf("</strong"));
            if (Rating.Contains("/10"))
            {
                string[] SplitQ = Rating.Split(new string[] { "<br />" }, StringSplitOptions.RemoveEmptyEntries);
                Rating = "";
                Boolean First = true;
                foreach (string a in SplitQ)
                {
                    Rating += a;
                }
            }
            else
            {
                string RatingFix = Content.Substring(0, Content.LastIndexOf("<strong>"));
                RatingFix = RatingFix.Substring(RatingFix.LastIndexOf("<strong") + "<strong>".Length);
                RatingFix = RatingFix.Substring(0, RatingFix.IndexOf("</strong"));
                if (RatingFix.Contains("/10"))
                {
                    string[] SplitQ = RatingFix.Split(new string[] { "<br />" }, StringSplitOptions.RemoveEmptyEntries);
                    RatingFix = "";
                    Boolean First = true;
                    foreach (string a in SplitQ)
                    {
                        RatingFix += a;
                    }
                    Rating = RatingFix + "\n" + Rating;
                }
            }
            Content = Content.Substring(0, Content.LastIndexOf("<p style"));
            while (Content.Contains("<strong>"))
            {
                int start = Content.IndexOf("<strong>");
                int end = Content.IndexOf("</strong>") + "</strong>".Length;
                string temp = Content.Substring(0, start);
                temp += Content.Substring(end);
                Content = temp;
            }
            while (Content.Contains("<span"))
            {
                int start = Content.IndexOf("<span");
                int end = Content.IndexOf("</span>") + "</span>".Length;
                string temp = Content.Substring(0, start);
                temp += Content.Substring(end);
                Content = temp;
            }

            while (Content.Contains("<p "))
            {
                int start = Content.IndexOf("<p ");
                string temp1 = Content.Substring(start);
                int end = temp1.IndexOf("</br>") + "</br>".Length + start;
                string temp = Content.Substring(0, start);
                temp += Content.Substring(end);
                Content = temp;
            }
            while (Content.Contains("<br />"))
            {
                int start = Content.IndexOf("<br />");
                string temp1 = Content.Substring(start);
                int end = temp1.IndexOf("<br />") + "<br />".Length + start;
                string temp = Content.Substring(0, start);
                temp += Content.Substring(end);
                Content = temp;
            }
            string ImageContent = Content;
            while (Content.Contains("<a"))
            {

                int start = Content.IndexOf("<a");
                int startImage = ImageContent.IndexOf("<a");
                string Link = Content.Substring(start);
                Link = Link.Substring(Link.IndexOf("href") + 6);
                Link = Link.Substring(0, Link.IndexOf("\""));

                int end = Content.IndexOf("</a>") + "</a>".Length;
                int endImage = Content.IndexOf("</a>") + "</a>".Length;
                string ImageTemp = ImageContent.Substring(0, startImage);
                string temp = Content.Substring(0, start);
                
                if (Link.EndsWith(".jpg") || Link.EndsWith(".png"))
                {
                    ImageTemp += "\n\n(Img:" + Link + ")";
                    ImageTemp += "\n\n" + Content.Substring(endImage);
                }
                else
                {
                    string NameTemp = Content.Substring(start);
                    NameTemp = NameTemp.Substring(NameTemp.IndexOf(">") + 1);
                    NameTemp = NameTemp.Substring(0, NameTemp.IndexOf("<"));
                    ImageTemp += NameTemp;
                    ImageTemp += Content.Substring(endImage);
                    temp += NameTemp;
                }
                temp += Content.Substring(end);
                Content = temp;
                ImageContent = ImageTemp;
            }
            while (Content.Contains("<p "))
            {
                int start = Content.IndexOf("<p ");
                string temp1 = Content.Substring(start);
                int end = temp1.IndexOf("</p>") + "</p>".Length + start;
                string temp = Content.Substring(0, start);
                temp += Content.Substring(end);
                Content = temp;
            }
            string[] SplitP = Content.Split(new string[] { "<p></p>" }, StringSplitOptions.RemoveEmptyEntries);
            Content = "";
            foreach (string a in SplitP)
            {
                Content += a + "\n";
            }
            string[] SplitI = ImageContent.Split(new string[] { "<p></p>" }, StringSplitOptions.None);
            ImageContent = "";
            foreach (string a in SplitI)
            {
                ImageContent += a;
            }
            string[] SplitF = Content.Split(new string[] { "<p>", "</p>" }, StringSplitOptions.RemoveEmptyEntries);
            Content = "";
            foreach (string a in SplitF)
            {
                Content += a + "";
            }
            string[] SplitJ = ImageContent.Split(new string[] { "<p>", "</p>" }, StringSplitOptions.RemoveEmptyEntries);
            ImageContent = "";
            foreach (string a in SplitJ)
            {
                ImageContent += a + "";
            }
            string[] SplitG = Content.Split(new string[] { "\t\t\t" }, StringSplitOptions.RemoveEmptyEntries);
            Content = "";
            foreach (string a in SplitG)
            {
                Content += "" + a + "";
            }
            string[] SplitK = ImageContent.Split(new string[] { "\t\t\t" }, StringSplitOptions.RemoveEmptyEntries);
            ImageContent = "";
            foreach (string a in SplitK)
            {
                ImageContent += "" + a + "";
            }
            string[] SplitH = Content.Split(new string[] { "\n\n\n\n\n\n\n" }, StringSplitOptions.RemoveEmptyEntries);
            Content = "";
            foreach (string a in SplitH)
            {
                Content += "" + a + "\n\n";
            }
            ReadText += " " + Content;
            

            BackColor = Color.White;
            Location = new Point(100, 0);
            Size = new Size(600, 450);
            int Offset = 50;
            Boolean Override = false;
            AutoScroll = true;
            Boolean LastSpace = false;
            RichTextBox Text1 = new RichTextBox();
            Text1.Location = new Point(30, Offset);
            Controls.Add(Text1);
            Text1.ReadOnly = true;
            Text1.BorderStyle = BorderStyle.None;
            Text1.BackColor = Color.White;
            Text1.Text = Title;
            //Text.Select(10, 10);
            //Text.SelectionColor = Color.FromArgb(100, 255, 0, 0);

            Text1.Size = new Size(540, Text1.Size.Height);
            if (Text1.Text == "")
            {
                LastSpace = true;
            }
            using (Graphics g = CreateGraphics())
            {
                Text1.Height = (int)g.MeasureString(Text1.Text,
                    Text1.Font, Text1.Width).Height + 5;
            }
            Offset += Text1.Size.Height + 10;

            int WordCount = 0;
            foreach (char word in Title)
            {
                WordCount++;
            }
            string[] SplitM = Content.Split(new string[] { "tyle=\"text-align: center;\"><br />",  "tyle=\"text-align: center;\">"}, StringSplitOptions.RemoveEmptyEntries);
            Content = "";
            foreach (string a in SplitM)
            {
                Content += a + "";
            }
            string[] SplitN = ImageContent.Split(new string[] { "tyle=\"text-align: center;\"><br />", "tyle=\"text-align: center;\">" }, StringSplitOptions.RemoveEmptyEntries);
            ImageContent = "";
            foreach (string a in SplitN)
            {
                ImageContent += a + "";
            }
            Counts[LabelCount] = WordCount;
            LabelCount++;
            Synth.Rate = 0;
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
            Label Score = new Label();
            Score.Location = new Point(30, Offset);
            Score.Size = new Size(540, 100);
            Score.TextAlign = ContentAlignment.MiddleCenter;
            Score.Text = Rating;
            Score.Font = new Font(Score.Font.FontFamily, 15, FontStyle.Bold);
            Controls.Add(Score);
            Synth.SpeakAsync(ReadText);
        }

        void b_MouseEnter(object sender, EventArgs e)
        {
            Focus();
        }
    }
}
