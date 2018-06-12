﻿using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;



namespace Controllers
{

    public class BattleController
    {
        private Battle batt;
        private PlayerController playc;
        private IAController iac;

        public BattleController()
        {
            playc = new PlayerController();
            iac = new IAController();
            batt = new Battle();
            batt.Turn_play = 0;
            //initial_turn();
            
        }

        public int printturno()
        {
            return batt.Turn_play;
        }

        public void turn()
        {
            //1 red
            //2 blue
            int atual_play = batt.Turn_play;
            if (atual_play == 1)
            {
                batt.Turn_play = 2;
            }
            else if (atual_play == 2)
            {
                batt.Turn_play = 1;
            }
        }


        public bool initial_turn()
        {
            //1 red
            //2 blue
            Random random = new Random();
            batt.Turn_play = random.Next(1,3);

            if (batt.Turn_play == 2)
            {
                return true;
            }

            return false;
        }

        public int pass_turn(object atual_turn)
        {
            int atual_turn_int = conversion_object_toint(atual_turn);

            if (atual_turn_int != batt.Turn)
            {
                //erro
            }

            atual_turn_int = batt.Turn += 1;

            turn();
            return atual_turn_int;
        }


        public int attack_red(object life)
        {
            if (batt.Turn_play == 1)
            {
                int life_int = conversion_object_toint(life);
                return playc.attack(life_int);
            }
            else
            {
                int life_int = conversion_object_toint(life);
                return life_int;
            }
            
        }

        public int attack_blue(object life)
        {
            int life_int = conversion_object_toint(life);

            return iac.attack(life_int);
        }

        public bool skill_select(int skill)
        {
            if (skill == 12)
            {

                return true;
            }
            return false;
        }

        public bool dead_confirmation(object life)
        {
            int life_int = conversion_object_toint(life);
            if (life_int <= 0)
            {
                return true;
            }

            return false;
        }

        public int convert_name_int(string name)
        {

            name = String.Join("", System.Text.RegularExpressions.Regex.Split(name, @"[^\d]"));
            return int.Parse(name);
        }

        public int conversion_object_toint(object obj)
        {
            int obj_int = int.Parse(obj.ToString());
            return obj_int;
        }
    }

}
