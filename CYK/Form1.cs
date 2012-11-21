using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CYK
{
    public partial class Form1 : Form
    {
        string fileName;
        Grammar mGrammar;

        public Form1()
        {
            InitializeComponent();

            openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            fileName = openFileDialog1.FileName;
            var mInputFile = new FileParser(fileName);
            

            if(mInputFile.Exists())
                mGrammar = mInputFile.ParseFile();

        }
    }
}
