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
        List<string>[,] cykMatrix;
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

            wordCount = words.Length;
            cykMatrix = new List<string>[wordCount, wordCount]; //Inicializar matrix


            for (int i = 0; i < wordCount; i++)
                for (int n = 0; n < wordCount; n++)
                    cykMatrix[i, n] = new List<string>();



            foreach (string word in words)
            {
                if (!mGrammar.WordBelongsToGrammar(word)) //Checar se alguma palavra nao pertence aos terminais
                {
                    label1.Text = "A frase nao pertence a gramatica!";
                    return;
                }
            }


            ///////// Algoritmo copiado do livro

            for (int i = 0; i < wordCount; i++) //Etapa 1
            {
                foreach (Rule rule in mGrammar.GetRules(words[i]))
                    cykMatrix[i, 0].Add(rule.name);
            }

            for (int s = 1; s < wordCount; s++) //Etapa 2
            {
                for (int r = 0; r < wordCount - s + 1; r++)
                {
                    //Clear node
                    for (int k = 0; k < s - 1; k++)
                    {
                        List<string> rulesA = cykMatrix[r, k];
                        List<string> rulesB = cykMatrix[r + k, s - k];

                        /*TODO:
                         * - Fazer a composiçao das regras e checar se elas sao atingiveis por alguma regra da gramatica
                         * - (do tipo S -> AB) para cada regra de A ou B
                         * - Se sim, adicioná-la a lista de cykMatrix[r, s]
                         */
                        
                    }
                }
            }

                label1.Text = "Tudo OK!";
                PrintMatrix();

            //TODO: Popular matriz! (O algoritmo em si, vai lá gosantos, ritmo de festa!)

        }

        void PrintMatrix()
        {
            for (int i = wordCount - 1; i >= 0; i--)
            {
                for (int n = wordCount - 1; n >= 0; n--)
                {
                    if (cykMatrix[n, i].Count == 0)
                        textBox2.Text += " . ";

                    else
                    {
                        foreach (string word in cykMatrix[n, i])
                            textBox2.Text += " " + word;
                    }
                }
                textBox2.Text += "\r\n";
            }
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
