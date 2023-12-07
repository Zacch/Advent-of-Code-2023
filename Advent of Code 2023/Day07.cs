// ReSharper disable LoopCanBeConvertedToQuery
namespace Advent_of_Code_2023;

public abstract class Day07
{

    private enum HandType { HighCard, OnePair, TwoPair, ThreeOfAKind, FullHouse, FourOfAKind, FiveOfAKind }

    private static readonly Dictionary<char, int> CardValues = new()
    {
        ['2'] = 2,
        ['3'] = 3,
        ['4'] = 4,
        ['5'] = 5,
        ['6'] = 6,
        ['7'] = 7,
        ['8'] = 8,
        ['9'] = 9,
        ['T'] = 10,
        ['J'] = 11,
        ['Q'] = 12,
        ['K'] = 13,
        ['A'] = 14
    };

    private static readonly Dictionary<char, int> Part2Values = new()
    {
        ['J'] = 1,
        ['2'] = 2,
        ['3'] = 3,
        ['4'] = 4,
        ['5'] = 5,
        ['6'] = 6,
        ['7'] = 7,
        ['8'] = 8,
        ['9'] = 9,
        ['T'] = 10,
        ['Q'] = 12,
        ['K'] = 13,
        ['A'] = 14
    };

    private class Hand : IComparable<Hand>
    {
        protected readonly char[] Cards;
        public readonly int Bid;
        protected HandType HandType;

        public Hand(char[] cards, int bid)
        {
            Cards = cards;
            Bid = bid;
            HandType = DetermineType();
        }

        protected HandType DetermineType()
        {
            var buckets = new Dictionary<char, int>();
            foreach (var card in Cards)
            {
                buckets.TryAdd(card, 0);
                buckets[card]++;
            }

            if (buckets.Count == 1) return HandType.FiveOfAKind;
            if (buckets.Values.Any(i => i == 4)) return HandType.FourOfAKind;
            if (buckets.Values.Any(i => i == 3))
                return buckets.Values.Any(i => i == 2) ? HandType.FullHouse : HandType.ThreeOfAKind;
            return buckets.Values.Count(i => i == 2) switch
            {
                0 => HandType.HighCard,
                1 => HandType.OnePair,
                2 => HandType.TwoPair,
                _ => throw new ApplicationException()
            };
        }

        public int CompareTo(Hand? other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            if (HandType != other.HandType) return HandType.CompareTo(other.HandType);
            for (var i = 0; i < 5; i++)
                if (Cards[i] != other.Cards[i]) return CardValues[Cards[i]].CompareTo(CardValues[other.Cards[i]]);
            return 0;
        }
    }

    private class SecondHand : Hand, IComparable<SecondHand>
    {
        public SecondHand(char[] cards, int bid) : base(cards, bid)
        {
            HandType = DetermineType();
        }

        private new HandType DetermineType()
        {
            var jokers = Cards.Count(c => c == 'J');
            if (jokers == 0) return base.DetermineType();
            if (jokers > 3) return HandType.FiveOfAKind;

            var buckets = new Dictionary<char, int>();
            foreach (var card in Cards.Where(c => c != 'J'))
            {
                buckets.TryAdd(card, 0);
                buckets[card]++;
            }

            if (buckets.Count == 1) return HandType.FiveOfAKind;
            if (buckets.Values.Any(i => i + jokers == 4)) return HandType.FourOfAKind;
            if (buckets.Values.Any(i => i + jokers == 3))
                return buckets.Values.Count(i => i == 2) switch
                {
                    0 => HandType.ThreeOfAKind,
                    1 => HandType.ThreeOfAKind,
                    2 => HandType.FullHouse,
                    _ => throw new ApplicationException()
                };
            if (buckets.Values.Any(i => i >= 2)) throw new ApplicationException();
            return HandType.OnePair;
        }

        public int CompareTo(SecondHand? other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            if (HandType != other.HandType) return HandType.CompareTo(other.HandType);
            for (var i = 0; i < 5; i++)
                if (Cards[i] != other.Cards[i]) return Part2Values[Cards[i]].CompareTo(Part2Values[other.Cards[i]]);
            return 0;
        }
    }

    public static void Solve()
    {
        var lines = File.ReadLines("Day07.txt").ToArray();

        var hands = new SortedSet<Hand>();
        var part2Hands = new SortedSet<SecondHand>();

        foreach (var line in lines)
        {
            var parts = line.Split(' ');
            var hand = new Hand(parts[0].ToCharArray(), int.Parse(parts[1]));
            hands.Add(hand);
            var secondHand = new SecondHand(parts[0].ToCharArray(), int.Parse(parts[1]));
            part2Hands.Add(secondHand);
        }

        var multiplier = 1L;
        var part1 = hands.Sum(hand => multiplier++ * hand.Bid);
        Console.WriteLine($"Part 1: {part1}");

        multiplier = 1L;
        var part2 = part2Hands.Sum(secondHand => multiplier++ * secondHand.Bid);
        Console.WriteLine($"Part 2: {part2}");
    }
}