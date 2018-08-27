﻿using UnityEngine;

namespace Mahjong
{
    public enum TileVisibility : byte
    {
        FaceDown,
        FaceUp,
        InHand
    }

    public class Tile : MonoBehaviour
    {
        private TileID id;
        private bool set = false;
        private TileRenderer tileRenderer;
        private int _accessKey = 0;
        private TileVisibility visibility;
        public bool StolenDiscard { get; set; }

        //Initialization
        private void Start()
        {
            tileRenderer = gameObject.GetComponent<TileRenderer>();
        }

        //Sets the tile. Can only be set once. 
        public void Set(TileID.Suits suit, byte number, bool dora = false, bool aka = false)
        {
            if (set) return;
            id = new TileID(suit, number, aka);
            set = true;
        }

        //Sets the owner of this tile by setting the access key for read permission
        //only if there is no current owner
        public void SetOwner(int newAccessKey)
        {
            if (_accessKey != 0) return;
            _accessKey = newAccessKey;
        }

        //Releases ownership of this tile by resetting the access key for read permission
        public void ReleaseOwnership(int accessKey)
        {
            if (accessKey != _accessKey) return;
            _accessKey = 0;
        }

        //Reveals the tile and releases ownership (all objects will have read permission)
        public void SetVisibility(TileVisibility value, int accessKey = 0)
        {
            //If the value is the same, don't bother
            if (visibility == value) return; 
            //Check if locked for ownership permissions and if so, check access key
            if (_accessKey != 0 && accessKey != _accessKey) return;
            //Set visibility
            visibility = value;
            //If it's face up, there's no point in restricting read permissions anymore.
            if (visibility == TileVisibility.FaceUp) ReleaseOwnership(_accessKey);
            //Pass visibility setting to renderer
            tileRenderer.Visibility = visibility;
        }

        //Queries the tile. If the tile is locked and wrong access key is passed, return Hidden
        public TileID Query(int accessKey = 0)
        {
            if (visibility == TileVisibility.FaceUp || accessKey == _accessKey)
                return id.Copy();
            else
                return TileID.Hidden;
        }

    }

}
