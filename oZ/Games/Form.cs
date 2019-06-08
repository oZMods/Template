using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;
using Memory;
using WindowsInput;
using WindowsInput.Native;
using static Memory.Mem;

namespace oZTool {
 
        #region DontTouchThisRegion
    public partial class FormoZ : Form {
        public FormoZ()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;

            try
            {
                SecurityPermission sp = new SecurityPermission(SecurityPermissionFlag.UnmanagedCode);
                sp.Demand();
            }
            catch (Exception ex)
            {
                fConsole.WriteLine("Demand for SecurityPermissionFlag.UnmanagedCode failed: " + ex.Message);
            }
        }
        #endregion
   
        #region Fields
        public static GlobalKeyboardHook gkh = new GlobalKeyboardHook();
        public static int cheat1_click = 0;
        public static int cheat2_click = 0;
        public static int cheat3_click = 0;
        public static int cheat4_click = 0;
        public static int cheat5_click = 0;
        public static int cheat6_click = 0;
        public static int cheat7_click = 0;
        public static int cheat8_click = 0;
        public static int cheat9_click = 0;
        public static int Vis_click = 0;
       
        public static Mem MemLib = new Mem();
        public bool Attached = false;
        public static string procName = "TheChase-Win64-Shipping";
        public static InputSimulator InSim = new InputSimulator();
        public static volatile bool isRunning;
        #endregion
     
        #region Framework
        private void oZ_Load(object sender, EventArgs e)
        {

        }

        private void BackgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {

            while (MemLib.OpenProcess(procName))
            {

                procID.Text = "ProcessID: " + MemLib.theProc.Id;
                HID.Text = "HandleID" + MemLib.theProc.Handle;
                BA.Text = "BaseAddresss: " + MemLib.theProc.MainModule.BaseAddress.ToString("X");

                if (Globals.Stam == true)
                {
                    StamBox.Checked = true;
                }
                else if (Globals.Stam == false)
                {
                    StamBox.Checked = false;
                }

                if (Globals.HP == true)
                {
                    HealthBox.Checked = true;
                }
                else if (Globals.HP == false)
                {
                    HealthBox.Checked = false;
                }

                if (Globals.RG == true)
                {
                    RageBox.Checked = true;
                }
                else if (Globals.RG == false)
                {
                    RageBox.Checked = false;
                }

                if (Globals.MP == true)
                {
                    ManaBox.Checked = true;
                }
                else if (Globals.MP == false)
                {
                    ManaBox.Checked = false;
                }

                if (MemLib.theProc.HasExited)
                {
                    Dispose();
                    Application.Exit();
                }
            }
        }

