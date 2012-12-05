using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CYK
{
    public class TableItem
    {
        public int index;

        public List<string> ruleNames;
        public double probability;
        public bool isTerminal;
        public string ruleName;

        public TableItem leftChild;
        public TableItem rightChild;

        public TableItem()
        {
            this.ruleNames = new List<string>();
            this.isTerminal = false;
        }


        public string GetDerivationTree(string[] terminals)
        {
            if (this.leftChild == null && this.rightChild == null)
            {
                if (this.isTerminal)
                {
                    foreach (string rule in this.ruleNames)
                        return rule + "[" + terminals[this.index] + "]";
                }

                else
                    return string.Empty;
            }

            else
                foreach (string rule in this.ruleNames)
                    return rule + "[" + this.leftChild.GetDerivationTree(terminals) + " " + this.rightChild.GetDerivationTree(terminals) + "]";

            return String.Empty;
        }
    }



}
