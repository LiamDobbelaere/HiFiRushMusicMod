namespace HiFiRushMusicMod
{
    public enum BeatDirection
    {
        Up,
        Down,
        Unchanged
    }

    public enum Rank
    {
        S,
        A,
        LOWER
    }

    public partial class Form1 : Form
    {
        private long lastFrameCapture;
        private int fpsCount;
        private int totalFps;

        private Bitmap screen;
        private Graphics screenGraphics;
        private Bitmap rank;
        private Graphics rankGraphics;

        private int xOffset = 930;
        private int yOffset = 820;

        private int rankXOffset = 1710;
        private int rankYOffset = 185;

        private List<float> bpmCatValues = new List<float>();
        private bool shouldLog = false;

        // color from hex:
        private Color sRankColor = ColorTranslator.FromHtml("#EF1375");
        private Color aRankColor = ColorTranslator.FromHtml("#E52F0A");

        int lastRedValue = 0;
        private BeatDirection lastBeatDirection = BeatDirection.Up;

        long lastBeatTimestamp = 0;
        long lastPixelColorChangeTimestamp = 0;

        private RollingAverage bpmRollingAverage = new RollingAverage(10);

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.DoubleBuffered = false;
            lastFrameCapture = GetTimestamp();
            lastPixelColorChangeTimestamp = GetTimestamp();

            screen = new Bitmap(64, 64);
            screenGraphics = Graphics.FromImage(screen);

            rank = new Bitmap(1, 1);
            rankGraphics = Graphics.FromImage(rank);

            bwGameCapture.RunWorkerAsync();
        }

        private void bwGameCapture_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            while (true)
            {
                if (GetTimestamp() - lastFrameCapture >= (1 / 60f * 1000f) * 10000)
                {
                    float fps = 1000 / ((GetTimestamp() - lastFrameCapture) / 10000f);

                    lblFps.Invoke(new Action(() =>
                    {
                        lblFps.Text = $"FPS: {fps:0.00}";
                    }));

                    CaptureScreen();

                    // lerp form BackColor towards white
                    this.Invoke(new Action(() =>
                    {
                        this.BackColor = Color.FromArgb(Lerp(this.BackColor.R, 255, 0.5f), Lerp(this.BackColor.G, 255, 0.5f), Lerp(this.BackColor.B, 255, 0.5f));
                    }));

                    lastFrameCapture = GetTimestamp();
                }
            }
        }

        private long GetTimestamp()
        {
            return System.Diagnostics.Stopwatch.GetTimestamp();
        }

        private Rank GetRankFromColor(Color color)
        {
            // use euclidean color distance to determine rank
            double sRankDistance = Math.Sqrt(Math.Pow(sRankColor.R - color.R, 2) + Math.Pow(sRankColor.G - color.G, 2) + Math.Pow(sRankColor.B - color.B, 2));
            double aRankDistance = Math.Sqrt(Math.Pow(aRankColor.R - color.R, 2) + Math.Pow(aRankColor.G - color.G, 2) + Math.Pow(aRankColor.B - color.B, 2));
            double tolerance = 10f;

            if (sRankDistance < tolerance)
            {
                return Rank.S;
            }
            else if (aRankDistance < tolerance)
            {
                return Rank.A;
            }

            return Rank.LOWER;
        }

        private void CaptureScreen()
        {
            // capture the center of the screen (64x64)
            screenGraphics.CopyFromScreen(xOffset, yOffset, 0, 0, screen.Size);
            rankGraphics.CopyFromScreen(rankXOffset, rankYOffset, 0, 0, rank.Size);

            // get the color of the center pixel and push it to the list
            Color pixelColor = screen.GetPixel(32, 8);
            Color rankColor = rank.GetPixel(0, 0);

            Rank currentRank = GetRankFromColor(rankColor);

            this.lblRank.Invoke(new Action(() =>
            {
                this.lblRank.Text = currentRank.ToString() + " rank";
                this.lblRank.ForeColor = rankColor;
            }));

            long lastChangeDiff = GetTimestamp() - lastPixelColorChangeTimestamp;
            bool isProbablyNoise = lastChangeDiff < 1000000;

            if (!isProbablyNoise && pixelColor.R != lastRedValue)
            {
                lastPixelColorChangeTimestamp = GetTimestamp();
                BeatDirection dir = BeatDirection.Unchanged;

                if (pixelColor.R > lastRedValue)
                {
                    dir = BeatDirection.Up;
                }

                if (pixelColor.R < lastRedValue)
                {
                    dir = BeatDirection.Down;
                }

                if (dir != BeatDirection.Unchanged && dir != lastBeatDirection)
                {
                    lastBeatDirection = dir;
                    if (dir == BeatDirection.Up)
                    {
                        this.Invoke(new Action(() =>
                        {
                            this.BackColor = Color.Red;
                        }));

                        long timestampDiff = (GetTimestamp() - lastBeatTimestamp); // in ticks
                        int bpm = (int)(600000000 / timestampDiff);

                        bpmRollingAverage.Add(bpm);

                        lblBpm.Invoke(new Action(() =>
                        {
                            lblBpm.Text = $"{bpmRollingAverage.GetAverage()}";
                        }));

                        lastBeatTimestamp = GetTimestamp();
                    }
                }
            }

            lastRedValue = pixelColor.R;

            if (shouldLog)
            {
                rtbLog.Invoke(new Action(() =>
                {
                    int brightness = (int)(pixelColor.R);
                    rtbLog.AppendText("Brightness: " + brightness + Environment.NewLine);
                    rtbLog.ScrollToCaret();
                }));
            }

            this.BackgroundImage = screen;
            this.Invoke(new Action(() => this.Refresh()));
        }

        private void btnStopLogging_Click(object sender, EventArgs e)
        {
            shouldLog = false;
        }

        private int Lerp(int a, int b, float t)
        {
            return (int)(a + (b - a) * t);
        }
    }

    public class RollingAverage
    {
        private int size;
        private Queue<float> values = new Queue<float>();

        public RollingAverage(int size)
        {
            this.size = size;
        }
        public void Add(float value)
        {
            values.Enqueue(value);
            if (values.Count > size)
            {
                values.Dequeue();
            }
        }
        public float GetAverage()
        {
            // loop though all values and calculate the average
            float sum = 0;
            foreach (float value in values)
            {
                sum += value;
            }
            return sum / values.Count;
        }
    }
}
