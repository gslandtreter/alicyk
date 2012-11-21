using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace CYK
{
    public partial class Form1 : Form
    {
        string fileName;
        string[,] cykMatrix;
        int wordCount;
        Grammar mGrammar;

        public Form1()
        {
            InitializeComponent();

            openFileDialog1.ShowDialog();
        }

        void DoProcessing()
        {
            string[] words = Regex.Split(textBox1.Text, @"\W+");

            cykMatrix = new string[words.Length, words.Length];
            wordCount = words.Length;

            foreach (string word in words)
            {
                if (!mGrammar.WordBelongsToGrammar(word))
                {
                    label1.Text = "A frase nao pertence a gramatica!";
                    return;
                }
            }

            label1.Text = "Tudo OK!";

            //TODO: Popular matriz! (O algoritmo em si, vai lá gosantos, ritmo de festa!)

        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            fileName = openFileDialog1.FileName;
            var mInputFile = new FileParser(fileName);
            
            if(mInputFile.Exists())
                mGrammar = mInputFile.ParseFile();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DoProcessing();
        }
    }
}
