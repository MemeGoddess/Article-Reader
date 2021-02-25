using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Svg;
using System.IO;

namespace ArticleReader
{
    class TemplateArticle : BaseArticle
    {
        public Color BackColorA = Color.FromArgb(0, 0, 0, 0);
        string Template;
        WebBrowser Web = new WebBrowser();
        public TemplateArticle(string URL, Panel panel1, string template)
        {
            Size = new Size(600, 450);
            Location = new Point(100, 0);
            BackColor = Color.White;
            AutoScroll = true;
            Template = template;
            panel1.BackColor = BackColorA;
            panel1.Controls.Add(this);
            ReadText = "";
            Web.ScriptErrorsSuppressed = true;
            Web.Navigate(URL);
            Web.DocumentCompleted += webBrowser1_DocumentCompleted;
            Timer t = new Timer();
            t.Interval = 1;
            t.Tick += t_Tick;
            t.Start();
        }

        void t_Tick(object sender, EventArgs e)
        {
            if (Web.Document != null)
            {
                Web.Document.Window.Error +=
     new HtmlElementErrorEventHandler(Window_Error);
                Timer t = (Timer)sender;
                t.Dispose();
            }
        }
        Boolean Done = false;
        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            ((WebBrowser)sender).Document.Window.Error +=
        new HtmlElementErrorEventHandler(Window_Error);
            if (!Done)
            {
                Done = true;
                string[] Identifiers = Template.Split(',')[1].Split('|');
                string Tag = "", Class = "", ID = "";
                foreach (string a in Identifiers)
                {
                    if (a.StartsWith("t:"))
                    {
                        Tag = a.Substring(2);
                    }
                    else if (a.StartsWith("c:"))
                    {
                        Class = a.Substring(2);
                    }
                    else if (a.StartsWith("i:"))
                    {
                        ID = a.Substring(2);
                    }
                }
                HtmlDocument doc = Web.Document;
                foreach (HtmlElement b in doc.GetElementsByTagName(Tag))
                {
                    //ID
                    int IDMatch = 0; //0 = match; 1 = single variable; 2 = double variable; 3 = match

                    if (ID != "")
                    {
                        IDMatch += 1;
                    }
                    if (b.Id != null)
                    {
                        IDMatch += 1;
                    }
                    if (b.Id == ID)
                    {
                        IDMatch += 1;
                    }

                    //Class
                    int ClassMatch = 0; //0 = match; 1 = single variable; 2 = double variable; 3 = match
                    string ClassTemp = b.GetAttribute("className");
                    if (Class != "")
                    {
                        ClassMatch += 1;
                    }
                    if (b.GetAttribute("className") != "")
                    {
                        ClassMatch += 1;
                    }
                    if (b.GetAttribute("className") == Class)
                    {
                        ClassMatch += 1;
                    }
                    if ((ClassMatch == 0 || ClassMatch == 3) && (IDMatch == 0 || IDMatch == 3))
                    {
                        Selected = b;
                    }
                }

                BuildArticle(GetArticle());
            }
        }

