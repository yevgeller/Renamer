using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RenamerUtility.Test
{
    [TestClass]
    public class RenamingProcessorTests
    {
        string ReplaceWith = string.Empty;
        string capsCheckInput = "this should be all caps";
        string capsCheckExpectedOutput = "This Should Be All Caps";
        string capsCheckExpectedOutput_Fail = "this Should be All caps";
        string capsCheckRegex = @"\w";

        string prependCheckInput = "item 5.txt";
        string prependCheckExpectedOutput = "item 005.txt";
        string prependCheckExpectedOutput_Fail = "item005.txt";
        string prependCheckRegex = @"\d{1}";
        
        string appendCheckInput = "";
        string appendCheckExpectedOutput = "";
        string removeCheckInput = "";
        string removeCheckExpectedOutput = "";

        [TestMethod]
        public void AllCaps_Test_Success()
        {
            List<ItemForRenaming> input = new List<ItemForRenaming>();
            input.Add(new ItemForRenaming { NewName = capsCheckInput, OldName = capsCheckInput, IsFile = true });
            List<ItemForRenaming> ret = RenamingProcessor.Method1(input, true, 
                capsCheckRegex, capsCheckRegex, capsCheckRegex, 
                new MatchEvaluator(this.MatchTransformation_CapText));
            Assert.AreEqual(
                String.Compare(ret[0].NewName, capsCheckExpectedOutput, false), 0,
                String.Format("Actual: {0}, Expected: {1}", ret[0].NewName, capsCheckExpectedOutput));
        }

        [TestMethod]
        public void AllCaps_Test_Failure()
        {
            List<ItemForRenaming> input = new List<ItemForRenaming>();
            input.Add(new ItemForRenaming { NewName = capsCheckInput, OldName = capsCheckInput, IsFile = true });
            List<ItemForRenaming> ret = RenamingProcessor.Method1(input, true, 
                capsCheckRegex, capsCheckRegex, capsCheckRegex,
                new MatchEvaluator(this.MatchTransformation_CapText));
            Assert.AreNotEqual(
                String.Compare(ret[0].NewName, capsCheckExpectedOutput_Fail, false), 0,
                String.Format("Actual: {0}, Expected: {1}", ret[0].NewName, capsCheckExpectedOutput));
        }

        [TestMethod]
        public void Prepend_Test_Success()
        {
            this.ReplaceWith = "00";
            List<ItemForRenaming> input = new List<ItemForRenaming>();
            input.Add(new ItemForRenaming { NewName = prependCheckInput, OldName = prependCheckInput, IsFile = true });
            List<ItemForRenaming> ret = RenamingProcessor.Method1(input, true,
                prependCheckRegex, prependCheckRegex, "00", new MatchEvaluator(this.MatchTransformation_Prepend));
            Assert.AreEqual(
                string.Compare(ret[0].NewName, prependCheckExpectedOutput, false), 0,
                string.Format("Actual: {0}, Expected: {1}", ret[0].NewName, prependCheckExpectedOutput));
        }

        [TestMethod]
        public void Prepend_Test2_Success()
        {
            prependCheckRegex = @"\d{3}";
            prependCheckInput = "item 05.txt";
            prependCheckExpectedOutput = "item 05.txt";

            this.ReplaceWith = "00";
            List<ItemForRenaming> input = new List<ItemForRenaming>();
            input.Add(new ItemForRenaming { NewName = "item 05.txt", OldName = "item 05.txt", IsFile = true });
            List<ItemForRenaming> ret = RenamingProcessor.Method1(input, true,
                prependCheckRegex, prependCheckRegex, "00", new MatchEvaluator(this.MatchTransformation_Prepend));
            Assert.AreEqual(
                string.Compare(ret[0].NewName, prependCheckExpectedOutput, false), 0,
                string.Format("Actual: {0}, Expected: {1}", ret[0].NewName, prependCheckExpectedOutput));
        }
        //I have "item 1.txt"; I need "item 001.txt"
        //so I need to skip items with two digits and then a ".txt"
        //I have "item 01.txt". I need "item 001.txt"
        //I need to skip "item 1.txt", and "item 001.txt"
        //I have "item 29.txt", I need "item 029.txt"

        //
        [TestMethod]
        public void Prepend_Test3_Success()
        {
            prependCheckRegex = @"^\w+\d{2}\.txt";
            prependCheckInput = "item 005.txt";
            prependCheckExpectedOutput = "item 005.txt";

            this.ReplaceWith = "00";
            List<ItemForRenaming> input = new List<ItemForRenaming>();
            input.Add(new ItemForRenaming { NewName = prependCheckInput, OldName = prependCheckInput, IsFile = true });
            List<ItemForRenaming> ret = RenamingProcessor.Method1(input, true,
                prependCheckRegex, prependCheckRegex, "00", new MatchEvaluator(this.MatchTransformation_Prepend));
            Assert.AreEqual(
                string.Compare(ret[0].NewName, prependCheckExpectedOutput, false), 0,
                string.Format("Actual: {0}, Expected: {1}", ret[0].NewName, prependCheckExpectedOutput));
        }
              
                [TestMethod]
        public void Prepend_Test4_Success()
        {
            prependCheckRegex = @"^\w+\d{2}\.txt";
            prependCheckInput = "item 005.txt";
            prependCheckExpectedOutput = "item 005.txt";

            this.ReplaceWith = "00";
            List<ItemForRenaming> input = new List<ItemForRenaming>();
            input.Add(new ItemForRenaming { NewName = prependCheckInput, OldName = prependCheckInput, IsFile = true });
            List<ItemForRenaming> ret = RenamingProcessor.Method1(input, true,
                prependCheckRegex, prependCheckRegex, "00", new MatchEvaluator(this.MatchTransformation_Prepend));
            Assert.AreEqual(
                string.Compare(ret[0].NewName, prependCheckExpectedOutput, false), 0,
                string.Format("Actual: {0}, Expected: {1}", ret[0].NewName, prependCheckExpectedOutput));
        }
        
        
        
        #region MatchEvaluatorTransformations

        //copied from http://msdn.microsoft.com/en-us/library/cft8645c(VS.80).aspx
        private string MatchTransformation_CapText(Match m)
        {
            string x = m.ToString();
            if (char.IsLower(x[0]))
                return char.ToUpper(x[0]) + x.Substring(1, x.Length - 1);

            return x;
        }

        private string MatchTransformation_Prepend(Match m)
        {
            return ReplaceWith + m;
        }

        private string MatchTransformation_Append(Match m)
        {
            return m + ReplaceWith;
        }

        private string MatchTransformation_Remove(Match m)
        {
            return string.Empty;
        }
        #endregion
    }
}