        private void StartThreads()
        {
            Process process = MemLib.theProc;

            AoB S7 = new AoB(process, process.Handle, "F3 44 0F 11 99 A4 17 00 00");
            S7.setModule(process.MainModule);
            S7.FindPattern();
            var getS7 = S7.FindPattern();
            var S7Math = getS7 - (ulong)MemLib.theProc.MainModule.BaseAddress;
            fConsole.WriteLine(getS7.ToString("X"));
            fConsole.WriteLine(S7Math.ToString("X"));
            Globals.S7Pointer = S7Math.ToString("X");
            Globals.S7Offset = getS7.ToString("X");
            fConsole.WriteLine("---------");

            AoB S6 = new AoB(process, process.Handle, "F3 44 0F 11 A9 AC 17 00 00");
            S6.setModule(process.MainModule);
            S6.FindPattern();
            var getS6 = S6.FindPattern();
            var S6Math = getS6 - (ulong)MemLib.theProc.MainModule.BaseAddress;
            fConsole.WriteLine(getS6.ToString("X"));
            fConsole.WriteLine(S6Math.ToString("X"));
            Globals.S6Pointer = S6Math.ToString("X");
            Globals.S6Offset = getS6.ToString("X");
            fConsole.WriteLine("---------");

            AoB S5 = new AoB(process, process.Handle, "F3 44 0F 5E F0 0F 28 C2");
            S5.setModule(process.MainModule);
            S5.FindPattern();
            var getS5 = S5.FindPattern();
            var S5Math = getS5 - (ulong)MemLib.theProc.MainModule.BaseAddress;
            fConsole.WriteLine(getS5.ToString("X"));
            fConsole.WriteLine(S5Math.ToString("X"));
            Globals.S5Pointer = S5Math.ToString("X");
            Globals.S5Offset = getS5.ToString("X");
            fConsole.WriteLine("---------");

            AoB S4 = new AoB(process, process.Handle, "F3 44 0F 11 91 9C 17 00 00");
            S4.setModule(process.MainModule);
            S4.FindPattern();
            var getS4 = S4.FindPattern();
            var S4Math = getS4 - (ulong)MemLib.theProc.MainModule.BaseAddress;
            fConsole.WriteLine(getS4.ToString("X"));
            fConsole.WriteLine(S4Math.ToString("X"));
            Globals.S4Pointer = S4Math.ToString("X");
            Globals.S4Offset = getS4.ToString("X");
            fConsole.WriteLine("---------");

            gkh.HookedKeys.Add(Keys.F1);
            gkh.HookedKeys.Add(Keys.F2);
            gkh.HookedKeys.Add(Keys.F3);
            gkh.HookedKeys.Add(Keys.F4);
            gkh.HookedKeys.Add(Keys.F5);
            gkh.HookedKeys.Add(Keys.F6);
            gkh.HookedKeys.Add(Keys.F7);
            gkh.HookedKeys.Add(Keys.F8);
            gkh.HookedKeys.Add(Keys.F9);
            gkh.HookedKeys.Add(Keys.Insert);

            gkh.KeyDown += new KeyEventHandler(KeyDownEvent);
            gkh.KeyUp += new KeyEventHandler(Hotkey1);
            gkh.KeyUp += new KeyEventHandler(Hotkey2);
            gkh.KeyUp += new KeyEventHandler(Hotkey3);
            gkh.KeyUp += new KeyEventHandler(Hotkey4);
            gkh.KeyUp += new KeyEventHandler(Hotkey5);
            gkh.KeyUp += new KeyEventHandler(Hotkey6);
            gkh.KeyUp += new KeyEventHandler(Hotkey7);
            gkh.KeyUp += new KeyEventHandler(Hotkey8);
            gkh.KeyUp += new KeyEventHandler(Hotkey9);

        }
        #endregion

        #region Buttons
        private void MaterialFlatButton1_Click(object sender, EventArgs e)
        {
            {
                int count = 0;
                bool success = false;

                do
                {
                    MemLib.OpenProcess(procName);
                    if (MemLib.OpenProcess(procName))
                    {
                        try
                        {
                            IntPtr handle = (IntPtr)MemLib.theProc.Id;
                            if (handle != IntPtr.Zero)
                            {
                                success = true;
                                fConsole.Clear();
                                fConsole.WriteLine("Attached");
                                fConsole.WriteLine("---------");
                            }
                            else if (success == false)
                            {
                                fConsole.WriteLine("Could not Attach");
                            }
                        }

                        catch (Exception)
                        {
                            fConsole.WriteLine("Could not Attach to Process");
                        }
                    }

                    else
                    {
                        if (count++ == 0)
                        {
                            fConsole.Clear();
                            fConsole.Write($"Attaching. \n");
                            fConsole.WriteLine("---------");

                        }
                        else if (count < 3)
                        {
                            fConsole.Clear();
                            fConsole.Write($"Attaching.. \n");
                            fConsole.WriteLine("---------");

                        }
                        else if (count < 4)
                        {
                            fConsole.Clear();
                            fConsole.Write($"Attaching... \n");
                            fConsole.WriteLine("---------");

                            count = 0;
                        }
                        Thread.Sleep(1000);
                    }
                }

                while (!success);
                isRunning = true;
                Attached = true;
                StartThreads();
                if (!backgroundWorker1.IsBusy)
                    backgroundWorker1.RunWorkerAsync();
            }
        }
        private void CloseBtn_Click(object sender, EventArgs e)
        {
            MemLib.closeProcess();
            Application.Exit();
        }
        private void MetroToggle1_CheckedChanged(object sender, EventArgs e)
        {
            if (Attached is true)
            {
                Globals.StopOverlay = false;
                new Thread(() => new OVL().ShowDialog()).Start();
                fConsole.WriteLine("Overlay Starting");
                fConsole.WriteLine("---------");
            }
            else if (Attached is false)
            {
                fConsole.Clear();
                fConsole.WriteLine("Cannot start Overlay without first Attaching to a Process");
                fConsole.WriteLine("---------");

                OverlayBox.Checked = false;
            }
        }
        private void RestartButton_Click(object sender, EventArgs e)
        {
            if (Attached is true)
            {
                Application.Restart();
            }
            else if (Attached is false)
            {
                fConsole.WriteLine("No reason to restart - haven't attached yet");
            }
        }
        #endregion

