using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArticleReader
{
    class TemplateMenu : BaseMenuItem
    {
        BaseMenuSystem Parent;
        Form1 PForm;
        public TemplateMenu(BaseMenuSystem p, Form1 F)
        {
            Parent = p;
            PForm = F;
            BackColor = Color.FromArgb(133, 185, 221);
            //CoverImage.Image = Image.FromFile(@"D:\Projects\Assets\ArticleReader\EPUB_logo2.png");
            CoverImage.BackColor = Color.FromArgb(133, 185, 221);
            CoverImage.Click += CoverImage_Click;
            CoverImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            CoverName.Text = "Template";
            CoverName.Font = new Font(CoverName.Font, FontStyle.Bold);
            CoverName.ForeColor = Color.White;
        }

        private void CoverImage_Click(object sender, EventArgs e)
        {
            TemplateForm TForm = new TemplateForm();
            TForm.ShowDialog();
        }
    }
}
