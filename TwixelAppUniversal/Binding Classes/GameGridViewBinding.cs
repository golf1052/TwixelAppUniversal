using System;
using TwixelAPI;

namespace TwixelAppUniversal
{
    public class GameGridViewBinding
    {
        public string Name { get; set; }        
        public long? Viewers { get; set; }
        public long? Channels { get; set; }
        public Uri Image { get; set; }

        public Game game;
        
        public GameGridViewBinding(string name, long? viewers, long? channels, Uri image)
        {
            Name = name;
            Viewers = viewers;
            Channels = channels;
            Image = image;
        }

        public GameGridViewBinding(Game game)
        {
            Name = game.name;
            Viewers = game.viewers;
            Channels = game.channels;
            Image = game.box["large"];
            this.game = game;
        }
    }
}
