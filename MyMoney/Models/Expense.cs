using LiteDB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

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
    }
}
}
