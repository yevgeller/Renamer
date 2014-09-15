using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenamerUtility
{
    public class ItemForRenaming
    {
        public string OldName { get; set; }
        public string NewName { get; set; }
        public bool IsFile { get; set; }

        public override string ToString()
        {
            string ret = this.IsFile ? "File: " : "Dir: ";
            ret += this.OldName + " -> " + this.NewName;
            return ret;
        }

        public override bool Equals(object obj)
        {
            ItemForRenaming input = obj as ItemForRenaming;
            if (input == null)
                return false;

            return input.NewName == this.NewName &&
                input.OldName == this.OldName &&
                input.IsFile == this.IsFile;
        }
    }
}
