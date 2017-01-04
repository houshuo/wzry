namespace PigeonCoopToolkit.Generic
{
    using System;

    [Serializable]
    public class VersionInformation
    {
        public int Major = 1;
        public int Minor;
        public string Name;
        public int Patch;

        public VersionInformation(string name, int major, int minor, int patch)
        {
            this.Name = name;
            this.Major = major;
            this.Minor = minor;
            this.Patch = patch;
        }

        public bool Match(VersionInformation other, bool looseMatch)
        {
            if (looseMatch)
            {
                return (((other.Name == this.Name) && (other.Major == this.Major)) && (other.Minor == this.Minor));
            }
            return ((((other.Name == this.Name) && (other.Major == this.Major)) && (other.Minor == this.Minor)) && (other.Patch == this.Patch));
        }

        public override string ToString()
        {
            object[] args = new object[] { this.Name, this.Major, this.Minor, this.Patch };
            return string.Format("{0} {1}.{2}.{3}", args);
        }
    }
}

