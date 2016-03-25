using System.Drawing;
using System.IO;
using AlphaLaunch.Core.Icons;
using AlphaLaunch.Core.Indexes;

namespace AlphaLaunch.Core
{
    public class FileItem : IIndexable
    {
        public string FullName { get; set; }
        public string Name { get; set; }
        public string BoostIdentifier { get { return FullName; } }

        public FileItem(string fullName)
        {
            FullName = fullName;
            Name = Path.GetFileName(fullName);
        }

        public object GetDetails()
        {
            return new FileItemDetails(FullName);
        }

        public IIconResolver GetIconResolver()
        {
            return new FileIconResolver(FullName);
        }
    }
}