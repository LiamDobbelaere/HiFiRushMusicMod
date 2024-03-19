using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Globalization;
using System.Runtime.InteropServices;

namespace HiFiRushMusicMod
{
    public partial class Form1 : Form
    {
        // REQUIRED CONSTS

        const int PROCESS_QUERY_INFORMATION = 0x0400;
        const int MEM_COMMIT = 0x00001000;
        const int PAGE_READWRITE = 0x04;
        const int PROCESS_WM_READ = 0x0010;

        // REQUIRED METHODS

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess
             (int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        static extern bool ReadProcessMemory(IntPtr hProcess,
            IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        static extern void GetSystemInfo(out SYSTEM_INFO lpSystemInfo);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int VirtualQueryEx(IntPtr hProcess,
        IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);


        // REQUIRED STRUCTS

        public struct MEMORY_BASIC_INFORMATION
        {
            public int BaseAddress;
            public int AllocationBase;
            public int AllocationProtect;
            public int RegionSize;
            public int State;
            public int Protect;
            public int lType;
        }

        public struct SYSTEM_INFO
        {
            public ushort processorArchitecture;
            ushort reserved;
            public uint pageSize;
            public IntPtr minimumApplicationAddress;
            public IntPtr maximumApplicationAddress;
            public IntPtr activeProcessorMask;
            public uint numberOfProcessors;
            public uint processorType;
            public uint allocationGranularity;
            public ushort processorLevel;
            public ushort processorRevision;
        }


        /*[DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll", SetLastError = true)]
        static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);

        private static Bitmap screenBitmap = new Bitmap(1, 1, PixelFormat.Format32bppArgb);
        private static Graphics g = Graphics.FromImage(screenBitmap);
        private Point catCoordinates = new Point(956, 950); //new Point(956, 825);

        private long lastTime;
        private int beatCount = 0;
        private bool upwardsBeat = true;
        private int tolerance = 16;
        private static IntPtr hdc;*/


        public Form1()
        {
            InitializeComponent();
        }

        private void tmrScreenGrabber_Tick(object sender, EventArgs e)
        {
            //g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            //g.CopyFromScreen(Screen.PrimaryScreen.Bounds.Left + catCoordinates.X, Screen.PrimaryScreen.Bounds.Top + catCoordinates.Y, 0, 0, new Size(1, 1));

            /*Color beatColor = GetPixelColor(catCoordinates.X, catCoordinates.Y); // screenBitmap.GetPixel(0, 0);
            this.BackColor = beatColor;

            if (upwardsBeat && beatColor.R < (255 - tolerance)) return;
            if (!upwardsBeat && beatColor.R > tolerance) return;

            if (upwardsBeat)
            {
                upwardsBeat = false;
                beatCount++;
                return;
            }
            else
            {
                upwardsBeat = true;
            }


            long elapsedMillis = DateTimeOffset.Now.ToUnixTimeMilliseconds() - lastTime;

            if (beatCount <= 0 || elapsedMillis <= 0) return;

            double bpm = (beatCount * 60000.0) / elapsedMillis;

            this.Text = bpm.ToString();*/
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            /*lastTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            hdc = GetDC(IntPtr.Zero); // Get the device context for the entire screen
            */
            SYSTEM_INFO sys_info = new SYSTEM_INFO();
            GetSystemInfo(out sys_info);

            IntPtr proc_min_address = sys_info.minimumApplicationAddress;
            IntPtr proc_max_address = sys_info.maximumApplicationAddress;

            long proc_min_address_l = (long)proc_min_address;
            long proc_max_address_l = (long)proc_max_address;

            Process game = ProcessFromWindowTitle("Hi-Fi RUSH");
            if (game == null || game.MainModule == null)
            {
                MessageBox.Show("Game not found");
                return;
            }

            int[] offsets = new int[] {
                0x8,
                0x1C0,
                0x20,
                0x1C0,
                0x38,
                0x278,
                0x10,
            };

            GMemProcess gProc = new GMemProcess(game, game.MainModule);
            ptrObject obj = gProc.create_ptr_object(0x0728D6C8, offsets);

            float floatValue = gProc.read<float>(obj);

            MessageBox.Show(floatValue.ToString());
        }


        private Process ProcessFromWindowTitle(string windowTitle)
        {
            Process[] processes = Process.GetProcesses();
            foreach (var process in processes)
            {
                if (process.MainWindowTitle.Contains(windowTitle))
                {
                    return process;
                }
            }

            return null;
        }


        private void tmrResetter_Tick(object sender, EventArgs e)
        {
            /*
            beatCount = 0;
            lastTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            upwardsBeat = true;
            */
        }

        public static Color GetPixelColor(int x, int y)
        {
            return Color.Red;
            /*
            uint pixel = GetPixel(hdc, x, y); // Get the pixel color at the specified coordinates

            // Extract the RGB components from the pixel color (which is in the format 0x00BBGGRR)
            int r = (int)(pixel & 0x000000FF);
            int g = (int)((pixel & 0x0000FF00) >> 8);
            int b = (int)((pixel & 0x00FF0000) >> 16);

            return Color.FromArgb(r, g, b);*/
        }

    }
}
