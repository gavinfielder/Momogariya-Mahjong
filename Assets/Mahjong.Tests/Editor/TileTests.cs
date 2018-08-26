using NUnit.Framework;

namespace Mahjong.Tests
{
    public class TileCreationTests
    {
        [Test]
        public void Sanman_is_manzu()
        {
            Tile tile = new Tile(Tile.Suits.Man, 3);
            Assert.AreEqual(Tile.Suits.Man, tile.Suit);
        }

        [Test]
        public void Sanman_is_san()
        {
            Tile tile = new Tile(Tile.Suits.Man, 3);
            Assert.AreEqual(3, tile.Number);
        }

        [Test]
        public void Ton_is_kaze()
        {
            Tile tile = new Tile(Tile.Suits.Kaze, Tile.TON);
            Assert.AreEqual(Tile.Suits.Kaze, tile.Suit);
        }

        [Test]
        public void Ton_is_TonNumber()
        {
            Tile tile = new Tile(Tile.Suits.Kaze, Tile.TON);
            Assert.AreEqual(Tile.TON, tile.Number);
        }

        [Test]
        public void Haku_is_Sangen()
        {
            Tile tile = new Tile(Tile.Suits.Sangen, Tile.HAKU);
            Assert.AreEqual(Tile.Suits.Sangen, tile.Suit);
        }

        [Test]
        public void Haku_is_HakuNumber()
        {
            Tile tile = new Tile(Tile.Suits.Sangen, Tile.HAKU);
            Assert.AreEqual(Tile.HAKU, tile.Number);
        }

        [Test]
        public void Above_9_is_invalid_man()
        {
            Tile tile = new Tile(Tile.Suits.Man, 10);
            Assert.IsTrue(tile == Tile.Invalid);
        }

    }

    public class TileClassificationTests
    {
        [Test]
        public void Nipin_is_not_routou()
        {
            Tile tile = new Tile(Tile.Suits.Pin, 2);
            Assert.IsFalse(tile.Routou);
        }

        [Test]
        public void Nipin_is_not_jihai()
        {
            Tile tile = new Tile(Tile.Suits.Pin, 2);
            Assert.IsFalse(tile.Jihai);
        }

        [Test]
        public void Paapin_is_not_routou()
        {
            Tile tile = new Tile(Tile.Suits.Pin, 8);
            Assert.IsFalse(tile.Routou);
        }

        [Test]
        public void Paapin_is_not_jihai()
        {
            Tile tile = new Tile(Tile.Suits.Pin, 8);
            Assert.IsFalse(tile.Jihai);
        }

        [Test]
        public void Iipin_is_routou()
        {
            Tile tile = new Tile(Tile.Suits.Pin, 1);
            Assert.IsTrue(tile.Routou);
        }

        [Test]
        public void Iipin_is_not_jihai()
        {
            Tile tile = new Tile(Tile.Suits.Pin, 1);
            Assert.IsFalse(tile.Jihai);
        }

        [Test]
        public void Chuupin_is_routou()
        {
            Tile tile = new Tile(Tile.Suits.Pin, 9);
            Assert.IsTrue(tile.Routou);
        }

        [Test]
        public void Chuupin_is_not_jihai()
        {
            Tile tile = new Tile(Tile.Suits.Pin, 9);
            Assert.IsFalse(tile.Jihai);
        }

        [Test]
        public void Shaa_is_not_routou()
        {
            Tile tile = new Tile(Tile.Suits.Kaze, Tile.SHAA);
            Assert.IsFalse(tile.Routou);
        }

        [Test]
        public void Shaa_is_jihai()
        {
            Tile tile = new Tile(Tile.Suits.Kaze, Tile.SHAA);
            Assert.IsTrue(tile.Jihai);
        }

        [Test]
        public void Pei_is_not_routou()
        {
            Tile tile = new Tile(Tile.Suits.Kaze, Tile.PEI);
            Assert.IsFalse(tile.Routou);
        }

        [Test]
        public void Pei_is_jihai()
        {
            Tile tile = new Tile(Tile.Suits.Kaze, Tile.PEI);
            Assert.IsTrue(tile.Jihai);
        }

