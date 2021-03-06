﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleCards.Engine
{
    public class Game
    {
        private readonly List<AI> _ais = new List<AI>();

        public Game(RankSet ranks, SuitSet suits, Rules rules, Parties parties)
        {
            RankSet = ranks;
            SuitSet = suits;
            Rules = rules;

            Table = new Table(Rules.ZoneFactory);
            Parties = parties;
            Round = new Round(Table, rules, parties);

            EnsurePartiesValid(parties);
        }

        public RankSet RankSet { get; }
        public SuitSet SuitSet { get; }
        public Rules Rules { get; }

        public Table Table { get; }
        public Parties Parties { get; }
        public Round Round { get; }

        public void Init()
        {
            var pack = Rules.MaterializeRequiredPack(SuitSet, RankSet);

            Table.GameField.PlacePile(pack);

            foreach (var item in Parties)
            {
                _ais.AddRange(item.Players.Select(x => new AI() { Player = x }));
            }
        }

        public void Move(Movement movement)
        {
            var player = Parties.Players.FirstOrDefault(x => x.Name == movement.PlayerName);
            if (player == null)
                throw new ArgumentException($"There is no player {movement.PlayerName} in game");

            if (movement.Action == Action.PlayCard)
            {
                var selectedCard = player.Hand.GetCard(movement.Card!);

                var trickPile = (TrickPile)Table.GameField.Pile;
                trickPile.Push(selectedCard, player);
            }
            else
            {
                throw new ArgumentException("Unknown action type: " + movement.Action);
            }

            Round.OnMove(movement);
        }

        private void EnsurePartiesValid(Parties parties)
        {
            var maxPlayers = Rules.GetMaxPlayers(SuitSet, RankSet);
            var playersCount = parties.Players.Count();
            if (playersCount > maxPlayers)
            {
                var ex = new ArgumentException("Too many players", nameof(parties));
                ex.Data.Add(nameof(playersCount), playersCount);
                ex.Data.Add(nameof(maxPlayers), maxPlayers);
                throw ex;
            }
        }
    }
}