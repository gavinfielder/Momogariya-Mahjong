using NUnit.Framework;

namespace Mahjong.Tests
{
    public class TileCreationTests
    {
        [Test]
        public void Sanman_is_manzu()
        {
            TileID tile = new TileID(TileID.Suits.Man, 3);
            Assert.AreEqual(TileID.Suits.Man, tile.Suit);
        }

        [Test]
        public void Sanman_is_san()
        {
            TileID tile = new TileID(TileID.Suits.Man, 3);
            Assert.AreEqual(3, tile.Number);
        }

        [Test]
        public void Ton_is_kaze()
        {
            TileID tile = new TileID(TileID.Suits.Kaze, TileID.TON);
            Assert.AreEqual(TileID.Suits.Kaze, tile.Suit);
        }

        [Test]
        public void Ton_is_TonNumber()
        {
            TileID tile = new TileID(TileID.Suits.Kaze, TileID.TON);
            Assert.AreEqual(TileID.TON, tile.Number);
        }

        [Test]
        public void Haku_is_Sangen()
        {
            TileID tile = new TileID(TileID.Suits.Sangen, TileID.HAKU);
            Assert.AreEqual(TileID.Suits.Sangen, tile.Suit);
        }

        [Test]
        public void Haku_is_HakuNumber()
        {
            TileID tile = new TileID(TileID.Suits.Sangen, TileID.HAKU);
            Assert.AreEqual(TileID.HAKU, tile.Number);
        }

        [Test]
        public void Above_9_is_invalid_man()
        {
            TileID tile = new TileID(TileID.Suits.Man, 10);
            Assert.IsTrue(tile == TileID.Invalid);
        }

    }

    public class TileClassificationTests
    {
        [Test]
        public void Nipin_is_not_routou()
        {
            TileID tile = new TileID(TileID.Suits.Pin, 2);
            Assert.IsFalse(tile.Routou);
        }

        [Test]
        public void Nipin_is_not_jihai()
        {
            TileID tile = new TileID(TileID.Suits.Pin, 2);
            Assert.IsFalse(tile.Jihai);
        }

        [Test]
        public void Paapin_is_not_routou()
        {
            TileID tile = new TileID(TileID.Suits.Pin, 8);
            Assert.IsFalse(tile.Routou);
        }

        [Test]
        public void Paapin_is_not_jihai()
        {
            TileID tile = new TileID(TileID.Suits.Pin, 8);
            Assert.IsFalse(tile.Jihai);
        }

        [Test]
        public void Iipin_is_routou()
        {
            TileID tile = new TileID(TileID.Suits.Pin, 1);
            Assert.IsTrue(tile.Routou);
        }

        [Test]
        public void Iipin_is_not_jihai()
        {
            TileID tile = new TileID(TileID.Suits.Pin, 1);
            Assert.IsFalse(tile.Jihai);
        }

        [Test]
        public void Chuupin_is_routou()
        {
            TileID tile = new TileID(TileID.Suits.Pin, 9);
            Assert.IsTrue(tile.Routou);
        }

        [Test]
        public void Chuupin_is_not_jihai()
        {
            TileID tile = new TileID(TileID.Suits.Pin, 9);
            Assert.IsFalse(tile.Jihai);
        }

        [Test]
        public void Shaa_is_not_routou()
        {
            TileID tile = new TileID(TileID.Suits.Kaze, TileID.SHAA);
            Assert.IsFalse(tile.Routou);
        }

        [Test]
        public void Shaa_is_jihai()
        {
            TileID tile = new TileID(TileID.Suits.Kaze, TileID.SHAA);
            Assert.IsTrue(tile.Jihai);
        }

        [Test]
        public void Pei_is_not_routou()
        {
            TileID tile = new TileID(TileID.Suits.Kaze, TileID.PEI);
            Assert.IsFalse(tile.Routou);
        }

        [Test]
        public void Pei_is_jihai()
        {
            TileID tile = new TileID(TileID.Suits.Kaze, TileID.PEI);
            Assert.IsTrue(tile.Jihai);
        }

        [Test]
        public void Ton_is_not_routou()
        {
            TileID tile = new TileID(TileID.Suits.Kaze, TileID.TON);
            Assert.IsFalse(tile.Routou);
        }

        [Test]
        public void Ton_is_jihai()
        {
            TileID tile = new TileID(TileID.Suits.Kaze, TileID.TON);
            Assert.IsTrue(tile.Jihai);
        }

        [Test]
        public void Hatsu_is_not_routou()
        {
            TileID tile = new TileID(TileID.Suits.Sangen, TileID.HATSU);
            Assert.IsFalse(tile.Routou);
        }

        [Test]
        public void Hatsu_is_jihai()
        {
            TileID tile = new TileID(TileID.Suits.Sangen, TileID.HATSU);
            Assert.IsTrue(tile.Jihai);
        }

        [Test]
        public void Chun_is_not_routou()
        {
            TileID tile = new TileID(TileID.Suits.Sangen, TileID.CHUN);
            Assert.IsFalse(tile.Routou);
        }

        [Test]
        public void Chun_is_jihai()
        {
            TileID tile = new TileID(TileID.Suits.Sangen, TileID.CHUN);
            Assert.IsTrue(tile.Jihai);
        }

        [Test]
        public void Uuman_is_not_routou()
        {
            TileID tile = new TileID(TileID.Suits.Man, 5);
            Assert.IsFalse(tile.Routou);
        }

        [Test]
        public void Uuman_is_not_jihai()
        {
            TileID tile = new TileID(TileID.Suits.Man, 5);
            Assert.IsFalse(tile.Jihai);
        }

