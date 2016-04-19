﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Newtonsoft.Json;

namespace SeaBattleLibrary
{
    public class Player
    {
        private BattleMap map;
        private EndPoint ip;
        private Turn turn;

        public BattleMap Map 
        { 
            get 
            {
                return map;
            } 
        }
        public EndPoint Ip
        {
            get
            {
                return ip;
            }
        }
        public Turn WhoseTurn
        {
            get
            {
                return turn;
            }
            internal set
            {
                turn = value;
            }
        }

        public Player() {}
        public Player(EndPoint ip)
        {
            this.ip = ip;
            map = new BattleMap();
        }
        public Player(EndPoint ip, BattleMap map)
        {
            this.ip = ip;
            this.map = map;
        }

        public enum Turn
        {
            YOUR = 1, //твой
            ENEMY = 2 //врага
        }
    }
}
