namespace Assets.Scripts.GameSystem
{
    using System;

    public class CTextImageNode
    {
        public bool bText;
        public string content;
        public float height;
        public float posX;
        public float posY;
        public float width;

        public CTextImageNode(string ct, bool bText, float width, float height, float x, float y)
        {
            this.content = ct;
            this.bText = bText;
            this.width = width;
            this.height = height;
            this.posX = x;
            this.posY = y;
        }
    }
}

