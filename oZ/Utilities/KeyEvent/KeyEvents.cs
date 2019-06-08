using System.Windows.Forms;

namespace oZTool
{
   public static class KeyEvents
    {
       
            public static void KeyDownEvent(object sender, KeyEventArgs e)
        {
            Globals.cheat1 = (e.KeyCode == Keys.F1);
            Globals.cheat2 = (e.KeyCode == Keys.F2);
            Globals.cheat3 = (e.KeyCode == Keys.F3);
            Globals.cheat4 = (e.KeyCode == Keys.F4);

            e.Handled = true;
        }
        static int cheat1_click = 0;
        static int cheat2_click = 0;
        static int cheat3_click = 0;
        static int cheat4_click = 0;

        public static void KeyUpEvent(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                cheat1_click++;
                if (cheat1_click == 1)
                {
                    Globals.cheat1 = true;
                }
                else if (cheat1_click == 2)
                {
                    cheat1_click = 0;
                    Globals.cheat1 = false;
                }
            }
            if (e.KeyCode == Keys.F2)
            {
                cheat2_click++;
                if (cheat2_click == 1)
                {
                    Globals.cheat2 = true;
                }
                else if (cheat2_click == 2)
                {
                    cheat2_click = 0;
                    Globals.cheat2 = false;
                }
            }
            if (e.KeyCode == Keys.F3)
            {
                cheat3_click++;
                if (cheat3_click == 1)
                {
                   Globals.cheat3 = true;
                }
                else if (cheat3_click == 2)
                {
                    cheat3_click = 0;
                   Globals.cheat3 = false;
                }
            }
            if (e.KeyCode == Keys.F4)
            {
                cheat4_click++;
                if (cheat4_click == 1)
                {
                   Globals.cheat4 = true;
                }
                else if (cheat4_click == 2)
                {
                    cheat4_click = 0;
                   Globals.cheat4 = false;
                }
            }
            e.Handled = true;
        }
    }
}
