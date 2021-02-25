using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArticleReader
{
    public partial class TemplateForm : Form
    {
        public TemplateForm()
        {
            InitializeComponent();
            panel3.Visible = false;
            BackColor = Color.White;
            button2.Enabled = false;
            panel3.AutoScroll = true;
        }
        int ID = 0;
        Timer t = new Timer();
        private void TemplateForm_Load(object sender, EventArgs e)
        {
            if (File.Exists("Templates.txt"))
            {
                foreach(string a in File.ReadAllLines("Templates.txt"))
                {
                    string[] Split = a.Split(',');
                    panel1.Controls.Add(new Entry(Split[0], Split[1], panel1, ID));
                    ID++;
                }
            }
            else
            {
                File.Create("Templates.txt");
                
            }
            webBrowser1.ScriptErrorsSuppressed = false;
            webBrowser1.Navigating += webBrowser1_Navigating;
            webBrowser1.Dispose();
            
            t.Interval = 1;
            t.Tick += t_Tick;
        }

        void t_Tick(object sender, EventArgs e)
        {
            if (webBrowser1.Document != null)
            {
                webBrowser1.Document.Window.Error +=
     new HtmlElementErrorEventHandler(Window_Error);
                Timer t = (Timer)sender;
                t.Dispose();
            }
        }
        void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            
        }

        private void Window_Error(object sender, HtmlElementErrorEventArgs e)
        {
            e.Handled = true;
        }

        int Stage = 0;
        private void button1_Click(object sender, EventArgs e)
        {
            switch (Stage)
            {
                case 0:
                    string Address = ShowMyDialogBox();
                    if (Address != "Cancelled")
                    {
                        webBrowser1 = new WebBrowser();
                        webBrowser1.Size = new Size(630, 510);
                        panel2.Controls.Add(webBrowser1);
                        webBrowser1.Navigate(Address);
                        t.Start();
                        webBrowser1.DocumentCompleted += webBrowser1_DocumentCompleted;
                        webBrowser1.Visible = true;
                        panel2.Visible = true;
                        button1.Text = "Import";
                        button1.Enabled = false;
                        Stage = 1;
                        button2.Enabled = true;
                        button2.Text = "Cancel";
                    }
                    break;
                case 1:
                    webBrowser1.Visible = false;
                    panel3.Visible = true;
                    string Article = GetArticle();
                    BuildArticle(Article);
                    Stage = 2;
                    button2.Text = "Retry";
                    button1.Text = "Finish";
                    break;
                case 2:
                    string Website = webBrowser1.Url.ToString();
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
                    string SaveText = "";
                    if (Selected.TagName != "")
                    {
                        SaveText += "t:" + Selected.TagName;
                    }
                    if (Selected.GetAttribute("className") != "")
                    {
                        SaveText += "|c:" + Selected.GetAttribute("className");
                    }
                    if (Selected.Id != null)
                    {
                        SaveText += "|i:" + Selected.Id;
                    }
                    File.AppendAllText("Templates.txt", Website + "," + SaveText + "\r\n");
                    panel2.Visible = false;
                    button1.Text = "Add Template";
                    button2.Text = "Cancel";
                    button2.Enabled = false;
                    panel3.Visible = false;
                    Stage = 0;
                    webBrowser1.Dispose();
                    panel1.Controls.Clear();
                    ID = 0;
                    if (File.Exists("Templates.txt"))
                    {
                        foreach (string a in File.ReadAllLines("Templates.txt"))
                        {
                            string[] Split = a.Split(',');
                            panel1.Controls.Add(new Entry(Split[0], Split[1], panel1, ID));
                            ID++;
                        }
                    }
                    break;
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            switch (Stage)
            {
                case 1:
                    button2.Text = "Cancel";
                    button2.Enabled = false;
                    button1.Text = "Add Template";
                    button1.Enabled = true;
                    panel2.Visible = false;
                    webBrowser1.Visible = false;
                    Stage = 0;
                    break;
                case 2:
                    webBrowser1.Visible = true;
                    panel3.Visible = false;
                    panel3.Controls.Clear();
                    Stage = 1;
                    button1.Text = "Import";
                    button2.Text = "Cancel";
                    break;
            }
        }
        
        public void BuildArticle(string Article)
        {
            Boolean SVGAlert = false;
            Boolean LastSpace = false;
            int Offset = 50;
            foreach (string a in Article.Split('\n'))
            {
                if (a.StartsWith("(Img:"))
                {
                    if (!a.EndsWith(".svg)"))
                    {
                        try
                        {
                            PictureBox b = new PictureBox();
                            b.Size = new Size(540, 300);
                            b.SizeMode = PictureBoxSizeMode.StretchImage;
                            var request = WebRequest.Create(a.Substring(5, a.Length - 6));

                            using (var response = request.GetResponse())

                            using (var stream = response.GetResponseStream())
                            {
                                ImageConverter Img = new ImageConverter();

                                b.Image = Bitmap.FromStream(stream);
                            }
                            panel3.Controls.Add(b);
                            b.Location = new Point(30, Offset);
                            Offset += 320;
                            LastSpace = false;
                            b.MouseEnter += b_MouseEnter;
                        }
                        catch (Exception e1)
                        {

                        }
                    }
                    else if (!SVGAlert)
                    {
                        MessageBox.Show("This page uses .SVG images, which are currently not supported.");
                        SVGAlert = true;
                    }
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
                            Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
                            panel3.Controls.Add(Text);
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
                        }
                    }
                }
            }
        }
        void b_MouseEnter(object sender, EventArgs e)
        {
            Focus();
        }
        HtmlDocument document;
        void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            this.document = this.webBrowser1.Document;
            this.document.MouseOver += new HtmlElementEventHandler(document_MouseOver);
            this.document.MouseLeave += new HtmlElementEventHandler(document_MouseLeave);
            this.document.Click += new HtmlElementEventHandler(document_MouseClick);
        }

        public string GetArticle()
        {
            string[] Paragraphs = Selected.InnerHtml.Split(new string[] { "<P>" }, StringSplitOptions.RemoveEmptyEntries);
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
                    string Link = temp.Substring(temp.IndexOf("src=") + "src=\"".Length);
                    Link = Link.Substring(0, Link.IndexOf("\""));
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
                    int start = Paragraphs[i].IndexOf("<ASIDE");
                    string IndexEnd = Paragraphs[i].Substring(start);
                    int end = IndexEnd.IndexOf(">") + ">".Length;
                    string temp = Paragraphs[i].Substring(0, start);
                    temp += Paragraphs[i].Substring(start + end);
                    Paragraphs[i] = temp;
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
                        RemoveString += "";
                    }
                }
            }

            return RemoveString;
        }

        HtmlElement Selected;

        private void document_MouseClick(object sender, HtmlElementEventArgs e)
        {
            if (Selected != null)
            {
                if (this.elementStyles.ContainsKey(Selected))
                {
                    string style = this.elementStyles[Selected];
                    this.elementStyles.Remove(Selected);
                    Selected.Style = style;
                }
            }
            HtmlElement element = b;
            if (!this.elementStyles.ContainsKey(element))
            {
                string style = element.Style;
                this.elementStyles.Add(element, style);
                element.Style = style + "; background-color: #85B9DD;";
                this.Text = element.Id ?? "(no id)";
                Selected = b;
                button1.Enabled = true;
            }
        }
        private IDictionary<HtmlElement, string> elementStyles = new Dictionary<HtmlElement, string>();

        private void document_MouseLeave(object sender, HtmlElementEventArgs e)
        {
            HtmlElement element = e.FromElement;
            if (this.elementStyles.ContainsKey(element))
            {
                string style = this.elementStyles[element];
                this.elementStyles.Remove(element);
                element.Style = style;
            }
        }
        HtmlElement b;
        private void document_MouseOver(object sender, HtmlElementEventArgs e)
        {
            HtmlElement element = e.ToElement.Parent;
            if (element != null)
            {
                b = element;
            }
        }

        public string ShowMyDialogBox()
        {
            TextInput testDialog = new TextInput();
            string TextIn = "";
            if (testDialog.ShowDialog(this) == DialogResult.OK)
            {
                TextIn = testDialog.textBox1.Text;
            }
            else
            {
                TextIn = "Cancelled";
            }
            testDialog.Dispose();
            return TextIn;
        }

    }
    public class Entry : Panel
    {
        TextBox WebBox = new TextBox();
        TextBox ElemetBox = new TextBox();
        public Entry(string Web, string Element, Panel panel1, int ID)
        {
            Location = new Point(0, 15 * ID);
            Label WebLabel = new Label();
            Label ElementLabel = new Label();
            WebLabel.Text = Web;
            ElementLabel.Text = Element;
            WebLabel.AutoSize = false;
            ElementLabel.AutoSize = false;
            
            WebBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            ElemetBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            Controls.Add(WebBox);
            Controls.Add(ElemetBox);
            Controls.Add(WebLabel);
            Controls.Add(ElementLabel);
            WebBox.Size = new Size((panel1.Size.Width / 2) - 2, 20);
            WebLabel.Size = new Size((panel1.Size.Width / 2) - 2, 20);
            ElemetBox.Size = new Size((panel1.Size.Width / 2) - 2, 20);
            ElementLabel.Size = new Size((panel1.Size.Width / 2) - 2, 20);
            ElemetBox.Location = new Point(panel1.Size.Width / 2, 0);
            ElementLabel.Location = new Point(panel1.Size.Width / 2, 0);
            WebBox.Location = new Point(3, 0);
            BackColor = Color.White;
            Size = new Size(panel1.Size.Width, 15);
            ElemetBox.BringToFront();
            
            WebBox.Text = Web;
            ElemetBox.Text = Element;
            WebBox.Visible = false;
            ElemetBox.Visible = false;
            WebLabel.Click += WebBox_Click;
            foreach (Control a in Controls)
            {
                a.MouseEnter += WebLabel_MouseHover;
                a.MouseLeave += Entry_MouseLeave;
            }
            MouseEnter += WebLabel_MouseHover;
            MouseLeave += Entry_MouseLeave;
        }

        void Entry_MouseLeave(object sender, EventArgs e)
        {
            BackColor = Color.White;
            WebBox.BackColor = BackColor;
            ElemetBox.BackColor = BackColor;
        }

        void WebLabel_MouseHover(object sender, EventArgs e)
        {
            BackColor = Color.FromArgb(153, 205, 239);
            WebBox.BackColor = BackColor;
            ElemetBox.BackColor = BackColor;
        }

        void WebBox_Click(object sender, EventArgs e)
        {
            WebBox.Visible = true;
            WebBox.Focus();
            WebBox.SelectAll();
        }
    }
}
