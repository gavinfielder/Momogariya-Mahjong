using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mahjong
{
    class TileRenderer : MonoBehaviour
    {
        #pragma warning disable CS0649
        public SpriteRenderer BaseSpriteRenderer;
        public SpriteRenderer FaceSpriteRenderer;
        #pragma warning restore CS0649

        //Gets or sets the tile that this renderer shows
        private Tile face;
        public Tile Face
        {
            get
            {
                return face.Copy();
            }
            set
            {
                if (value == Tile.Invalid) return;
                if (value == Tile.Hidden)
                {
                    BaseSpriteRenderer.sprite = Resources.Load<Sprite>("Tiles/TempDev/Back-Gold");
                    FaceSpriteRenderer.sprite = null;
                    face = value;
                }
                else if (value == Tile.HiddenHand)
                {
                    BaseSpriteRenderer.sprite = Resources.Load<Sprite>("Tiles/TempDev/Top");
                    FaceSpriteRenderer.sprite = null;
                    face = value;
                }
                else
                {
                    BaseSpriteRenderer.sprite = Resources.Load<Sprite>("Tiles/TempDev/Front");
                    FaceSpriteRenderer.sprite = Resources.Load<Sprite>(GetAssetPath(value));
                    face = value.Copy();
                }
            }

        }

        //Sets the sorting orders in the sprite rendering layer
        public int Layer
        {
            set
            {
                BaseSpriteRenderer.sortingOrder = value * 2;
                FaceSpriteRenderer.sortingOrder = value * 2 + 1;
            }
        }
        
        //Returns the asset path of the face
        private string GetAssetPath(Tile tile)
        {
            string path = "Tiles/TempDev/";
            if (tile.Suit == Tile.Suits.Man)
            {
                path += "Man";
                path += tile.Number;
            }
            else if (tile.Suit == Tile.Suits.Pin)
            {
                path += "Pin";
                path += tile.Number;
            }
            else if (tile.Suit == Tile.Suits.Sou)
            {
                path += "Sou";
                path += tile.Number;
            }
            else if (tile.Suit == Tile.Suits.Kaze)
            {
                if (tile.Number == Tile.TON)
                    path += "Ton";
                else if (tile.Number == Tile.NAN)
                    path += "Nan";
                else if(tile.Number == Tile.SHAA)
                    path += "Shaa";
                else if(tile.Number == Tile.PEI)
                    path += "Pei";
            }
            else if (tile.Suit == Tile.Suits.Sangen)
            {
                if (tile.Number == Tile.CHUN)
                    path += "Chun";
                else if (tile.Number == Tile.HAKU)
                    path += "Haku";
                else if (tile.Number == Tile.HATSU)
                    path += "Hatsu";
            }
            return path;
        }
    }
}
