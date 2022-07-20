using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Good iPhone12 = new Good("IPhone 12");
            Good iPhone11 = new Good("IPhone 11");

            Warehouse warehouse = new Warehouse();

            Shop shop = new Shop(warehouse);

            warehouse.Add(iPhone12, 10);
            warehouse.Add(iPhone11, 1);

            //Вывод всех товаров на складе с их остатком

            Cart cart = shop.CreateCart();
            cart.Add(iPhone12, 4);
            cart.Add(iPhone11, 1); //при такой ситуации возникает ошибка так, как нет нужного количества товара на складе

            //Вывод всех товаров в корзине
            ShowAllGoods(cart);

            Console.WriteLine(cart.CreateOrder().PayLink);

            cart.Add(iPhone12, 9); //Ошибка, после заказа со склада убираются заказанные товары
        }

        public static void ShowAllGoods(ICellsCollection cells)
        {
            foreach (var item in cells)
            {
                Console.WriteLine($"{item.Good.Title} : {item.Count}");
            }
        }
    }

    public interface IReadOnlyCells : IEnumerable<Cells>
    { }

    public interface ICellsCollection : IReadOnlyCells
    {
        void Add(Good good, int count);
        void Remove(Good good, int count);
        bool Contains(Good good, int count);
    }

    public class Good
    {
        private string _title;

        public string Title => _title;

        public Good(string title)
        {
            if (string.IsNullOrEmpty(title))
                throw new Exception("Отсутствует название товара");

            _title = title;
        }
    }

    public class Cells
    {
        public int Count { get; }
        public Good Good { get; }

        public Cells(Good good, int count)
        {
            Good = good ?? throw new InvalidOperationException("Товар не может быть пустым");
            if (count < 0)
                throw new Exception("Кол-во товара не может быть 0 или меньше 0");
            Count = count;
        }
    }

    public class Cart : ICellsCollection
    {
        private ICellsCollection _goods;
        private ICellsCollection _inCart;

        public Cart(ICellsCollection goods)
        {
            _goods = goods;
            _inCart = new GoodsCollection();
        }

        public IEnumerator<Cells> GetEnumerator()
        {
            return _inCart.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(Good good, int count)
        {
            if (!_goods.Contains(good, count))
                throw new Exception("Количество товара меньше запрашиваемого кол-ва");
            _inCart.Add(good, count);
        }

        public void Remove(Good good, int count)
        {
            _inCart.Remove(good, count);
        }

        public bool Contains(Good good, int count)
        {
            return _inCart.Contains(good, count);
        }

        internal Order CreateOrder()
        {
            return new Order(this);
        }
    }

    class Order : IReadOnlyCells
    {
        private readonly ReadOnlyCollection<Cells> _goods;

        public string PayLink { get; }

        public Order(IEnumerable<Cells> products)
        {
            _goods = new ReadOnlyCollection<Cells>(products.ToList());
            PayLink = "случайная строка";
        }

        public IEnumerator<Cells> GetEnumerator()
        {
            return _goods.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    class Shop
    {
        private Warehouse _warehouse;

        public Shop(Warehouse warehouse)
        {
            _warehouse = warehouse ?? throw new Exception("Склад должен быть не пустым!");
        }

        public Cart CreateCart()
        {
            return new Cart(_warehouse);
        }
    }

    class GoodsCollection : ICellsCollection
    {
        private Dictionary<Good, int> _storage;

        public GoodsCollection()
        {
            _storage = new Dictionary<Good, int>();
        }

        public void Add(Good good, int count)
        {
            if (good == null)
                throw new Exception("Продукт не может быть без названия");

            if (count <= 0)
                throw new Exception("Кол-во не может быть отрицательным или 0");

            if (!_storage.TryGetValue(good, out int currentCount))
            {
                _storage.Add(good, count);
                return;
            }

            _storage[good] = currentCount + count;
        }

        public void Remove(Good good, int count)
        {

            if (good == null)
                throw new Exception("Продукт не может быть без названия");

            if (count <= 0)
                throw new Exception("Кол-во не может быть отрицательным или 0");

            if (!Contains(good, count))
                throw new Exception("Нету товара для удаления!");

            _storage[good] -= count;

            if (_storage[good] == 0)
                _storage.Remove(good);
        }

        public bool Contains(Good good, int count)
        {
            if (good == null)
                throw new Exception("Продукт не может быть без названия");

            if (count <= 0)
                throw new Exception("Кол-во не может быть отрицательным или 0");

            if (!_storage.TryGetValue(good, out int currentCount))
                return false;

            return currentCount >= count;
        }

        public IEnumerator<Cells> GetEnumerator()
        {
            foreach (var item in _storage)
            {
                yield return new Cells(item.Key, item.Value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    class Warehouse: ICellsCollection
    {
        private ICellsCollection _cells;

        public Warehouse()
        {
            _cells = new GoodsCollection();
        }

        public void Add(Good good, int count)
        {
            _cells.Add(good, count);
        }

        public void Remove(Good good, int count)
        {
            _cells.Remove(good, count);
        }

        public bool Contains(Good good, int count)
        {
            return _cells.Contains(good, count);
        }

        public IEnumerator<Cells> GetEnumerator()
        {
            return _cells.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
