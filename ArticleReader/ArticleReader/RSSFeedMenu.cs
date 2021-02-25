using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArticleReader
{
    class RSSFeedMenu : BaseMenuItem
    {
        BaseMenuSystem Parent;
        Form1 PForm;
        public RSSFeedMenu(BaseMenuSystem p, Form1 F)
        {
            Parent = p;
            PForm = F;
            BackColor = Color.FromArgb(255, 102, 0);
            CoverImage.BackColor = BackColor;
            CoverImage.Click += CoverImage_Click;
            CoverName.Text = "RSS Feed";
            CoverName.Font = new Font(CoverName.Font, FontStyle.Bold);
            CoverName.ForeColor = Color.White;
        }

        void CoverImage_Click(object sender, EventArgs e)
        {
            Panel Pan = new Panel();
        }
    }
}
