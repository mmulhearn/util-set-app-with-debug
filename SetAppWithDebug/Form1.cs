using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using SetAppWithDebug.Properties;

namespace SetAppWithDebug
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _selectFolder(txtSharedLibRoot);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _selectFolder(txtNugetRoot);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            _selectFolder(txtSharedCoreRoot);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var sharedRootLib = Settings.Default["SharedStandardLibRoot"]?.ToString();
            var nugetRoot = Settings.Default["NugetRoot"]?.ToString();
            var csProj = Settings.Default["CsProjPath"]?.ToString();
            var sharedCoreLib = Settings.Default["SharedCoreLibRoot"]?.ToString();
            var searchPattern = Settings.Default["SearchPattern"]?.ToString();

            if (!string.IsNullOrWhiteSpace(sharedRootLib))
            {
                txtSharedLibRoot.Text = sharedRootLib;
            }

            if (!string.IsNullOrWhiteSpace(nugetRoot))
            {
                txtNugetRoot.Text = nugetRoot;
            }
            else
            {
                var userRootFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                txtNugetRoot.Text = $@"{userRootFolder}\.nuget";
            }

            if (!string.IsNullOrWhiteSpace(csProj))
            {
                txtTargetProjFile.Text = csProj;
            }

            if (!string.IsNullOrWhiteSpace(sharedCoreLib))
            {
                txtSharedCoreRoot.Text = sharedCoreLib;
            }

            if (!string.IsNullOrWhiteSpace(searchPattern))
                txtSearchPattern.Text = searchPattern;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = txtTargetProjFile.Text;
            var result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                txtTargetProjFile.Text = openFileDialog1.FileName;
            }
        }

        private void _selectFolder(TextBox txt)
        {
            folderBrowserDialog1.SelectedPath = txt.Text;
            var result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                txt.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var context = new Context
            {
                NugetFolderRoot = txtNugetRoot.Text.Trim(),
                ProjectPath = txtTargetProjFile.Text.Trim(),
                SearchRegex = string.IsNullOrWhiteSpace(txtSearchPattern.Text.Trim())
                    ? null
                    : new Regex(txtSearchPattern.Text.Trim()),
                SharedCoreLibraryRoot = txtSharedCoreRoot.Text.Trim(),
                SharedStandardLibraryRoot = txtSharedLibRoot.Text.Trim()
            };

            if (!context.IsValid(out string validationError))
            {
                MessageBox.Show(validationError, "Validation Error", MessageBoxButtons.OK);
                return;
            }


            var engine = new Engine();

            Settings.Default["SharedStandardLibRoot"] = context.SharedStandardLibraryRoot;
            Settings.Default["NugetRoot"] = context.NugetFolderRoot;
            Settings.Default["CsProjPath"] = context.ProjectPath;
            Settings.Default["SharedCoreLibRoot"] = context.SharedCoreLibraryRoot;
            Settings.Default["SearchPattern"] = context.SearchRegex?.ToString();
            Settings.Default.Save();

            engine.SwapDebugDlls(context);

            var results = new Results
            {
                Context = context
            };
            results.ShowDialog(this);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var nugetFolder = txtNugetRoot.Text.Trim();

            if (string.IsNullOrWhiteSpace(nugetFolder))
            {
                MessageBox.Show("Nuget folder is empty.", "Error", MessageBoxButtons.OK);
                return;
            }

            if (!Directory.Exists(nugetFolder))
            {
                MessageBox.Show("Nuget folder does not exist.", "Error", MessageBoxButtons.OK);
                return;
            }

            var packages = $@"{nugetFolder}\packages";
            foreach (var dir in Directory.GetDirectories(packages, "Comcast.Cs.*"))
            {
                foreach (var versionDir in Directory.GetDirectories(dir))
                {
                    var libDir = $@"{versionDir}\lib";

                    foreach (var frameworkDir in Directory.GetDirectories(libDir))
                    {
                        var origFiles = Directory.GetFiles(frameworkDir, "*.dll.orig");
                        foreach (var origFile in origFiles)
                        {
                            var originalName = origFile.Substring(0, origFile.Length - 5);
                            if (File.Exists(originalName))
                                File.Delete(originalName);
                            File.Move(origFile, originalName);
                        }
                    }
                }
            }

            MessageBox.Show("All original files have been reinstated.", "Finished", MessageBoxButtons.OK);
        }
    }
}
