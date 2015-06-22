using System;

namespace TwixelApp
{
    public class GameGridViewBinding
    {
        public Uri Image { get; set; }
        public string Name { get; set; }

        public GameGridViewBinding(string name, Uri image)
        {
            Name = name;
            Image = image;
        }
    }
}
