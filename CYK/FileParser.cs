using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace CYK
{
    
    class FileParser
    {
        string fileName;
        public FileParser(string name)
        {
            this.fileName = name;
        }

        public bool Exists()
        {
            if(File.Exists(fileName))
                return true;

            return false;
        }

        private string RemoveComments(string input)
        {
            if (input.Contains('#'))
                return input.Substring(0, input.IndexOf('#'));

            else return input;
        }

        public Grammar ParseFile()
        {
            var resultGrammar = new Grammar();

            StreamReader fileStream = new StreamReader(fileName);

            string line;
            while ((line = fileStream.ReadLine()) != null) //Read file line by line
            {
                line = RemoveComments(line); //Remove comentarios da linha
                
                if (line.StartsWith("Terminais", true, null)) //Ler terminais
                {
                    line = fileStream.ReadLine();
                    resultGrammar.PopulateTerminals(RemoveComments(line));
                }
                else if (line.StartsWith("Variaveis", true, null)) //Ler variaveis
                {
                    line = fileStream.ReadLine();
                    resultGrammar.PopulateVariables(RemoveComments(line));
                }
                else if (line.StartsWith("Inicial", true, null)) //Ler inicial
                {
                    line = fileStream.ReadLine();
                    resultGrammar.SetStartingRule(RemoveComments(line));
                }

                else if (line.StartsWith("Regras", true, null)) 
                    continue;

                else //Ler regras
                {
                    resultGrammar.AddRule(line);
                }

            }

            fileStream.Close();
            return resultGrammar;
        }
    }
}
