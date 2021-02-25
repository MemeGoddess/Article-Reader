using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArticleReader
{
    class BaseMenuItem : Panel
    {
        public PictureBox CoverImage = new PictureBox();
        public Label CoverName = new Label();
        public Boolean ItemMenu = true;
        public BaseMenuItem()
        {
            CoverImage.SizeMode = PictureBoxSizeMode.StretchImage;
            Size = new Size(128, 85);
            Controls.Add(CoverImage);
            CoverImage.Size = new Size(128, 72);
            Click += CoverImage_Click;
            CoverName.Location = new Point(0, 72);
            CoverName.Size = new Size(128, 15);
            Controls.Add(CoverName);
            CoverName.TextAlign = ContentAlignment.TopCenter;
            CoverImage.Click += CoverImage_Click;
            CoverName.Click += CoverImage_Click;
        }

        void CoverImage_Click(object sender, EventArgs e)
        {
            
        }
    }
}
