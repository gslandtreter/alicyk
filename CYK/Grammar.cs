﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

struct Rule
{
    public string name;
    public string rule;
    public Double probability; //TODO: find out wtf that thing means
}

namespace CYK
{
    class Grammar
    {
        public List<string> terminals;
        public List<string> variables;
        public string startingRule;
        public List<Rule> rules;

        public Grammar()
        {
            terminals = new List<string>();
            variables = new List<string>();
            rules = new List<Rule>();
        }

        public void PopulateTerminals(string inputString)
        {
            string[] tokens = Regex.Split(inputString, @"\W+");

            foreach (string token in tokens)
            {
                if (token.Length == 0)
                    continue;

                terminals.Add(token);
            }
        }

        public void PopulateVariables(string inputString)
        {
            string[] tokens = Regex.Split(inputString, @"\W+");

            foreach (string token in tokens)
            {
                if (token.Length == 0)
                    continue;

                variables.Add(token);
            }
        }

        public void SetStartingRule(string inputString) //Gambiarra pra programar rapido ftw
        {
            string[] tokens = Regex.Split(inputString, @"\W+");

            foreach (string token in tokens)
            {
                if (token.Length == 0)
                    continue;

                startingRule = token;
                return;
            }
        }

        public void AddRule(string inputString)
        {
            Rule ruleToAdd;

            string rule = inputString.Substring(0, inputString.IndexOf(';'));
            ruleToAdd.probability = Convert.ToDouble(inputString.Substring(inputString.IndexOf(';')).Remove(0,1).Replace('.',',')); //Gambiarra

            rule = rule.Replace("{", "").Replace("}","").Replace(" ", "");

            ruleToAdd.name = rule.Substring(0, rule.IndexOf('>'));
            ruleToAdd.rule = rule.Substring(rule.IndexOf('>')).Remove(0,1);

            rules.Add(ruleToAdd);
        }
    }
}
