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
        Table[,] cykTable;
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

            cykTable = new Table[wordCount, wordCount]; //Inicializar matrix


            for (int i = 0; i < wordCount; i++)
            {
                for (int n = 0; n < wordCount; n++)
                {
                    cykTable[i, n] = new Table();
                    cykTable[i, n].index = i;
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
                {
                    TableItem newItem = new TableItem();
                    newItem.ruleName = rule.name;
                    newItem.probability = rule.probability;
                    newItem.index = r;
                    cykTable[r, 0].items.Add(newItem);
                }
            }

            for (int s = 1; s < wordCount + 1; s++) //Etapa 2
            {
                for (int r = 0; r < wordCount - s; r++)
                {
                    for (int k = 0; k <= s - 1; k++)
                    {
                        Table tRulesA = cykTable[r, k];
                        Table tRulesB = cykTable[r + k + 1, s - k - 1];

                        if (tRulesA.items.Count > 0 && tRulesB.items.Count > 0) //Ambas as celulas contem regras
                        {
                            foreach (TableItem ruleA in tRulesA.items)
                            {
                                foreach (TableItem ruleB in tRulesB.items)
                                {
                                    string composedRule = ruleA.ruleName + "," + ruleB.ruleName; //Compondo as regras conforme notacao presente no arquivo de entrada

                                    List<Rule> rules = mGrammar.GetRules(composedRule);
                                    foreach (Rule ruleFound in rules)
                                    {
                                        TableItem newItem = new TableItem();
                                        newItem.ruleName = ruleFound.name;
                                        newItem.probability = ruleFound.probability;
                                        newItem.leftChild = ruleA;
                                        newItem.rightChild = ruleB;
                                        cykTable[r, s].items.Add(newItem);  
                                    }
                                }
                            }
                        }
                        else if (tRulesA.items.Count > 0) //Somente a celula A contem regras
                        {
                            foreach (TableItem rule in tRulesA.items)
                            {
                                List<Rule> rules = mGrammar.GetRules(rule.ruleName);
                                foreach (Rule ruleFound in rules)
                                {
                                    TableItem newItem = new TableItem();
                                    newItem.ruleName = ruleFound.name;
                                    newItem.probability = ruleFound.probability;
                                    newItem.leftChild = rule;
                                    newItem.rightChild = null;
                                    cykTable[r, s].items.Add(newItem);  
                                }
                            }
                        }
                        else //Somente a celula B contem regras
                        {
                            foreach (TableItem rule in tRulesB.items)
                            {
                                List<Rule> rules = mGrammar.GetRules(rule.ruleName);
                                foreach (Rule ruleFound in rules)
                                {
                                    TableItem newItem = new TableItem();
                                    newItem.ruleName = ruleFound.name;
                                    newItem.probability = ruleFound.probability;
                                    newItem.leftChild = null;
                                    newItem.rightChild = rule;
                                    cykTable[r, s].items.Add(newItem);
                                }
                            }
                        }
                    }
                }
            }

            PrintMatrix();

            label1.Text = "Palavra nao reconhecida";

            foreach (TableItem item in cykTable[0,wordCount-1].items.FindAll(x=>x.ruleName.Equals(mGrammar.startingRule)))
            {
                label1.Text = "Palavra reconhecida com sucesso!";

                textBox2.Text += "\r\n" + item.GetDerivationTree(words);
                textBox2.Text += " - Probabilidade: " + item.GetTreeProbability() + "\r\n\r\n";
            }
        }

        void PrintMatrix()
        {
            for (int i = wordCount - 1; i >= 0; i--)
            {
                for (int n = 0; n < wordCount; n++)
                {
                    if (cykTable[n, i].items.Count == 0)
                        textBox2.Text += " . ";

                    else
                    {
                        for (int j = 0; j < cykTable[n, i].items.Count; j++)
                        {
                            if(j == 0)
                                textBox2.Text += cykTable[n, i].items[j].ruleName;
                            else
                                textBox2.Text += "," + cykTable[n, i].items[j].ruleName;
                        }
                        textBox2.Text += "  ";
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
            textBox2.Clear();

            if (mGrammar != null)
                DoProcessing();
            else
                openFileDialog1.ShowDialog();
        }
    }
}
