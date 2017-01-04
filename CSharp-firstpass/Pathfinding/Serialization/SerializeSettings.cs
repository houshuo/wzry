namespace Pathfinding.Serialization
{
    using System;

    public class SerializeSettings
    {
        public bool editorSettings;
        public bool nodes = true;
        public bool prettyPrint;

        public static SerializeSettings All
        {
            get
            {
                return new SerializeSettings { nodes = true };
            }
        }

        public static SerializeSettings Settings
        {
            get
            {
                return new SerializeSettings { nodes = false };
            }
        }
    }
}

