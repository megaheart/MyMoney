using LiteDB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Encodings.Web;
using j = System.Text.Json;

namespace MyMoney.Models
{
    public class Expense : NotifiableObject
    {
        private Expense() { }
        public Expense(IEnumerable<Tag> tags, Item item)
        {
            _tags = new ObservableCollection<Tag>(tags);
            _item = item;
        }
        private Amount _amount;
        public Amount Amount
        {
            get => _amount;
            set
            {
                if (value != _amount)
                {
                    _amount = value;
                    OnPropertyChanged("Amount");
                }
            }
        }
        private Item _item;
        public Item Item
        {
            get => _item;
            set
            {
                if (value != _item)
                {
                    _item = value;
                    OnPropertyChanged("Amount");
                }
            }
        }
        private ObservableCollection<Tag> _tags;
        [BsonIgnore]
        public ObservableCollection<Tag> Tags
        {
            get => _tags;
        }
        private int _price;
        public int Price
        {
            set
            {
                if (value != _price)
                {
                    _price = value;
                    OnPropertyChanged("Price");
                }
            }
            get => _price;
        }
        private DateTime _time;
        public DateTime Time
        {
            set
            {
                if (value != _time)
                {
                    _time = value;
                    OnPropertyChanged("Time");
                }
            }
            get => _time;
        }
        //public int ExpenseListId { set; get; }
        [BsonId]
        public int Id { set; get; }
        public class BsonValueStructureOfExpense
        {
            public Amount Amount { set; get; }
            public int ItemId { set; get; }
            public int[] TagIds { set; get; }
            public int Price { set; get; }
            public DateTime Time { set; get; }
            public int Id { set; get; }

        }
        public BsonValueStructureOfExpense Serialize() => new BsonValueStructureOfExpense()
        {
            Amount = _amount,
            ItemId = Item.Id,
            TagIds = _tags.Select(t => t.Id).ToArray(),
            Price = _price,
            Time = _time,
            Id = Id
        };
        public static Expense Deserialize(BsonValueStructureOfExpense bsonValueStructureOfExpense, IDictionary<int, Tag> tagsDict, IDictionary<int, Item> itemsDict)
        {
            return new Expense()
            {
                _amount = bsonValueStructureOfExpense.Amount,
                _item = itemsDict[bsonValueStructureOfExpense.ItemId],
                _tags = new ObservableCollection<Tag>(bsonValueStructureOfExpense.TagIds.Select(tId => tagsDict[tId])),
                _price = bsonValueStructureOfExpense.Price,
                _time = bsonValueStructureOfExpense.Time,
                Id = bsonValueStructureOfExpense.Id
            };
        }
        public Expense Clone() => new Expense()
        {
            _amount = this._amount,
            _item = this._item,
            _tags = this._tags,
            _price = this._price,
            _time = this._time,
            Id = this.Id
        };
        public void CopyTo(Expense e)
        {
            if (e._amount != this._amount)
            {
                e._amount = this._amount;
                e.OnPropertyChanged("Time");
            }
            if (e._item != this._item)
            {
                e._item = this._item;
                e.OnPropertyChanged("Time");
            }
            if (e._tags != this._tags)
            {
                e._tags = this._tags;
                e.OnPropertyChanged("Time");
            }
            if (e._price != this._price)
            {
                e._price = this._price;
                e.OnPropertyChanged("Time");
            }
            if (e._time != this._time)
            {
                e._time = this._time;
                e.OnPropertyChanged("Time");
            }
            if (e.Id != this.Id)
            {
                e.Id = this.Id;
                e.OnPropertyChanged("Time");
            }
        }
        private static JavaScriptEncoder _encoder;
        public override string ToString()
        {
            if (_encoder == null) _encoder = JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All);
            return "{" + string.Format("\n    Id: {0},\n    Item: {1},\n    Amount: {2},\n    Tags: {3},\n    Price: {4},\n    Time: {5}\n",
                Id,
                j.JsonSerializer.Serialize(_item, new j.JsonSerializerOptions() { Encoder = _encoder }),
                _amount.ToString(),
                j.JsonSerializer.Serialize(_tags, new j.JsonSerializerOptions() { Encoder = _encoder }),
                _price,
                _time) + "}";
        }

    }
}

