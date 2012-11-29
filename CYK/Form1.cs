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
        TableItem[,] cykMatrix;
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
            cykMatrix = new TableItem[wordCount, wordCount]; //Inicializar matrix


            for (int i = 0; i < wordCount; i++)
            {
                for (int n = 0; n < wordCount; n++)
                {
                    cykMatrix[i, n] = new TableItem();
                    cykMatrix[i, n].index = i;
                }
            }
                    


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
                    cykMatrix[r, 0].ruleNames.Add(rule.name);

                cykMatrix[r, 0].isTerminal = true;
            }

            for (int s = 1; s < wordCount + 1; s++) //Etapa 2
            {
                for (int r = 0; r < wordCount - s; r++)
                {
                    for (int k = 0; k <= s - 1; k++)
                    {
                        TableItem rulesA = cykMatrix[r, k];
                        TableItem rulesB = cykMatrix[r + k + 1, s - k - 1];

                        if (rulesA.ruleNames.Count != 0 && rulesB.ruleNames.Count != 0) //Ambas as celulas contem regras
                        {
                            foreach (string ruleA in rulesA.ruleNames)
                            {
                                foreach (string ruleB in rulesB.ruleNames)
                                {
                                    string composedRule = ruleA + "," + ruleB; //Compondo as regras conforme notacao presente no arquivo de entrada

                                    List<Rule> rules = mGrammar.GetRules(composedRule);
                                    foreach (Rule ruleFound in rules)
                                    {
                                        if (!cykMatrix[r, s].ruleNames.Contains(ruleFound.name)) // Celula nao contem regra
                                        {
                                            cykMatrix[r, s].ruleNames.Add(ruleFound.name);
                                            cykMatrix[r, s].leftChild = rulesA;
                                            cykMatrix[r, s].rightChild = rulesB;
                                        }
                                            
                                    }
                                }
                            }
                        }
                        else if (rulesA.ruleNames.Count != 0) //Somente a celula A contem regras
                        {
                            foreach (string rule in rulesA.ruleNames)
                            {
                                List<Rule> rules = mGrammar.GetRules(rule);
                                foreach (Rule ruleFound in rules)
                                {
                                    if (!cykMatrix[r, s].ruleNames.Contains(ruleFound.name)) // Celula nao contem regra
                                    {
                                        cykMatrix[r, s].leftChild = rulesA;
                                        cykMatrix[r, s].ruleNames.Add(ruleFound.name);
                                    }
                                }
                            }
                        }
                        else if (rulesB.ruleNames.Count != 0) //Somente a celula B contem regras
                        {
                            foreach (string rule in rulesB.ruleNames)
                            {
                                List<Rule> rules = mGrammar.GetRules(rule);
                                foreach (Rule ruleFound in rules)
                                {
                                    if (!cykMatrix[r, s].ruleNames.Contains(ruleFound.name)) // Celula nao contem regra
                                    {
                                        cykMatrix[r, s].rightChild = rulesB;
                                        cykMatrix[r, s].ruleNames.Add(ruleFound.name);
                                    }
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

            PrintMatrix();

            label1.Text = "Palavra nao reconhecida";

            foreach (string startingRule in cykMatrix[0, wordCount - 1].ruleNames) //Posicao da regra inicial, se existe
            {
                if (mGrammar.IsStartingRule(startingRule))
                {
                    label1.Text = "Palavra reconhecida com sucesso!";
                    textBox2.Text += "\r\n\r\n" + cykMatrix[0, wordCount - 1].GetDerivationTree(words);
                    break;
                }
            }
        }

        void PrintMatrix()
        {
            for (int i = wordCount - 1; i >= 0; i--)
            {
                for (int n = 0; n < wordCount; n++)
                {
                    if (cykMatrix[n, i].ruleNames.Count == 0)
                        textBox2.Text += " . ";

                    else
                    {
                        foreach (string word in cykMatrix[n, i].ruleNames)
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
