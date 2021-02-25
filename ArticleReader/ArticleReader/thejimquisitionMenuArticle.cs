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
    class thejimquisitionMenuArticle : Panel
    {
        Form1 Form;
        BaseMenuSystem Base;
        string Link;
        public thejimquisitionMenuArticle(string imgURL, string Name, string ArticleLink, Form1 F, BaseMenuSystem B)
        {
            Base = B;
            Form = F;
            Link = ArticleLink;
            var request = WebRequest.Create(imgURL);
            PictureBox CoverImage = new PictureBox();
            Label CoverName = new Label();
            using (var response = request.GetResponse())
            using (var stream = response.GetResponseStream())
            {
                CoverImage.Image = Bitmap.FromStream(stream);
            }
            CoverName.Text = Name;
            Size = new Size((16 * 36) + 7, 9 * 40);
            CoverImage.Size = new Size((16 * 36) + 7, 9 * 36);
            CoverImage.Location = new Point(0, 0);
            CoverName.Location = new Point(0, 9 * 36);
            CoverName.Size = new Size(16 * 36, 9 * 4);
            CoverName.Font = new Font(CoverName.Font.FontFamily, 20, FontStyle.Bold);
            CoverName.TextAlign = ContentAlignment.MiddleCenter;
            CoverName.ForeColor = Color.FromArgb(68, 68, 68);
            Controls.Add(CoverImage);
            Controls.Add(CoverName);
            CoverImage.SizeMode = PictureBoxSizeMode.StretchImage;
            CoverImage.Click += CoverImage_Click;
            CoverName.Click += CoverImage_Click;
            
        }

        void CoverImage_Click(object sender, EventArgs e)
        {
            Form.Article = new thejimquisition(Link, Base.panel1);
            Form.Article.BringToFront();
            Form.Synth = Form.Article.Synth;
            Base.Dispose();
        }
    }
}
