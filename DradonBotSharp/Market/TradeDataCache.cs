using System;

namespace DradonBotSharp.Market
{
    [Serializable]
    struct TradeDataCache
    {
        private readonly string _itemName;
        public int price;
        public int quantity;
        public readonly ulong guildID;
        public readonly ulong ownerID;
        

        public TradeDataCache(string itemName, int price, int quantity, ulong guildID, ulong ownerID)
        {
            this._itemName = itemName;
            this.price = price;
            this.quantity = quantity;
            this.guildID = guildID;
            this.ownerID = ownerID;
        }

        public string ItemName => _itemName;

        public void UpdatePrice(int price) => this.price = price;
        

        public void UpdateQuantity(int quantity)  => this.quantity = quantity;

        public override bool Equals(object obj)
        {
            if (!(obj is TradeDataCache))
            {
                return false;
            }

            var cache = (TradeDataCache)obj;
            return _itemName == cache._itemName &&
                   guildID == cache.guildID &&
                   ownerID == cache.ownerID;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_itemName, guildID, ownerID);
        }
    } 
}