        private void Window_Error(object sender, HtmlElementErrorEventArgs e)
        {
            e.Handled = true;
        }
        HtmlElement Selected;
        public string GetArticle()
        {
            string[] Paragraphs = Selected.InnerHtml.Split(new string[] { "<P>"}, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < Paragraphs.Length; i++)
            {
                while (Paragraphs[i].Contains("class=credit"))
                {
                    string Temp = Paragraphs[i].Substring(0, Paragraphs[i].IndexOf("class=credit"));
                    int Start = Temp.LastIndexOf("<");
                    string Tag = Temp.Substring(Start + 1);
                    Tag = Tag.Substring(0, Tag.IndexOf(" "));
                    string Backup = Paragraphs[i];
                    int TagStart = Backup.IndexOf(Tag) - 1;
                    string TagIndexEnd = Backup.Substring(TagStart + 1 + Tag.Length);
                    int TagEnd = TagIndexEnd.IndexOf("</" + Tag) + "</".Length + Tag.Length + 2 + Tag.Length;
                    Paragraphs[i] = Backup.Substring(0, TagStart);
                    Paragraphs[i] += Backup.Substring(TagStart + TagEnd);
                }
                while (Paragraphs[i].Contains("<IMG"))
                {
                    string Backup = Paragraphs[i];
                    int start = Backup.IndexOf("<IMG");
                    string IndexEnd = Paragraphs[i].Substring(start);
                    int end = IndexEnd.IndexOf(">") + ">".Length;
                    string temp = Paragraphs[i].Substring(Paragraphs[i].IndexOf("<IMG") + "<IMG".Length);
                    string Link = "";
                    if (Paragraphs[i].Contains("srcset=\""))
                    {
                        Link = temp.Substring(temp.IndexOf("srcset=") + 8);
                        Link = Link.Substring(Link.IndexOf(",") + 2);
                        Link = Link.Substring(0, Link.IndexOf(" "));
                    }
                    else
                    {
                        Link = temp.Substring(temp.IndexOf("src=") + "src=\"".Length);
                        Link = Link.Substring(0, Link.IndexOf("\""));
                    }
                        Paragraphs[i] = Backup.Substring(0, start);
                        Paragraphs[i] += "\n(Img:" + Link + ")\n";
                        Paragraphs[i] += Backup.Substring(start + end);
                    
                }
                while (Paragraphs[i].Contains("<DIV"))
                {
                    int start = Paragraphs[i].IndexOf("<DIV");
                    string IndexEnd = Paragraphs[i].Substring(start);
                    int end = IndexEnd.IndexOf("</DIV>") + "</DIV>".Length;
                    string temp = Paragraphs[i].Substring(0, start);
                    temp += Paragraphs[i].Substring(start + end);
                    Paragraphs[i] = temp;
                }
                while (Paragraphs[i].Contains("<ASIDE"))
                {
                    string Backup = Paragraphs[i];
                    int start = Backup.IndexOf("<ASIDE");
                    string IndexEnd = Paragraphs[i].Substring(start);
                    int end = IndexEnd.IndexOf("/ASIDE>") + "/ASIDE>".Length;
                    string temp = Paragraphs[i].Substring(Paragraphs[i].IndexOf("<ASIDE") + "<ASIDE".Length);
                    Paragraphs[i] = Backup.Substring(0, start);
                    Paragraphs[i] += Backup.Substring(start + end);
                }
                while (Paragraphs[i].Contains("<"))
                {
                    int start = Paragraphs[i].IndexOf("<");
                    string IndexEnd = Paragraphs[i].Substring(start);
                    int end = IndexEnd.IndexOf(">") + ">".Length;
                    string temp = Paragraphs[i].Substring(0, start);
                    temp += Paragraphs[i].Substring(start + end);
                    Paragraphs[i] = temp;
                }
            }
            string read = "";
            foreach (string a in Paragraphs)
            {
                read += a + "\n\n";
            }
            read = RemoveCodes(read);
            return read;

        }
        public string RemoveCodes(string RemoveString)
        {
            // '
            while (RemoveString.Contains("&nbsp;"))
            {
                string[] Split = RemoveString.Split(new string[] { "&nbsp;" }, StringSplitOptions.RemoveEmptyEntries);
                RemoveString = "";
                for (int i = 0; i < Split.Length; i++)
                {
                    RemoveString += Split[i];
                    if (i != Split.Length - 1)
                    {
                        RemoveString += " ";
                    }
                }
            }

            return RemoveString;
        }
        int Offset = 50;
        Boolean LastSpace = false;
        int WordCount = 0;
        Boolean SVGAlert = false;
        public void BuildArticle(string Article)
        {
            foreach (string a in Article.Split('\n'))
            {
                if (a.StartsWith("(Img:"))
                {
                    if (!a.EndsWith(".svg)"))
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
                        b.MouseEnter += b_MouseEnter;
                    }
                    else
                    {
                        try
                        {
                            WebClient Web = new WebClient();
                            Directory.CreateDirectory("Temp");
                            Web.DownloadFile(a.Substring(5, a.Length - 6), @"Temp\Image.svg");
                            SvgDocument svgdoc = SvgDocument.Open(@"Temp\Image.svg");
                            PictureBox b = new PictureBox();
                            b.Size = new Size(540, 300);
                            b.SizeMode = PictureBoxSizeMode.StretchImage;
                            Controls.Add(b);
                            b.Location = new Point(30, Offset);
                            Offset += 320;
                            b.MouseEnter += b_MouseEnter;
                            b.Image = svgdoc.Draw();

                        }
                        catch (Exception e)
                        {
                            if (!SVGAlert)
                            {
                                MessageBox.Show("This page uses .SVG images, which are currently not supported. - " + e.Message);
                                SVGAlert = true;
                            }
                        }
                    }
                }
                else
                {
                    if (!LastSpace || a != "")
                    {
                        Boolean FoundText = false;
                        foreach (char c in a)
                        {
                            if (c != '\t' && c != '\r')
                            {
                                FoundText = true;
                            }
                        }
                        if (FoundText)
                        {
                            ReadText += a + "\n";
                            RichTextBox Text = new RichTextBox();
                            Text.Location = new Point(30, Offset);
                            Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
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
