using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RenamerUtility
{
    public static class RenamingProcessor
    {
        public static List<ItemForRenaming> Method1(List<ItemForRenaming> input, bool UseRegularExpressions, string RegexPattern, string ReplaceWhat, string ReplaceWith, MatchEvaluator selectedMatchEvaluatorMethod)
        {
            List<ItemForRenaming> ret = new List<ItemForRenaming>();
            foreach (ItemForRenaming ifr in input)
            {
                if (UseRegularExpressions)
                {
                    ifr.NewName = Regex.Replace(ifr.OldName, RegexPattern, selectedMatchEvaluatorMethod);
                }
                else
                    ifr.NewName = ifr.OldName.Replace(ReplaceWhat, ReplaceWith);

                ret.Add(ifr);
            }
            return ret;
        }
    }
}
