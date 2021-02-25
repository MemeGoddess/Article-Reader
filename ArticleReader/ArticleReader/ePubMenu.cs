using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
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
    class ePubMenu : BaseMenuItem
    {
        BaseMenuSystem Parent;
        Form1 PForm;
        public ePubMenu(BaseMenuSystem p, Form1 F)
        {
            Parent = p;
            PForm = F;
            BackColor = Color.FromArgb(133, 185, 22);
            CoverImage.Image = global::ArticleReader.Properties.Resources.EPUB_logo2;
            CoverImage.Click += CoverImage_Click;
            CoverImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            CoverName.Text = "ePub Reader";
            CoverName.Font = new Font(CoverName.Font, FontStyle.Bold);
            CoverName.ForeColor = Color.White;
        }
        private void CoverImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog Choose = new OpenFileDialog();
            if (Choose.ShowDialog() == DialogResult.OK)
            {
                PdfReader Reader = new PdfReader(Choose.FileName);
                MessageBox.Show(PdfTextExtractor.GetTextFromPage(Reader, 2));
                Reader.Close();
                
            }
        }
    }
}
