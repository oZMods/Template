using MemLib;
using Memory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Security.Permissions;
using System.Threading;
using System.Windows.Forms;
using static Memory.Mem;
using Matrix = MemLib.Matrix;

namespace oZTool
{
    public partial class OVL : Form {
        #region Threads
        private Process process = FormoZ.MemLib.theProc;

        private Thread WaitForExit;
        private Thread overlayThread;
        private Thread windowPosThread;

        private volatile bool isRunning = false;

        public static int Vis_click = 0;
        public static bool Vissed;
        #endregion

        #region Reading
        private int gameWidth, gameHeight;
        public static bool RageGen;
        public static bool HPRecovery;
        public static bool MaxStamina;


        #endregion

        #region Drawing
        public static Brush color;
        public static Brush color4;
        public static Brush Red;
        public static Pen PRed;
        public static Brush Greens;
        public static Pen color1;
        public static Color Color;

        private BufferedGraphics bufferedGraphics;
        private Font mfont = new Font(FontFamily.GenericMonospace, 14, FontStyle.Bold);
        private Font font = new Font(FontFamily.GenericMonospace, 12, FontStyle.Regular);
        private Color colorTransparencyKey = Color.Yellow;
        private Brush hatchBrush3 = new HatchBrush(HatchStyle.Percent90, Color.Black);
        private Brush hatchBrush = new HatchBrush(HatchStyle.Percent90, Color);
        #endregion

        public OVL()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false; // <=== this is important for async access to thread