        [Test]
        public void Ton_is_not_routou()
        {
            Tile tile = new Tile(Tile.Suits.Kaze, Tile.TON);
            Assert.IsFalse(tile.Routou);
        }

        [Test]
        public void Ton_is_jihai()
        {
            Tile tile = new Tile(Tile.Suits.Kaze, Tile.TON);
            Assert.IsTrue(tile.Jihai);
        }

        [Test]
        public void Hatsu_is_not_routou()
        {
            Tile tile = new Tile(Tile.Suits.Sangen, Tile.HATSU);
            Assert.IsFalse(tile.Routou);
        }

        [Test]
        public void Hatsu_is_jihai()
        {
            Tile tile = new Tile(Tile.Suits.Sangen, Tile.HATSU);
            Assert.IsTrue(tile.Jihai);
        }

        [Test]
        public void Chun_is_not_routou()
        {
            Tile tile = new Tile(Tile.Suits.Sangen, Tile.CHUN);
            Assert.IsFalse(tile.Routou);
        }

        [Test]
        public void Chun_is_jihai()
        {
            Tile tile = new Tile(Tile.Suits.Sangen, Tile.CHUN);
            Assert.IsTrue(tile.Jihai);
        }

        [Test]
        public void Uuman_is_not_routou()
        {
            Tile tile = new Tile(Tile.Suits.Man, 5);
            Assert.IsFalse(tile.Routou);
        }

        [Test]
        public void Uuman_is_not_jihai()
        {
            Tile tile = new Tile(Tile.Suits.Man, 5);
            Assert.IsFalse(tile.Jihai);
        }

        [Test]
        public void Chuusou_is_routou()
        {
            Tile tile = new Tile(Tile.Suits.Sou, 9);
            Assert.IsTrue(tile.Routou);
        }

        [Test]
        public void Chuusou_is_not_jihai()
        {
            Tile tile = new Tile(Tile.Suits.Sou, 9);
            Assert.IsFalse(tile.Jihai);
        }

    }

    public class TileComparisonTests
    {
        [Test]
        public void Uuman_Uupin_notequal()
        {
            Tile t1 = new Tile(Tile.Suits.Man, 5);
            Tile t2 = new Tile(Tile.Suits.Pin, 5);
            Assert.IsFalse(t1 == t2);
        }

        [Test]
        public void Uuman_Ryuuman_notequal()
        {
            Tile t1 = new Tile(Tile.Suits.Man, 5);
            Tile t2 = new Tile(Tile.Suits.Man, 6);
            Assert.IsFalse(t1 == t2);
        }

        [Test]
        public void Uuman_Uuman_equal()
        {
            Tile t1 = new Tile(Tile.Suits.Man, 5);
            Tile t2 = new Tile(Tile.Suits.Man, 5);
            Assert.IsTrue(t1 == t2);
        }

        [Test]
        public void Ton_Nan_notequal()
        {
            Tile t1 = new Tile(Tile.Suits.Kaze, Tile.TON);
            Tile t2 = new Tile(Tile.Suits.Kaze, Tile.NAN);
            Assert.IsFalse(t1 == t2);
        }

        [Test]
        public void Ton_Ton_equal()
        {
            Tile t1 = new Tile(Tile.Suits.Kaze, Tile.TON);
            Tile t2 = new Tile(Tile.Suits.Kaze, Tile.TON);
            Assert.IsTrue(t1 == t2);
        }

        [Test]
        public void Ton_Chun_notequal()
        {
            Tile t1 = new Tile(Tile.Suits.Kaze, Tile.TON);
            Tile t2 = new Tile(Tile.Suits.Sangen, Tile.CHUN);
            Assert.IsFalse(t1 == t2);
        }

        [Test]
        public void Chun_Chun_equal()
        {
            Tile t1 = new Tile(Tile.Suits.Sangen, Tile.CHUN);
            Tile t2 = new Tile(Tile.Suits.Sangen, Tile.CHUN);
            Assert.IsTrue(t1 == t2);
        }

