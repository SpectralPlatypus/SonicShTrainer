using System;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using ProcessMemory;

namespace SonicShTrainer
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);

        int? sonicProcId = null;
        const int WM_HOTKEY = 0x0312;
        #region OFFSETS
        readonly string targetProc = "sonic";
        const uint P1_STRUCT = 0x2eb20;
        const uint VELOCITY = P1_STRUCT +0xb4;
        const uint GOODIES = P1_STRUCT + 0x18;
        const uint MAP_NAME = 0x65430;
        const uint POST_LOAD_MAP = 0x61740;
        const uint RAND_VAL = 0x33390;
        const uint DEMO_TIME = 0x2F32C;
        #endregion

        Process GetSonicProcess()
        {
            if (!sonicProcId.HasValue) return null;
            try
            {
                return Process.GetProcessById(sonicProcId.Value);
            }
            catch
            {
                return null;
            }
        }

        IntPtr ToAddr(Process proc, uint offset) => new IntPtr(proc.MainModule.BaseAddress.ToInt32() + offset);

        // Not confusing at all
        bool IsValidProc(Process p) => !((p?.HasExited).GetValueOrDefault(true));

        public Form1()
        {
            InitializeComponent();
            timer1.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var proc = GetSonicProcess();
            if (IsValidProc(proc))
            {
                uint goods = proc.ReadUInt32(ToAddr(proc,GOODIES));
                goods = Goodies.AddSonicPass(goods);
                proc.Write(ToAddr(proc, GOODIES), goods);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!sonicProcId.HasValue)
            {
                var proc = Process.GetProcessesByName(targetProc).FirstOrDefault();
                if (IsValidProc(proc))
                {
                    sonicProcId = proc.Id;
                    processStateLabel.Text = $"Detected process: {proc.ProcessName} [{proc.Id}]";
                }
                else
                    return;
            }

            try
            {
                var proc = Process.GetProcessById(sonicProcId.Value);
                byte[] mapNameArray = proc.ReadBytes(ToAddr(proc, MAP_NAME), 4);
                string mapName = Encoding.ASCII.GetString(mapNameArray);
                mapLabel.Text = mapName;

                if(mapName.Contains("_"))
                {

                }

                uint randVal = proc.ReadUInt32(ToAddr(proc, RAND_VAL));
                randLabel.Text = randVal.ToString("X");

                int vel = proc.ReadInt32(ToAddr(proc, VELOCITY));
                labelVelocity.Text = vel.ToString();

                uint timerDemo = proc.ReadUInt32(ToAddr(proc, DEMO_TIME));
                labelDemo.Text = timerDemo.ToString();
            }
            catch
            {
                sonicProcId = null;
                processStateLabel.Text = $"Searching for {targetProc}.exe...";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var proc = GetSonicProcess();
            if (IsValidProc(proc))
            {
                uint goods = proc.ReadUInt32(ToAddr(proc, GOODIES));
                goods = Goodies.AddBusPass(goods);
                proc.Write(ToAddr(proc, GOODIES), goods);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var proc = GetSonicProcess();
            if (IsValidProc(proc))
            {
                uint goods = proc.ReadUInt32(ToAddr(proc, GOODIES));
                goods = Goodies.AddKey(goods);
                proc.Write(ToAddr(proc, GOODIES), goods);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RegisterHotKey(this.Handle, 1, 0, (int)Keys.F1);
            RegisterHotKey(this.Handle, 2, 0, (int)Keys.F2);
            RegisterHotKey(this.Handle, 3, 0, (int)Keys.F3);
            RegisterHotKey(this.Handle, 4, 0, (int)Keys.F4);

            processStateLabel.Text = $"Searching for {targetProc}.exe...";
        }

        protected override void WndProc(ref Message m) //hotbuttons
        {
            if (m.Msg == WM_HOTKEY)
            {
                switch(m.WParam.ToInt32())
                {
                    case 1:
                        button1.PerformClick();
                        break;
                    case 2:
                        button2.PerformClick();
                        break;
                    case 3:
                        button3.PerformClick();
                        break;
                    case 4:
                        button4.PerformClick();
                        break;
                }
            }
            base.WndProc(ref m);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            SoundPlayer snd = new SoundPlayer(Properties.Resources.yahoo);
            snd.Play();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var proc = GetSonicProcess();
            if (IsValidProc(proc))
            {
                uint demoTimer = proc.ReadUInt32(ToAddr(proc, DEMO_TIME));
                demoTimer = (demoTimer == 60000u)? 5000u : 60000u;
                proc.Write(ToAddr(proc, DEMO_TIME), demoTimer);
            }
        }
    }
}