        #region Keys
        public static void KeyDownEvent(object sender, KeyEventArgs e)
        {
            Globals.cheat1 = (e.KeyCode == Keys.F1);
            Globals.cheat2 = (e.KeyCode == Keys.F2);
            Globals.cheat3 = (e.KeyCode == Keys.F3);
            Globals.cheat4 = (e.KeyCode == Keys.F4);
            Globals.cheat5 = (e.KeyCode == Keys.F5);
            Globals.cheat6 = (e.KeyCode == Keys.F6);
            Globals.cheat7 = (e.KeyCode == Keys.F7);
            Globals.cheat8 = (e.KeyCode == Keys.F8);
            Globals.cheat9 = (e.KeyCode == Keys.F9);
            
            Globals.Vis = (e.KeyCode == Keys.Insert);


            e.Handled = true;
        }
        public static void Hotkey1(object sender, KeyEventArgs e)
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
                if (Globals.cheat1 == true)
                {
                    Globals.cheat01 = true;
                }
                else if (Globals.cheat1 == false)
                {
                    Globals.cheat01 = false;
                }
            }
            e.Handled = true;
        }
        public static void Hotkey2(object sender, KeyEventArgs e)
        {
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
                if (Globals.cheat2 == true)
                {
                    Globals.cheat02 = true;
                }
                else if (Globals.cheat2 == false)
                {
                    Globals.cheat02 = false;
                }
            }
            e.Handled = true;
        }
        public static void Hotkey3(object sender, KeyEventArgs e)
        {
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
                if (Globals.cheat3 == true)
                {
                    Globals.cheat03 = true;
                }
                else if (Globals.cheat3 == false)
                {
                    Globals.cheat03 = false;
                }
            }
            e.Handled = true;
        }
        public static void Hotkey4(object sender, KeyEventArgs e)
        {
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
                if (Globals.cheat4 == true)
                {
                    Globals.cheat04 = true;
                }
                else if (Globals.cheat4 == false)
                {
                    Globals.cheat04 = false;
                }
            }
            e.Handled = true;
        }
        public static void Hotkey5(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                cheat5_click++;
                if (cheat5_click == 1)
                {
                    Globals.cheat5 = true;
                }
                else if (cheat5_click == 2)
                {
                    cheat5_click = 0;
                    Globals.cheat5 = false;
                }
                if (Globals.cheat5 == true)
                {
                    Globals.cheat05 = true;
                }
                else if (Globals.cheat5 == false)
                {
                    Globals.cheat05 = false;
                }
                e.Handled = true;
            }
        }
        public static void Hotkey6(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F6)
            {
                cheat6_click++;
                if (cheat6_click == 1)
                {
                    Globals.cheat6 = true;
                }
                else if (cheat6_click == 2)
                {
                    cheat6_click = 0;
                    Globals.cheat6 = false;
                }
                if (Globals.cheat6 == true)
                {
                    Globals.cheat06 = true;
                }
                else if (Globals.cheat6 == false)
                {
                    Globals.cheat06 = false;
                }
                e.Handled = true;
            }
        }
        public static void Hotkey7(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F7)
            {
                cheat7_click++;
                if (cheat7_click == 1)
                {
                    Globals.cheat7 = true;
                }
                else if (cheat7_click == 2)
                {
                    cheat7_click = 0;
                    Globals.cheat7 = false;
                }
                if (Globals.cheat7 == true)
                {
                    Globals.cheat07 = true;
                }
                else if (Globals.cheat7 == false)
                {
                    Globals.cheat07 = false;
                }
                e.Handled = true;
            }
        }
        public static void Hotkey8(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F8)
            {
                cheat8_click++;
                if (cheat8_click == 1)
                {
                    Globals.cheat8 = true;
                }
                else if (cheat8_click == 2)
                {
                    cheat8_click = 0;
                    Globals.cheat8 = false;
                }
                if (Globals.cheat8 == true)
                {
                    Globals.cheat08 = true;
                }
                else if (Globals.cheat8 == false)
                {
                    Globals.cheat08 = false;
                }
                e.Handled = true;
            }
        }
        public static void Hotkey9(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F9)
            {
                cheat9_click++;
                if (cheat9_click == 1)
                {
                    Globals.cheat9 = true;
                }
                else if (cheat9_click == 2)
                {
                    cheat9_click = 0;
                    Globals.cheat9 = false;
                }
                if (Globals.cheat9 == true)
                {
                    Globals.cheat09 = true;
                }
                else if (Globals.cheat9 == false)
                {
                    Globals.cheat09 = false;
                }
                e.Handled = true;
            }
        }
        #endregion

        #region CheckedChanged
        private void StamBox_CheckedChanged(object sender, EventArgs e)
        {
            if (StamBox.Checked == true)
            {
                fConsole.WriteLine("Stamina Recovery +5000");
                fConsole.WriteLine("---------");
                MemLib.writeMemory(Globals.S7Offset, "bytes", "F3 44 0F 11 A1 A4 17");
            }
            else if (StamBox.Checked == false)
            {
                fConsole.WriteLine("Stamina Recovery Off");
                fConsole.WriteLine("---------");
                MemLib.writeMemory(Globals.S7Offset, "bytes", "F3 44 0F 11 99 A4 17 00 00");
            }
        }

        private void ManaBox_CheckedChanged(object sender, EventArgs e)
        {
                if (ManaBox.Checked == true)
                {

                    MemLib.writeMemory(Globals.S6Offset, "bytes", "F3 44 0F 11 A1 AC 17 00 00");
                    fConsole.WriteLine("Mana Recovery +120k%");
                    fConsole.WriteLine("---------");
                }
                else if (ManaBox.Checked == false)
                {

                    fConsole.WriteLine("Mana Recovery Off");
                    fConsole.WriteLine("---------");
                    MemLib.writeMemory(Globals.S6Offset, "bytes", "F3 44 0F 11 A9 AC 17 00 00");
                }
            }

        private void HealthBox_CheckedChanged(object sender, EventArgs e)
        {
            if (HealthBox.Checked == true)
            {
                fConsole.WriteLine("Health Recovery +5000");
                fConsole.WriteLine("---------");
                MemLib.writeMemory(Globals.S4Offset, "bytes", "F3 44 0F 11 A1 9C 17 00 00");
            }
            else if (HealthBox.Checked == false)
            {
                fConsole.WriteLine("Health Recovery Off");
                fConsole.WriteLine("---------");
                MemLib.writeMemory(Globals.S4Offset, "bytes", "F3 44 0F 11 91 9C 17 00 00");
            }
        }

        private void RageBox_CheckedChanged(object sender, EventArgs e)
        {
            if (RageBox.Checked == true)
            {

                MemLib.writeMemory(Globals.S5Offset, "bytes", "F3 44 0F 5E F8 0F 28 C2");
                fConsole.WriteLine("Rage Generation Set");
                fConsole.WriteLine("---------");
            }
            else if (RageBox.Checked == false)
            {

                fConsole.WriteLine("Rage Generation Off");
                fConsole.WriteLine("---------");

                MemLib.writeMemory(Globals.S5Offset, "bytes", "F3 44 0F 5E F0 0F 28 C2");
            }
        }
        #endregion

        #region DragWindowNoBorder
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private void FormoZ_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

    }

    #endregion

}
