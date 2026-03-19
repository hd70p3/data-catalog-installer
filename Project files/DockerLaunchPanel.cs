using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Engrafo_1_Installer
{
    public partial class DockerLaunchPanel : UserControl
    {
        public event EventHandler BackClicked;
        public int Port { get; set; }
        public string AppFolder { get; set; }

        private Label lblTitle;
        private RichTextBox lblMessage;
        private Label lblDownloadHint;
        private Button bntRegularData;
        private Button btnLaunch;
        private Button btnFinish;
        private Button btnBack;
        private FlowLayoutPanel downloadPanel;
        private Button btnGetLicense;

        public DockerLaunchPanel()
        {
            InitializeAppComponent();
        }

        private void InitializeAppComponent()
        {
            this.Dock = DockStyle.Fill;
            const int btnHeight = 30;
            const int btnWidth = 120;
            const int margin = 6;

            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 5,
                Padding = new Padding(margin),
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            lblTitle = new Label
            {
                Text = "All Done! \r\n\r\n Click the Launch button. \r\n !! Come back to the installer app for user login and demo data\r\n" +
                "\r\n!!! On launch - Please allow the browser a short time to connect to the containers in Docker.\r\n" +
                "You can also start the application directly from Docker Desktop.",
                Font = new Font(this.Font, FontStyle.Bold),
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Margin = new Padding(0, 0, 0, margin / 2)
            };
            mainLayout.Controls.Add(lblTitle, 0, 0);

            lblMessage = new RichTextBox
            {
                Text = "",
                ReadOnly = true,
                BorderStyle = BorderStyle.None,
                ScrollBars = RichTextBoxScrollBars.None,
                DetectUrls = true,
                TabStop = false,
                BackColor = this.BackColor,
                ForeColor = this.ForeColor,
                Font = this.Font,
                AutoSize = false,
                MinimumSize = new Size(600, 0),
                Width = 600,
                Height = 10,
                Anchor = AnchorStyles.Left,
                Margin = new Padding(0, 0, 0, margin / 2)
            };

            lblMessage.ContentsResized += (s, e) =>
            {
                lblMessage.Height = e.NewRectangle.Height + 6;
            };

            lblMessage.LinkClicked += (s, e) =>
            {
                Process.Start(new ProcessStartInfo(e.LinkText) { UseShellExecute = true });
            };

            mainLayout.Controls.Add(lblMessage, 0, 1);

            lblDownloadHint = new Label
            {
                Text = "After launch of Engrafo you can download sample data to test Engrafo's API og view the result in Engrafo.",
                AutoSize = true,
                MaximumSize = new Size(600, 0),
                Anchor = AnchorStyles.Left,
                Margin = new Padding(0, 0, 0, margin / 2),
                Visible = false
            };

            //mainLayout.Controls.Add(lblDownloadHint, 0, 2);

            downloadPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Margin = new Padding(0, 0, 0, margin / 2),
                Visible = false
            };

            const string licenseUrl = "https://www.engrafo.eu/engrafo-license-models/";

            btnGetLicense = new Button
            {
                Text = "Get a license",
                AutoSize = true,
                Enabled = false,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };

            btnGetLicense.Click += (s, e) =>
            {
                Process.Start(new ProcessStartInfo(licenseUrl) { UseShellExecute = true });
            };

            bntRegularData = new Button
            {
                Text = "Upload Sample CSV Metadata",
                AutoSize = true,
                Enabled = false,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };

            string regText = "By clicking the button, metadata will be placed in the upload folder and read by Engrafo's metadata API \r\n"+
                "If you just want to download the metadata to view the structure, use tis link:";

            Func<Button, string, string, Panel> makeCard = (chk, text, url) =>
            {
                var card = new Panel
                {
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    BorderStyle = BorderStyle.None,
                    Margin = new Padding(0, 0, 0, margin / 2)
                };

                var layout = new TableLayoutPanel
                {
                    AutoSize = true,
                    ColumnCount = 1,
                    RowCount = 3,
                    Margin = new Padding(8)
                };
                layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

                layout.Controls.Add(chk, 0, 0);

                var lbl = new Label
                {
                    Text = text,
                    AutoSize = true,
                    MaximumSize = new Size(600, 0),
                    Margin = new Padding(0, 4, 0, 0)
                };
                layout.Controls.Add(lbl, 0, 1);

                var link = new LinkLabel
                {
                    Text = url,
                    AutoSize = true,
                    MinimumSize = new Size(600, 0),
                    Margin = new Padding(0, 4, 0, 0)
                };
                link.Links.Add(0, url.Length, url);
                link.LinkClicked += (s, e) =>
                {
                    Process.Start(new ProcessStartInfo(e.Link.LinkData.ToString()) { UseShellExecute = true });
                };
                layout.Controls.Add(link, 0, 2);

                card.Controls.Add(layout);
                return card;
            };

            // License: BUTTON ONLY (no link shown)
            var licenseCard = new Panel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BorderStyle = BorderStyle.None,
                Margin = new Padding(0, 0, 0, margin / 2)
            };

            var licenseLayout = new TableLayoutPanel
            {
                AutoSize = true,
                ColumnCount = 1,
                RowCount = 2,
                Margin = new Padding(0, 0, 0, margin / 2)
            };
            licenseLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            licenseLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            licenseLayout.Controls.Add(btnGetLicense, 0, 0);
            licenseLayout.Controls.Add(new Label
            {
                Text = "Click the button to open the license page in your browser.",
                AutoSize = true,
                MaximumSize = new Size(600, 0),
                Margin = new Padding(0, 4, 0, 0)
            }, 0, 1);

            licenseCard.Controls.Add(licenseLayout);
            downloadPanel.Controls.Add(licenseCard);

            // Regular data: keep card with link
            downloadPanel.Controls.Add(makeCard(bntRegularData, regText, "https://www.engrafo.eu/EngrafoVersions/WWISamples.zip"));
            mainLayout.Controls.Add(downloadPanel, 0, 3);

            var scrollPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true
            };
            mainLayout.Dock = DockStyle.Top;
            scrollPanel.Controls.Add(mainLayout);
            Controls.Add(scrollPanel);

            btnLaunch = new Button { Text = "Launch Engrafo", AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink };
            btnBack = new Button { Text = "← Back", AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink };
            btnFinish = new Button { Text = "Finish", AutoSize = true, Visible = false, AutoSizeMode = AutoSizeMode.GrowAndShrink };

            btnBack.Click += (s, e) => BackClicked?.Invoke(this, e);
            btnLaunch.Click += OnLaunchClick;
            btnFinish.Click += (s, e) => this.FindForm()?.Close();

            Controls.AddRange(new Control[] { btnLaunch, btnFinish, btnBack });

            btnBack.BringToFront();
            btnLaunch.BringToFront();
            btnFinish.BringToFront();
            this.Resize += (s, e) => LayoutHelper.PlaceFooterButtons(this, btnFinish, btnBack, btnLaunch, margin, 10);
            this.VisibleChanged += (s, e) =>
            {
                if (Visible) LayoutHelper.PlaceFooterButtons(this, btnFinish, btnBack, btnLaunch, margin, 10);
            };
            LayoutHelper.PlaceFooterButtons(this, btnFinish, btnBack, btnLaunch, margin, 10);
        }

        private async void OnLaunchClick(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo($"http://localhost:{Port}") { UseShellExecute = true });

            lblMessage.Clear();

            lblMessage.SelectionFont = new Font(lblMessage.Font, FontStyle.Bold);
            lblMessage.AppendText($"✔ Engrafo launched at http://localhost:{Port}\r\n\r\n");

            lblMessage.SelectionFont = new Font(lblMessage.Font, FontStyle.Bold);
            lblMessage.AppendText("IMPORTANT NOTE\r\n");

            lblMessage.SelectionFont = new Font(lblMessage.Font, FontStyle.Regular);
            lblMessage.AppendText(
                "Follow these steps to get started with Engrafo:\r\n" +
                "1) Login to Engrafo with (user,password): admin@admin.com, EngrafoDemopw.1\r\n" +
                "2) Get a license for Engrafo (you'll recieve a mail with license-file. Trial is availeble)\r\n" +
                "3) Upload the license in the license section of Engrafo\r\n" +
                "4) Optional: Download sample data to test Engrafo's features. You can use the sample data for testing or upload your own metadata\r\n"+
                "When using Docker SQL database, imported/created data will remain in a volume until deleted");

            btnLaunch.Visible = false;
            btnFinish.Visible = true;

            lblDownloadHint.Visible = false;
            downloadPanel.Visible = true;
            lblTitle.Visible = false;

            btnGetLicense.Enabled = true;

            await Task.Delay(TimeSpan.FromSeconds(5));
            if (!btnLaunch.Visible)
            {
                bntRegularData.Enabled = true;
            }

            // IMPORTANT: don't add Click handlers repeatedly each time Launch is clicked.
            // (This handler should already be wired in InitializeAppComponent.)
            bntRegularData.Click += async (s, e2) =>
            {
                await DownloadAndExtract("https://www.engrafo.eu/EngrafoVersions/WWISamples.zip");
            };
        }

        private async Task DownloadAndExtract(string url)
        {
            if (string.IsNullOrWhiteSpace(AppFolder))
            {
                MessageBox.Show("App folder not set – cannot download.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string targetDir = AppFolder;

            string tempZip = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".zip");
            try
            {
                using var client = new HttpClient();
                using var resp = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                resp.EnsureSuccessStatusCode();

                using (var fs = new FileStream(tempZip, FileMode.Create, FileAccess.Write, FileShare.None, 8192, useAsync: true))
                {
                    await resp.Content.CopyToAsync(fs);
                }

                ZipFile.ExtractToDirectory(tempZip, targetDir, overwriteFiles: true);

                MessageBox.Show($"Data downloaded & extracted to:\n{targetDir} \r\n You can use this folder for your own metadata",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to download/extract:\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (File.Exists(tempZip))
                    File.Delete(tempZip);
            }
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (Visible)
            {
                ResetPanelState();
                LayoutHelper.PlaceFooterButtons(this, btnFinish, btnBack, btnLaunch, 20, 10);
            }
        }

        private void ResetPanelState()
        {
            btnLaunch.Visible = true;
            btnFinish.Visible = false;

            lblTitle.Visible = true;
            lblMessage.Text = "";
            lblMessage.Visible = true;

            lblDownloadHint.Visible = false;
            downloadPanel.Visible = false;

            btnGetLicense.Enabled = false;
            bntRegularData.Enabled = false;
        }
    }

    internal static class RichTextBoxLinkExtensions
    {
        public static void SetSelectionLink(this RichTextBox rtb, bool link)
        {
            const int EM_SETCHARFORMAT = 1092;
            const int SCF_SELECTION = 1;
            const uint CFM_LINK = 0x00000020;
            const uint CFE_LINK = 0x00000020;

            var cf = new CHARFORMAT2_STRUCT
            {
                cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf<CHARFORMAT2_STRUCT>(),
                dwMask = CFM_LINK,
                dwEffects = link ? CFE_LINK : 0u
            };

            IntPtr lParam = IntPtr.Zero;
            try
            {
                lParam = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(System.Runtime.InteropServices.Marshal.SizeOf<CHARFORMAT2_STRUCT>());
                System.Runtime.InteropServices.Marshal.StructureToPtr(cf, lParam, false);
                SendMessage(rtb.Handle, EM_SETCHARFORMAT, new IntPtr(SCF_SELECTION), lParam);
            }
            finally
            {
                if (lParam != IntPtr.Zero)
                    System.Runtime.InteropServices.Marshal.FreeCoTaskMem(lParam);
            }
        }

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private struct CHARFORMAT2_STRUCT
        {
            public uint cbSize;
            public uint dwMask;
            public uint dwEffects;
            public int yHeight;
            public int yOffset;
            public int crTextColor;
            public byte bCharSet;
            public byte bPitchAndFamily;

            [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szFaceName;

            public ushort wWeight;
            public ushort sSpacing;
            public int crBackColor;
            public int lcid;
            public uint dwReserved;
            public short sStyle;
            public short wKerning;
            public byte bUnderlineType;
            public byte bAnimation;
            public byte bRevAuthor;
            public byte bReserved1;
        }
    }
}
