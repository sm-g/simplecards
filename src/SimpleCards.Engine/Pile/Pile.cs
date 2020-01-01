﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MoreLinq;

namespace SimpleCards.Engine
{
    /// <summary>
    /// A set of cards placed on a surface so that they partially or completely overlap.
    /// </summary>
    /// <remarks>
    /// Not inherit from List to hide irrelevant methods (such as Add, Insert).
    /// </remarks>
    public class Pile : IReadOnlyList<Card>
    {
        protected internal List<Card> CardsInPile;

        /// <summary>
        /// Creates pile with given cards.
        /// </summary>
        public Pile(IEnumerable<Card> cards)
        {
            CardsInPile = new List<Card>(cards);
        }

        /// <summary>
        /// Creates empty pile.
        /// </summary>
        public Pile()
        {
            CardsInPile = new List<Card>();
        }

        /// <summary>
        /// Quantity of cards in the pile.
        /// </summary>
        public int Size
        {
            get { return CardsInPile.Count; }
        }

        public bool IsEmpty
        {
            get { return Size == 0; }
        }

        public void Push(Card card, PilePosition p)
        {
            if (CardsInPile.Contains(card, CardByRefEqualityComparer.Instance))
                throw new ArgumentException("Given card instance already in pile", nameof(card));

            p.Push(this, card);
        }

        public void Push(IReadOnlyCollection<Card> cards, PilePosition p)
        {
            if (!cards.AllUnique(x => x, CardByRefEqualityComparer.Instance))
                throw new ArgumentException("Duplicate instances in given cards", nameof(cards));

            if (CardsInPile.Intersect(cards, CardByRefEqualityComparer.Instance).Any())
                throw new ArgumentException("One of given cards instance already in pile", nameof(cards));

            p.Push(this, cards);
        }

        public Card Peek(PilePosition p)
        {
            if (IsEmpty)
                throw new EmptyPileException(this);

            return p.Peek(this);
        }

        public Card Pop(PilePosition p)
        {
            if (IsEmpty)
                throw new EmptyPileException(this);

            return p.Pop(this);
        }

        public List<Card> Pop(PilePosition p, int count)
        {
            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            if (IsEmpty)
                throw new EmptyPileException(this);

            return p.Pop(this, count);
        }

        public List<Card> PopAll()
        {
            var result = CardsInPile.ToList();
            CardsInPile.Clear();
            return result;
        }

        public void Shuffle()
        {
            CardsInPile = CardsInPile.Shuffle().ToList();
        }

        internal int GetIndexOf(Card card)
        {
            for (var i = 0; i < CardsInPile.Count; i++)
            {
                if (CardByRefEqualityComparer.Instance.Equals(card, CardsInPile[i]))
                    return i;
            }
            return -1;
        }

        public override string ToString()
        {
            if (IsEmpty)
                return "Empty pile";

            var builder = new StringBuilder($"Pile with {Size} cards, top is {PilePosition.Top.Peek(this)}, bottom is {PilePosition.Bottom.Peek(this)}. All cards: \n");
            foreach (var card in CardsInPile)
            {
                builder.Append(card);
                builder.AppendLine();
            }
            return builder.ToString();
        }

        #region IEnumerable

        public IEnumerator<Card> GetEnumerator()
        {
            return CardsInPile.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return CardsInPile.GetEnumerator();
        }

        #endregion IEnumerable

        #region IReadOnlyList

        int IReadOnlyCollection<Card>.Count => CardsInPile.Count;

        Card IReadOnlyList<Card>.this[int index] => CardsInPile[index];

        #endregion IReadOnlyList
    }
}