        [Test]
        public void Chuusou_is_routou()
        {
            TileID tile = new TileID(TileID.Suits.Sou, 9);
            Assert.IsTrue(tile.Routou);
        }

        [Test]
        public void Chuusou_is_not_jihai()
        {
            TileID tile = new TileID(TileID.Suits.Sou, 9);
            Assert.IsFalse(tile.Jihai);
        }

    }

    public class TileComparisonTests
    {
        [Test]
        public void Uuman_Uupin_notequal()
        {
            TileID t1 = new TileID(TileID.Suits.Man, 5);
            TileID t2 = new TileID(TileID.Suits.Pin, 5);
            Assert.IsFalse(t1 == t2);
        }

        [Test]
        public void Uuman_Ryuuman_notequal()
        {
            TileID t1 = new TileID(TileID.Suits.Man, 5);
            TileID t2 = new TileID(TileID.Suits.Man, 6);
            Assert.IsFalse(t1 == t2);
        }

        [Test]
        public void Uuman_Uuman_equal()
        {
            TileID t1 = new TileID(TileID.Suits.Man, 5);
            TileID t2 = new TileID(TileID.Suits.Man, 5);
            Assert.IsTrue(t1 == t2);
        }

        [Test]
        public void Ton_Nan_notequal()
        {
            TileID t1 = new TileID(TileID.Suits.Kaze, TileID.TON);
            TileID t2 = new TileID(TileID.Suits.Kaze, TileID.NAN);
            Assert.IsFalse(t1 == t2);
        }

        [Test]
        public void Ton_Ton_equal()
        {
            TileID t1 = new TileID(TileID.Suits.Kaze, TileID.TON);
            TileID t2 = new TileID(TileID.Suits.Kaze, TileID.TON);
            Assert.IsTrue(t1 == t2);
        }

        [Test]
        public void Ton_Chun_notequal()
        {
            TileID t1 = new TileID(TileID.Suits.Kaze, TileID.TON);
            TileID t2 = new TileID(TileID.Suits.Sangen, TileID.CHUN);
            Assert.IsFalse(t1 == t2);
        }

        [Test]
        public void Chun_Chun_equal()
        {
            TileID t1 = new TileID(TileID.Suits.Sangen, TileID.CHUN);
            TileID t2 = new TileID(TileID.Suits.Sangen, TileID.CHUN);
            Assert.IsTrue(t1 == t2);
        }

        [Test]
        public void Uuman_Uupin_notequal_negativelogic()
        {
            TileID t1 = new TileID(TileID.Suits.Man, 5);
            TileID t2 = new TileID(TileID.Suits.Pin, 5);
            Assert.IsTrue(t1 != t2);
        }

        [Test]
        public void Uuman_Ryuuman_notequal_negativelogic()
        {
            TileID t1 = new TileID(TileID.Suits.Man, 5);
            TileID t2 = new TileID(TileID.Suits.Man, 6);
            Assert.IsTrue(t1 != t2);
        }

        [Test]
        public void Uuman_Uuman_equal_negativelogic()
        {
            TileID t1 = new TileID(TileID.Suits.Man, 5);
            TileID t2 = new TileID(TileID.Suits.Man, 5);
            Assert.IsFalse(t1 != t2);
        }
    }

    public class TileDoraTests
    {
        [Test]
        public void Get_dora_simple()
        {
            TileID tile = new TileID(TileID.Suits.Man, 4);
            Assert.AreEqual(new TileID(TileID.Suits.Man, 5), tile.GetDoraFromIndicator());
        }

        [Test]
        public void Get_dora_wrap()
        {
            TileID tile = new TileID(TileID.Suits.Man, 9);
            Assert.AreEqual(new TileID(TileID.Suits.Man, 1), tile.GetDoraFromIndicator());
        }

        [Test]
        public void Get_dora_ton()
        {
            TileID tile = new TileID(TileID.Suits.Kaze, TileID.TON);
            Assert.AreEqual(new TileID(TileID.Suits.Kaze, TileID.NAN), tile.GetDoraFromIndicator());
        }

        [Test]
        public void Get_dora_nan()
        {
            TileID tile = new TileID(TileID.Suits.Kaze, TileID.NAN);
            Assert.AreEqual(new TileID(TileID.Suits.Kaze, TileID.SHAA), tile.GetDoraFromIndicator());
        }

        [Test]
        public void Get_dora_shaa()
        {
            TileID tile = new TileID(TileID.Suits.Kaze, TileID.SHAA);
            Assert.AreEqual(new TileID(TileID.Suits.Kaze, TileID.PEI), tile.GetDoraFromIndicator());
        }

        [Test]
        public void Get_dora_pei()
        {
            TileID tile = new TileID(TileID.Suits.Kaze, TileID.PEI);
            Assert.AreEqual(new TileID(TileID.Suits.Kaze, TileID.TON), tile.GetDoraFromIndicator());
        }

        [Test]
        public void Get_dora_chun()
        {
            TileID tile = new TileID(TileID.Suits.Sangen, TileID.CHUN);
            Assert.AreEqual(new TileID(TileID.Suits.Sangen, TileID.HAKU), tile.GetDoraFromIndicator());
        }

        [Test]
        public void Get_dora_haku()
        {
            TileID tile = new TileID(TileID.Suits.Sangen, TileID.HAKU);
            Assert.AreEqual(new TileID(TileID.Suits.Sangen, TileID.HATSU), tile.GetDoraFromIndicator());
        }

        [Test]
        public void Get_dora_hatsu()
        {
            TileID tile = new TileID(TileID.Suits.Sangen, TileID.HATSU);
            Assert.AreEqual(new TileID(TileID.Suits.Sangen, TileID.CHUN), tile.GetDoraFromIndicator());
        }
    }

}
