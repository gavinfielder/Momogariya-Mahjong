using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace Mahjong.Tests
{
    public class WallTests
    {
        [Test]
        public void Build_wall_count_drawable()
        {
            WallManager wall = new WallManager();
            wall.Build(RuleSets.DefaultRules);
            Assert.AreEqual(122, wall.NumberDrawsRemaining);
        }

        [Test]
        public void Build_wall_count_dead()
        {
            WallManager wall = new WallManager();
            wall.Build(RuleSets.DefaultRules);
            Assert.AreEqual(14, wall.NumberOfDeadTiles);
        }

        [Test]
        public void Build_wall_count_hidden_dead()
        {
            WallManager wall = new WallManager();
            wall.Build(RuleSets.DefaultRules);
            Assert.AreEqual(13, wall.NumberOfHiddenDeadTiles);
        }

        [Test]
        public void Build_wall_count_dora()
        {
            WallManager wall = new WallManager();
            wall.Build(RuleSets.DefaultRules);
            Assert.AreEqual(1, wall.Doras.Count);
        }

        [Test]
        public void Drawing_decreases_drawable_tiles_by_one()
        {
            WallManager wall = new WallManager();
            wall.Build(RuleSets.DefaultRules);
            int prev = wall.NumberDrawsRemaining;
            wall.Draw();
            Assert.AreEqual(prev - 1, wall.NumberDrawsRemaining);
        }

        [Test]
        public void Cannot_draw_more_than_drawable()
        {
            WallManager wall = new WallManager();
            wall.Build(RuleSets.DefaultRules);
            int draws = wall.NumberDrawsRemaining;
            for (int i = 0; i < draws; i++)
            {
                wall.Draw();
            }
            Tile next = wall.Draw();
            Assert.AreEqual(Tile.Invalid, next);
        }

        [Test]
        public void Sequence_of_forced_draw_order()
        {
            WallManager wall = new WallManager();
            //Force a specific wall
            List<Tile> firstDraws = new List<Tile>();
            List<Tile> doras = new List<Tile>();
            firstDraws.Add(new Tile(Tile.Suits.Pin, 2));
            firstDraws.Add(new Tile(Tile.Suits.Kaze, Tile.SHAA));
            firstDraws.Add(new Tile(Tile.Suits.Man, 9));
            firstDraws.Add(new Tile(Tile.Suits.Sangen, Tile.CHUN));
            wall.Build_ForceOrder(RuleSets.DefaultRules, firstDraws, doras);
            //Make sure the first 4 draws are as defined above
            Tile t = wall.Draw();
            Assert.IsTrue(t == new Tile(Tile.Suits.Pin, 2));
            t = wall.Draw();
            Assert.IsTrue(t == new Tile(Tile.Suits.Kaze, Tile.SHAA));
            t = wall.Draw();
            Assert.IsTrue(t == new Tile(Tile.Suits.Man, 9));
            t = wall.Draw();
            Assert.IsTrue(t == new Tile(Tile.Suits.Sangen, Tile.CHUN));

        }

        [Test]
        public void Output_wall_to_file()
        {
            WallManager wall = new WallManager();
            wall.Build(RuleSets.DefaultRules);
            StreamWriter fout = new StreamWriter("output.txt");
            fout.WriteLine("New Wall:");
            fout.WriteLine("Dora: " + wall.Doras[0].ToString());
            fout.WriteLine("Wall tiles, in order: ");
            Tile tile;
            int i = 1;
            while (wall.NumberDrawsRemaining > 0)
            {
                tile = wall.Draw();
                fout.WriteLine("    " + i + ": " + tile.ToString());
                i++;
            }
            fout.Close();
        }
    }
}
