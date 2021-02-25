using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace ArticleReader
{
    class thejimquisitionMenu : BaseMenuItem
    {
        BaseMenuSystem Parent;
        Form1 PForm;
        public thejimquisitionMenu(BaseMenuSystem p, Form1 F)
        {
            Parent = p;
            PForm = F;
            //BackColor = Color.FromArgb(176, 40, 33);
            BackColor = Color.FromArgb(174, 0, 1);
            //var request = WebRequest.Create("https://etgeekera.files.wordpress.com/2014/11/jimquisition-logo.jpg?w=604&h=340");

            //using (var response = request.GetResponse())
            //using (var stream = response.GetResponseStream())
            {
                //CoverImage.Image = Bitmap.FromStream(stream);
                Assembly _assembly = Assembly.GetExecutingAssembly();
                Stream _imageStream = _assembly.GetManifestResourceStream("ArticleReader.Jimquisition5.png");
                CoverImage.Image = global::ArticleReader.Properties.Resources.Jimquisition5;
            }
            CoverImage.Click += CoverImage_Click;
            CoverName.Text = "The Jimquisition";
            CoverName.Font = new Font(CoverName.Font, FontStyle.Bold);
            CoverName.ForeColor = Color.White;
            
        }
        Panel View = new Panel();
        void CoverImage_Click(object sender, EventArgs e)
        {
            WebClient web = new WebClient();
            web.DownloadStringCompleted += web_DownloadStringCompleted;
            web.DownloadStringAsync(new Uri("http://www.thejimquisition.com/category/reviews/"));
            Panel Pan = new Panel();
            Pan.Size = new Size(800, 450);
            
            View.Size = new Size(600, 450);
            View.Location = new Point(100, 0);
            Parent.Controls.Add(Pan);
            Pan.Controls.Add(View);
            Pan.BackColor = Color.FromArgb(38, 38, 38);
            Pan.BringToFront();
            View.BackColor = Color.White;
            View.AutoScroll = true;

        }
        int Articles = 0;
        void web_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            string[] DataSplit = e.Result.Split(new string[] { "<article" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 1; i < DataSplit.Length; i++)
            {
                DataSplit[i] = DataSplit[i].Substring(0, DataSplit[i].IndexOf("</article>"));
                string Name = DataSplit[i].Substring(DataSplit[i].IndexOf("title=") + "title=\"".Length);
                Name = Name.Substring(0, Name.IndexOf("\""));
                string Link = DataSplit[i].Substring(DataSplit[i].IndexOf("src=") + "src=\"".Length);
                Link = Link.Substring(0, Link.IndexOf("\""));
                Name = RemoveHashCode(Name);
                string ArticleLink = DataSplit[i].Substring(DataSplit[i].IndexOf("href=") + "href=\"".Length);
                ArticleLink = ArticleLink.Substring(0, ArticleLink.IndexOf("\""));
                thejimquisitionMenuArticle Art = new thejimquisitionMenuArticle(Link, Name, ArticleLink, PForm, Parent);
                Art.Location = new Point(0, (Art.Size.Height + 20) * Articles);
                View.Controls.Add(Art);
                Articles++;
            }

        }

        public string RemoveHashCode(string RemoveString)
        {
            // '
            while (RemoveString.Contains("&#8217;") || RemoveString.Contains("&#8216;"))
            {
                string[] Split = RemoveString.Split(new string[] { "&#8217;", "&#8216;" }, StringSplitOptions.RemoveEmptyEntries);
                RemoveString = "";
                for (int i = 0; i < Split.Length; i++)
                {
                    RemoveString += Split[i];
                    if (i != Split.Length - 1)
                    {
                        RemoveString += "'";
                    }
                }
            }

            // "
            while (RemoveString.Contains("&#8220;") || RemoveString.Contains( "&#8221;"))
            {
                string[] Split = RemoveString.Split(new string[] { "&#8220;", "&#8221;" }, StringSplitOptions.RemoveEmptyEntries);
                RemoveString = "";
                for (int i = 0; i < Split.Length; i++)
                {
                    RemoveString += Split[i];
                    if (i != Split.Length - 1)
                    {
                        RemoveString += "\"";
                    }
                }
            }

            // -
            while (RemoveString.Contains("&#8212;") || RemoveString.Contains("&#8211;"))
            {
                string[] Split = RemoveString.Split(new string[] { "&#8212;", "&#8211;" }, StringSplitOptions.RemoveEmptyEntries);
                RemoveString = "";
                for (int i = 0; i < Split.Length; i++)
                {
                    RemoveString += Split[i];
                    if (i != Split.Length - 1)
                    {
                        RemoveString += "-";
                    }
                }
            }

            // ...
            while (RemoveString.Contains("&#8230;"))
            {
                string[] Split = RemoveString.Split(new string[] { "&#8230;" }, StringSplitOptions.RemoveEmptyEntries);
                RemoveString = "";
                for (int i = 0; i < Split.Length; i++)
                {
                    RemoveString += Split[i];
                    if (i != Split.Length - 1)
                    {
                        RemoveString += "...";
                    }
                }
            }
            return RemoveString;
        }
    }
}
