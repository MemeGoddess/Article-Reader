using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArticleReader
{
    class BaseMenuSystem : Panel
    {
        public Panel panel1;
        public BaseMenuSystem(Panel pan)
        {
            panel1 = pan;
            Size = new Size(800, 450);
            BackColor = Color.Transparent;
        }
        int X = 0;
        int Y = 0;
        public void AddItem(BaseMenuItem Item)
        {
            if (X > 4)
            {
                Y++;
                X = 0;
            }
            Controls.Add(Item);
            Item.Location = new Point(40 + ((Item.Size.Width + 40) * X), 40 + (150 * Y));
            X++;
        }
    }
}