        [Test]
        public void Uuman_Uupin_notequal_negativelogic()
        {
            Tile t1 = new Tile(Tile.Suits.Man, 5);
            Tile t2 = new Tile(Tile.Suits.Pin, 5);
            Assert.IsTrue(t1 != t2);
        }

        [Test]
        public void Uuman_Ryuuman_notequal_negativelogic()
        {
            Tile t1 = new Tile(Tile.Suits.Man, 5);
            Tile t2 = new Tile(Tile.Suits.Man, 6);
            Assert.IsTrue(t1 != t2);
        }

        [Test]
        public void Uuman_Uuman_equal_negativelogic()
        {
            Tile t1 = new Tile(Tile.Suits.Man, 5);
            Tile t2 = new Tile(Tile.Suits.Man, 5);
            Assert.IsFalse(t1 != t2);
        }
    }

    public class TileDoraTests
    {
        [Test]
        public void Can_set_dora()
        {
            Tile tile = new Tile(Tile.Suits.Man, 7);
            tile.Dora = true;
            Assert.IsTrue(tile.Dora);
        }

        [Test]
        public void Can_construct_as_dora()
        {
            Tile tile = new Tile(Tile.Suits.Man, 7, true);
            Assert.IsTrue(tile.Dora);
        }

        [Test]
        public void Cannot_remove_dora()
        {
            Tile tile = new Tile(Tile.Suits.Man, 7, true);
            tile.Dora = false;
            Assert.IsTrue(tile.Dora);
        }

        [Test]
        public void Get_dora_simple()
        {
            Tile tile = new Tile(Tile.Suits.Man, 4);
            Assert.AreEqual(new Tile(Tile.Suits.Man, 5), tile.GetDoraFromIndicator());
        }

        [Test]
        public void Get_dora_wrap()
        {
            Tile tile = new Tile(Tile.Suits.Man, 9);
            Assert.AreEqual(new Tile(Tile.Suits.Man, 1), tile.GetDoraFromIndicator());
        }

        [Test]
        public void Get_dora_ton()
        {
            Tile tile = new Tile(Tile.Suits.Kaze, Tile.TON);
            Assert.AreEqual(new Tile(Tile.Suits.Kaze, Tile.NAN), tile.GetDoraFromIndicator());
        }

        [Test]
        public void Get_dora_nan()
        {
            Tile tile = new Tile(Tile.Suits.Kaze, Tile.NAN);
            Assert.AreEqual(new Tile(Tile.Suits.Kaze, Tile.SHAA), tile.GetDoraFromIndicator());
        }

        [Test]
        public void Get_dora_shaa()
        {
            Tile tile = new Tile(Tile.Suits.Kaze, Tile.SHAA);
            Assert.AreEqual(new Tile(Tile.Suits.Kaze, Tile.PEI), tile.GetDoraFromIndicator());
        }

        [Test]
        public void Get_dora_pei()
        {
            Tile tile = new Tile(Tile.Suits.Kaze, Tile.PEI);
            Assert.AreEqual(new Tile(Tile.Suits.Kaze, Tile.TON), tile.GetDoraFromIndicator());
        }

        [Test]
        public void Get_dora_chun()
        {
            Tile tile = new Tile(Tile.Suits.Sangen, Tile.CHUN);
            Assert.AreEqual(new Tile(Tile.Suits.Sangen, Tile.HAKU), tile.GetDoraFromIndicator());
        }

        [Test]
        public void Get_dora_haku()
        {
            Tile tile = new Tile(Tile.Suits.Sangen, Tile.HAKU);
            Assert.AreEqual(new Tile(Tile.Suits.Sangen, Tile.HATSU), tile.GetDoraFromIndicator());
        }

        [Test]
        public void Get_dora_hatsu()
        {
            Tile tile = new Tile(Tile.Suits.Sangen, Tile.HATSU);
            Assert.AreEqual(new Tile(Tile.Suits.Sangen, Tile.CHUN), tile.GetDoraFromIndicator());
        }
    }

}
