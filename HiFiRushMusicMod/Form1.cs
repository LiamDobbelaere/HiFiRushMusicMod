using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Globalization;
using System.Runtime.InteropServices;

namespace HiFiRushMusicMod
{
    public partial class Form1 : Form
    {
        GMemProcess gProc;
        ptrObject bpmPtr;

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess
             (int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        static extern bool ReadProcessMemory(IntPtr hProcess,
            IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesRead);


        private int[] recognizedBpm = new int[] {
            135,
            155
        };

        private int lastUsedBPM;
        private MP3Player mp3Player;

        private string scheduledTrackPath = string.Empty;

        private static Bitmap screenBitmap = new Bitmap(1, 1, PixelFormat.Format32bppArgb);
        private static Graphics g = Graphics.FromImage(screenBitmap);
        private Point catCoordinates = new Point(956, 950); //new Point(956, 825);

        private bool upwardsBeat = true;
        private int tolerance = 16;

        public Form1()
        {
            InitializeComponent();
        }

        private void tmrScreenGrabber_Tick(object sender, EventArgs e)
        {
            if (scheduledTrackPath == string.Empty) return;

            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            g.CopyFromScreen(Screen.PrimaryScreen.Bounds.Left + catCoordinates.X, Screen.PrimaryScreen.Bounds.Top + catCoordinates.Y, 0, 0, new Size(1, 1));

            Color beatColor = screenBitmap.GetPixel(0, 0);
            this.btnHit.BackColor = beatColor;

            if (upwardsBeat && beatColor.R < (255 - tolerance)) return;
            if (!upwardsBeat && beatColor.R > tolerance) return;

            if (upwardsBeat)
            {
                if (scheduledTrackPath != string.Empty)
                {
                    if (mp3Player != null)
                    {
                        mp3Player.Stop();
                        mp3Player.Dispose();
                    }
                    mp3Player = new MP3Player(scheduledTrackPath);
                    mp3Player.Play();

                    scheduledTrackPath = string.Empty;
                }
                
                upwardsBeat = false;
                return;
            }
            else
            {
                upwardsBeat = true;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            /*lastTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            hdc = GetDC(IntPtr.Zero); // Get the device context for the entire screen
            */

            Process game = Process.GetProcessById(1608); //game = ProcessFromWindowTitle("Hi-Fi RUSH");
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

            gProc = new GMemProcess(game, game.MainModule);
            bpmPtr = gProc.create_ptr_object(0x0728D6C8, offsets);
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

        private float GetBPMFromMemory()
        {
            return gProc.read<float>(bpmPtr);
        }

        private void tmrMemoryBPM_Tick(object sender, EventArgs e)
        {
            float bpm = GetBPMFromMemory();
            int bpmAsInt = ((int)Math.Floor(bpm));

            int closestBPMValue = 0;
            int lastDiff = int.MaxValue;
            for (int i = 0; i < recognizedBpm.Length; i++)
            {
                int diff = Math.Abs(recognizedBpm[i] - bpmAsInt);
                if (diff < lastDiff)
                {
                    lastDiff = diff;
                    closestBPMValue = recognizedBpm[i];
                }
            }

            if (closestBPMValue != lastUsedBPM)
            {
                scheduledTrackPath = Path.Join(AppDomain.CurrentDomain.BaseDirectory, closestBPMValue.ToString() + ".mp3");

                bpmLabel.Text = "BPM: " + ((int)Math.Floor(bpm)).ToString() + " (" + closestBPMValue.ToString() + ")";

                lastUsedBPM = closestBPMValue;
            }
        }
    }
}