            try
            {
                SecurityPermission sp = new SecurityPermission(SecurityPermissionFlag.UnmanagedCode);
                sp.Demand();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Demand for SecurityPermissionFlag.UnmanagedCode failed: " + ex.Message);
            }
        }
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams CP = base.CreateParams;
                int WS_EX_TRANSPARENT = 0x00000020;
                CP.ExStyle = CP.ExStyle | WS_EX_TRANSPARENT;
                return CP;
            }
        }
        private void oZHax_Load(object sender, EventArgs e)
        {
            Visible = true;

            Route();
        }
        private void Route()
        {
            InitializeOverlayWindowAttributes();

            StartThreads();
        }
        private void InitializeOverlayWindowAttributes()
        {
            Visible = true;
            picBoxOverlay.Visible = true;
            TopMost = true;
            FormBorderStyle = FormBorderStyle.None;
            picBoxOverlay.Dock = DockStyle.Fill;
            picBoxOverlay.BackColor = colorTransparencyKey;
            TransparencyKey = colorTransparencyKey;
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            BufferedGraphicsContext buffContext = new BufferedGraphicsContext();
            bufferedGraphics = buffContext.Allocate(picBoxOverlay.CreateGraphics(), ClientRectangle);
            bufferedGraphics.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            bufferedGraphics.Graphics.CompositingQuality = CompositingQuality.HighQuality;
        }
        private void StartThreads()
        {
            isRunning = true;
            var procID = FormoZ.MemLib.getProcIDFromName(FormoZ.procName);
            var process = Process.GetProcessById(procID);

            var procID2 = FormoZ.MemLib.getProcIDFromName(FormoZ.procName);
            var process2 = Process.GetProcessById(procID);


            WaitForExit = new Thread(process.WaitForExit);
            WaitForExit.IsBackground = false;
            WaitForExit.Start();

            overlayThread = new Thread(UpdateHack);
            overlayThread.IsBackground = false;
            overlayThread.Start();

            windowPosThread = new Thread(UpdateWindow);
            windowPosThread.IsBackground = false;
            windowPosThread.Start(Handle);

            FormoZ.gkh.HookedKeys.Add(Keys.Insert);
            FormoZ.gkh.KeyUp += new KeyEventHandler(Vis);

        }
        private void UpdateWindow(object handle)
        {
            while (isRunning)
            {
                if (!FormoZ.MemLib.OpenProcess(FormoZ.procName))
                {
                    isRunning = false;
                    continue;
                }
                SetOverlayPosition((IntPtr)handle);
                Thread.Sleep(200);
            }
            BeginInvoke(new Action(() => Route()));
        }
        private void SetOverlayPosition(IntPtr overlayHandle)
        {
            IntPtr gameProcessHandle = FormoZ.MemLib.theProc.MainWindowHandle;
            if (gameProcessHandle == IntPtr.Zero)
                return;

            NativeMethods.RECT targetWindowPosition, targetWindowSize;
            if (!NativeMethods.GetWindowRect(gameProcessHandle, out targetWindowPosition))
                return;
            if (!NativeMethods.GetClientRect(gameProcessHandle, out targetWindowSize))
                return;

            int width = targetWindowPosition.Right - targetWindowPosition.Left;
            int height = targetWindowPosition.Bottom - targetWindowPosition.Top;

            int dwStyle = NativeMethods.GetWindowLong(gameProcessHandle, NativeMethods.GWL_STYLE);
            if ((dwStyle & NativeMethods.WS_BORDER) != 0)
            {
                int bWidth = targetWindowPosition.Right - targetWindowPosition.Left;
                int bHeight = targetWindowPosition.Bottom - targetWindowPosition.Top;

                width = targetWindowSize.Right - targetWindowSize.Left;
                height = targetWindowSize.Bottom - targetWindowSize.Top;

                int borderWidth = (bWidth - targetWindowSize.Right) / 2;
                int borderHeight = (bHeight - targetWindowSize.Bottom);
                borderHeight -= borderWidth;

                targetWindowPosition.Left += borderWidth;
                targetWindowPosition.Top += borderHeight;
            }
            NativeMethods.MoveWindow(overlayHandle, targetWindowPosition.Left, targetWindowPosition.Top, width, height, true);
            NativeMethods.SetWindowPos(gameProcessHandle, overlayHandle, 0, 0, 0, 0, NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE);

            gameWidth = width;
            gameHeight = height;
        }
        public void UpdateHack()
        {
            while (isRunning)
            {
                ReadGameMemory();
                Draw(bufferedGraphics.Graphics);

                if (FormoZ.MemLib.theProc.HasExited)
                {
                    Application.Exit();
                    Application.ExitThread();
                }
                Thread.Sleep(1);
            }
            FormoZ.MemLib.closeProcess();
            bufferedGraphics.Dispose();
            bufferedGraphics = null;


        }
        private void ReadGameMemory()
        {
            if (!isRunning) return;


        }
        private void Draw(Graphics g)
        {
            ClearScreen(g);

            bool oZHax = true;
            if (oZHax)
            {
                if (Vissed == true)
                {
                    color = Brushes.White;
                    color1 = Pens.White;
                    Color = Color.DimGray;
                    Red = Brushes.Red;
                    PRed = Pens.Red;
                    Greens = Brushes.Green;
                    hatchBrush3 = Brushes.Black;

                }
                else if (Vissed == false)
                {
                    color = Brushes.Transparent;
                    color1 = Pens.Transparent;
                    Color = Color.Transparent;
                    Red = Brushes.Transparent;
                    PRed = Pens.Transparent;
                    Greens = Brushes.Transparent;
                    hatchBrush3 = Brushes.Transparent;
                }

                //g.FillRectangle(hatchBrush3, 30, 147, 160, 85);
                //g.DrawRectangle(PRed, 30, 147, 160, 85);

                g.FillRectangle(hatchBrush3, 10, 298, 160, 85);
                g.DrawRectangle(PRed, 10, 298, 160, 85);

                if (Globals.cheat01 == true)
                {
                    Globals.Stam = true;

                    g.DrawString("F1   SPR", font, color, 10, 300);
                    g.DrawString(" On", font, Greens, 120, 300);

                }
                else if (Globals.cheat01 == false)
                {

                    Globals.Stam = false;
                    g.DrawString("F1   SPR", font, color, 10, 300);
                    g.DrawString(" Off", font, Red, 120, 300);

                }
                if (Globals.cheat02 == true)
                {

                    Globals.MP = true;
                    g.DrawString("F2   MPR", font, color, 10, 320);
                    g.DrawString(" On", font, Greens, 120, 320);

                }
                else if (Globals.cheat02 == false)
                {
                    Globals.MP = false;
                    g.DrawString("F2   MPR", font, color, 10, 320);
                    g.DrawString(" Off", font, Red, 120, 320);
                }
                if (Globals.cheat03 == true)
                {
                    Globals.RG = true;

                    g.DrawString("F3   RGR", font, color, 10, 340);
                    g.DrawString(" On", font, Greens, 120, 340);
                }
                else if (Globals.cheat03 == false)
                {
                    Globals.RG = false;
                    g.DrawString("F3   RGR", font, color, 10, 340);
                    g.DrawString(" Off", font, Red, 120, 340);

                }
                if (Globals.cheat04 == true)
                {
                    Globals.HP = true;
                    g.DrawString("F4   HPR", font, color, 10, 360);
                    g.DrawString(" On", font, Greens, 120, 360);
                }
                else if (Globals.cheat04 == false)
                {
                    Globals.HP = false;
                    g.DrawString("F4   HPR", font, color, 10, 360);
                    g.DrawString(" Off", font, Red, 120, 360);
                }

                //if (Globals.cheat05 == true)
                //{
                //    g.DrawString("Inf.DR - On", font, color, 30, 210);
                //}
                //else if (Globals.cheat05 == false)
                //{
                //    g.DrawString("Inf.DR - Off", font, color, 30, 210);
                //}

            }
            bufferedGraphics.Render();
        }
        private void ClearScreen(Graphics g)
        {
            g.FillRectangle(new SolidBrush(colorTransparencyKey), ClientRectangle);
        }
        public void oZHax_FormClosing(object sender, FormClosingEventArgs e)
        {
            isRunning = false;
            {
                windowPosThread.Join(2000);
                overlayThread.Join(2000);

                FormoZ.MemLib.closeProcess();
                Environment.Exit(0);
            }
        }

        #region Hide
        private void BackgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            while (FormoZ.MemLib.OpenProcess(FormoZ.procName))
            {

                if (Vissed == true)
                {
                    this.Show();
                }
                else if (Vissed == false)
                {
                    this.Hide();
                }
            }
        }


        public static void Vis(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Insert)
            {
                Vis_click++;
                if (Vis_click == 1)
                {
                    Vissed = false;
                }
                else if (Vis_click == 2)
                {
                    Vis_click = 0;
                    Vissed = true;
                }
            }
            e.Handled = true;
        }
    }   
    #endregion
}




