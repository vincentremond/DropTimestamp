using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DropTimestamp
{
    public sealed partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            AllowDrop = true;
            DragEnter += Form1_DragEnter;
            DragDrop += Form1_DragDrop;
            listBox1.DoubleClick += ListBox1OnDoubleClick;
        }

        private void ListBox1OnDoubleClick(object? sender, EventArgs e)
        {
            Clipboard.SetText(listBox1.SelectedItem.ToString());
        }

        void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        void Form1_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[]) e.Data.GetData(DataFormats.FileDrop);

            var dateTimes = files
                .SelectMany(GetFiles)
                .SelectMany(f => new[] {f.CreationTime, f.LastWriteTime})
                .Select(d => d.Date)
                .Distinct()
                .OrderBy(d => d)
                .Select(d => d.ToString("yyyy.MM.dd-"))
                .ToArray();

            listBox1.Items.Clear();
            listBox1.Items.AddRange(dateTimes);
        }

        private IEnumerable<FileSystemInfo> GetFiles(string s)
        {
            var pathAttributes = File.GetAttributes(s);
            if (pathAttributes.HasFlag(FileAttributes.Directory))
            {
                var directoryInfo = new DirectoryInfo(s);
                yield return directoryInfo;

                var files = directoryInfo.GetFiles("*", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    yield return file;
                }
            }
            else
            {
                yield return new FileInfo(s);
            }
        }
    }
}