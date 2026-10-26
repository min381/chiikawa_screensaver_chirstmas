\
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ChiikawaScreensaver;

internal static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        ApplicationConfiguration.Initialize();
        // Windows screensaver convention: /s = full screen; /c = configuration placeholder; /p = preview.
        string arg = args.FirstOrDefault() ?? "/s";
        if (arg.Equals("/c", StringComparison.OrdinalIgnoreCase) || arg.Equals("-c", StringComparison.OrdinalIgnoreCase))
        {
            MessageBox.Show("Chiikawa Screensaver\\nRandom characters appear every 10–30 seconds.", "Settings",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        Application.Run(new ScreensaverForm());
    }
}

internal sealed class ScreensaverForm : Form
{
    private readonly Timer frameTimer = new() { Interval = 16 };
    private readonly Timer spawnTimer = new();
    private readonly Random rng = new();
    private readonly List<Walker> walkers = new();
    private readonly List<Image> sprites = new();
    private Point mouseStart;
    private bool mouseMoved;
    private readonly DateTime started = DateTime.Now;

    public ScreensaverForm()
    {
        FormBorderStyle = FormBorderStyle.None;
        WindowState = FormWindowState.Maximized;
        TopMost = true;
        ShowInTaskbar = false;
        BackColor = Color.FromArgb(24, 38, 31);
        DoubleBuffered = true;
        Cursor.Hide();
        KeyPreview = true;
        mouseStart = Cursor.Position;

        string assetDir = Path.Combine(AppContext.BaseDirectory, "Assets");
        foreach (string file in Directory.Exists(assetDir)
            ? Directory.GetFiles(assetDir, "*.png").OrderBy(f => f).ToArray()
            : Array.Empty<string>())
        {
            try { sprites.Add(Image.FromFile(file)); } catch { }
        }

        KeyDown += (_, _) => Close();
        MouseDown += (_, _) => Close();
        MouseMove += (_, _) =>
        {
            if (Math.Abs(Cursor.Position.X - mouseStart.X) + Math.Abs(Cursor.Position.Y - mouseStart.Y) > 10)
                mouseMoved = true;
            if (mouseMoved) Close();
        };
        Deactivate += (_, _) => Close();

        // Start with one character, then add more at random 10–30 second intervals.
        SpawnWalker();
        ResetSpawnTimer();
        spawnTimer.Tick += (_, _) => { SpawnWalker(); ResetSpawnTimer(); };
        frameTimer.Tick += (_, _) => Invalidate();
        spawnTimer.Start();
        frameTimer.Start();
    }

    private void ResetSpawnTimer() => spawnTimer.Interval = rng.Next(10_000, 30_001);

    private void SpawnWalker()
    {
        if (sprites.Count == 0) return;
        // Keep the scene lively but bounded.
        if (walkers.Count >= 8) walkers.RemoveAt(0);
        var image = sprites[rng.Next(sprites.Count)];
        float scale = (float)(0.20 + rng.NextDouble() * 0.13);
        float height = Math.Max(70, ClientSize.Height * scale);
        float width = height * image.Width / Math.Max(1, image.Height);
        float x = rng.NextDouble() < 0.5 ? -width : ClientSize.Width + width;
        float y = Math.Max(0, ClientSize.Height - height - rng.Next(15, 100));
        float speed = (float)(1.0 + rng.NextDouble() * 2.2) * (x < 0 ? 1 : -1);
        walkers.Add(new Walker(image, x, y, width, height, speed, rng.NextDouble() * Math.PI * 2));
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
        e.Graphics.Clear(BackColor);
        // Subtle star specks for a calm Christmas-night backdrop.
        using var starBrush = new SolidBrush(Color.FromArgb(70, 255, 255, 255));
        for (int i = 0; i < 55; i++)
        {
            float sx = (i * 173) % Math.Max(1, ClientSize.Width);
            float sy = (i * 97) % Math.Max(1, ClientSize.Height);
            e.Graphics.FillEllipse(starBrush, sx, sy, 2, 2);
        }

        double t = (DateTime.Now - started).TotalSeconds;
        foreach (var w in walkers)
        {
            w.X += w.Speed;
            w.Phase += 0.16;
            // The source poster is a single still pose. This creates a readable bobbing gait
            // with alternating body lean; it does not invent separate limb artwork.
            if (w.Speed > 0 && w.X > ClientSize.Width + w.Width) w.X = -w.Width;
            if (w.Speed < 0 && w.X < -w.Width) w.X = ClientSize.Width + w.Width;
            float bob = (float)Math.Abs(Math.Sin(w.Phase)) * 7f;
            float lean = (float)Math.Sin(w.Phase) * 3.2f;
            var state = e.Graphics.Save();
            float cx = w.X + w.Width / 2;
            float cy = w.Y + w.Height / 2 - bob;
            e.Graphics.TranslateTransform(cx, cy);
            e.Graphics.RotateTransform(lean);
            e.Graphics.DrawImage(w.Image, -w.Width/2, -w.Height/2, w.Width, w.Height);
            e.Graphics.Restore(state);
        }
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        frameTimer.Stop();
        spawnTimer.Stop();
        foreach (var img in sprites) img.Dispose();
        Cursor.Show();
        base.OnFormClosed(e);
    }

    private sealed class Walker
    {
        public Image Image { get; }
        public float X, Y, Width, Height, Speed;
        public double Phase;
        public Walker(Image image, float x, float y, float width, float height, float speed, double phase)
        { Image=image; X=x; Y=y; Width=width; Height=height; Speed=speed; Phase=phase; }
    }
}
