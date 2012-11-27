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
            //TCP ends here
            string[] words = Regex.Split(textBox1.Text, @"\W+");

            wordCount = words.Length;
            cykMatrix = new List<string>[wordCount, wordCount]; //Inicializar matrix


            for (int i = 0; i < wordCount; i++)
                for (int n = 0; n < wordCount; n++)
                    cykMatrix[i, n] = new List<string>();


            ////////// Checando se todas as palavras sao terminais pertencentes a gramatica

            foreach (string word in words)
            {
                if (!mGrammar.WordBelongsToGrammar(word)) //Checar se alguma palavra nao pertence aos terminais
                {
                    label1.Text = "A frase nao pertence a gramatica!";
                    return;
                }
            }


            ///////// Algoritmo copiado do livro abç blauth

            for (int r = 0; r < wordCount; r++) //Etapa 1
            {
                foreach (Rule rule in mGrammar.GetRules(words[r]))
                    cykMatrix[r, 0].Add(rule.name);
            }

            for (int s = 1; s < wordCount + 1; s++) //Etapa 2
            {
                for (int r = 0; r < wordCount - s; r++)
                {
                    for (int k = 0; k <= s - 1; k++)
                    {
                        List<string> rulesA = cykMatrix[r, k];
                        List<string> rulesB = cykMatrix[r + k + 1, s - k - 1];

                        if (rulesA.Count != 0 && rulesB.Count != 0) //Ambas as celulas contem regras
                        {
                            foreach (string ruleA in rulesA)
                            {
                                foreach (string ruleB in rulesB)
                                {
                                    string composedRule = ruleA + "," + ruleB; //Compondo as regras conforme notacao presente no arquivo de entrada

                                    List<Rule> rules = mGrammar.GetRules(composedRule);
                                    foreach (Rule ruleFound in rules)
                                    {
                                        if (!cykMatrix[r, s].Contains(ruleFound.name)) // Celula nao contem regra
                                            cykMatrix[r, s].Add(ruleFound.name);
                                    }
                                }
                            }
                        }
                        else if (rulesA.Count != 0) //Somente a celula A contem regras
                        {
                            foreach (string rule in rulesA)
                            {
                                List<Rule> rules = mGrammar.GetRules(rule);
                                foreach (Rule ruleFound in rules)
                                {
                                    if (!cykMatrix[r, s].Contains(ruleFound.name)) // Celula nao contem regra
                                        cykMatrix[r, s].Add(ruleFound.name);
                                }
                            }
                        }
                        else if (rulesB.Count != 0) //Somente a celula B contem regras
                        {
                            foreach (string rule in rulesB)
                            {
                                List<Rule> rules = mGrammar.GetRules(rule);
                                foreach (Rule ruleFound in rules)
                                {
                                    if (!cykMatrix[r, s].Contains(ruleFound.name)) // Celula nao contem regra
                                        cykMatrix[r, s].Add(ruleFound.name);
                                }
                            }
                        }

                        /*TODO:
                         * - Alterar matriz para conter as probabilidades
                         * - Arvores de Derivacao
                         * - Calculo de Probabilidade
                         * - Bora fazer alguma coisa gosantos
                         * - quero churros
                         */
                    }
                }
            }

            label1.Text = "Palavra nao reconhecida";

            foreach (string startingRule in cykMatrix[0, wordCount - 1]) //Posicao da regra inicial, se existe
            {
                if (mGrammar.IsStartingRule(startingRule))
                {
                    label1.Text = "Palavra reconhecida com sucesso!";
                    break;
                }
            }
            PrintMatrix();

        }

        void PrintMatrix()
        {
            for (int i = wordCount - 1; i >= 0; i--)
            {
                for (int n = 0; n < wordCount; n++)
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
