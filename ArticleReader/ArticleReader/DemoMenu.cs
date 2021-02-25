using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArticleReader
{
    class DemoMenu : BaseMenuItem
    {
        new BaseMenuSystem baseMenuSystem;
        Form1 form1;

        public DemoMenu(BaseMenuSystem p, Form1 F)
        {
            baseMenuSystem = p;
            form1 = F;
            BackColor = Color.FromArgb(174, 0, 1);
            CoverImage.BackColor = Color.FromArgb(174, 0, 1);
            CoverImage.Click += CoverImage_Click;
            CoverImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            CoverName.Text = "Demo";
            CoverName.Font = new Font(CoverName.Font, FontStyle.Bold);
            CoverName.ForeColor = Color.White;
        }

        private void CoverImage_Click(object sender, EventArgs e)
        {
            form1.textBox1.Text = "https://arstechnica.com/gadgets/2020/11/macos-11-0-big-sur-the-ars-technica-review/";
            form1.button1.PerformClick();
        }
    }
}
