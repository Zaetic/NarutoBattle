﻿using Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ViewWpf
{
    /// <summary>
    /// Interaction logic for BattleWindow.xaml
    /// </summary>
    public partial class BattleWindow : Window
    {
        int skill_select;
        string skillNumber = "";
        string attack_char = "0";
        BattleController bat = new BattleController();
        private Timer timer;
        private string type_skill;
        private string account;

        public BattleWindow(string character1, string character2, string character3, string account)
        {
            InitializeComponent();

            this.account = account;
            if (bat.initial_turn()== true) //ia
            {
                ia_play();
            }
            else if ( bat.initial_turn() == false) //player
            {
                Timer_Function();
            }
            
            //Load character
            Character1_red.Source = Load_image(character1);
            Character2_red.Source = Load_image(character2);
            Character3_red.Source = Load_image(character3);
            Character1_red.Tag = "Characters/" + character1 + "/" + character1 + "_default.png";
            Character2_red.Tag = "Characters/" + character2 + "/" + character2 + "_default.png";
            Character3_red.Tag = "Characters/" + character3 + "/" + character3 + "_default.png";

            //skills
            Character1_red_skill1.Source = Load_skill(character1, 1);
            Character1_red_skill2.Source = Load_skill(character1, 2);
            Character1_red_skill3.Source = Load_skill(character1, 3);
                                           
            Character2_red_skill1.Source = Load_skill(character2, 1);
            Character2_red_skill2.Source = Load_skill(character2, 2);
            Character2_red_skill3.Source = Load_skill(character2, 3);
                                           
            Character3_red_skill1.Source = Load_skill(character3, 1);
            Character3_red_skill2.Source = Load_skill(character3, 2);
            Character3_red_skill3.Source = Load_skill(character3, 3);

            bat.Character1_red = character1;
            bat.Character2_red = character2;
            bat.Character3_red = character3;

            //Chakra
            Load_Chakras();
        }

        //=====================Loaders=====================


        private ImageSource Load_image(string character)
        {
            return new BitmapImage(new Uri("Characters/" + character + "/" + character + "_default.png", UriKind.Relative));
        }

        private ImageSource Load_skill(string character, int number)
        {
            return new BitmapImage(new Uri("Characters/" + character + "/" + character + "_skill" + number + "_default.png", UriKind.Relative));
        }

        //load red chakra
        private void Load_Chakras()
        {
            List<int> Chakras = new List<int>();
            Chakras = bat.Return_Chakras(1);

            TaijutsuNumber.Content = Chakras[0];
            BloodlineNumber.Content = Chakras[1];
            NinjutsuNumber.Content = Chakras[2];
            GenjutsuNumber.Content = Chakras[3];
        }

        //===================== Turno =====================

        //Pass turn button
        private void Pass_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            pass_turn();
            ia_play();
        }

        //Alterar numeração da label de turno
        private void pass_turn()
        {
            Turn.Content = bat.pass_turn(Turn.Content);
            skill_select = 0;
            bat.ChakraRed.turnChakra();
            Load_Chakras();
            Unlock_Character();
            Receive_SkillsDH();
            pb.Value = 0;
            try
            {
                timer.Start();
            }
            catch
            {
                Timer_Function();
            }
            Change_Life_GlobalColor();
        }

        private void Timer_Function()
        {
            timer = new Timer(1000);
            timer.Elapsed += new ElapsedEventHandler(Timer_Elapsed);
            timer.Start();
        }

        void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (Action)(() =>
            {
                if (pb.Value < 30)
                {
                    pb.Value += 1;
                }
                else
                {
                    timer.Stop();
                    pass_turn();
                    ia_play();
                }
            }));
        }

        //===================== Attack ====================

        //Receive number skill
        private void ReceiveSkillNumber(object sender, MouseButtonEventArgs e)
        {
            Image image = (Image)sender;

            //Numbers
            skillNumber = bat.convert_name_int(image.Name).ToString();
            string skill_second = (skillNumber.Last()).ToString();
            int char_select = int.Parse(skillNumber.Remove(skillNumber.Length - 1));
            skill_select = int.Parse(skill_second);

            //skills chakra
            List<string> Skills = new List<string>();
            Skills = bat.Skills(skill_second, char_select);
            bool haveChakra = bat.Have_Chakra(Skills);

            if(attack_char == "" || attack_char == "0" || attack_char == null)
            {
                if (haveChakra == true)
                {
                    //retira o chakra
                    bat.Withdraw_Chakra(Skills);

                    //Change chakras content
                    Load_Chakras();

                    //pass character to attack char
                    attack_char = bat.Attack_Char(char_select);

                    //search skill type
                    type_skill = bat.Skill_Type(char_select, skill_select);


                    var info = Application.GetResourceStream(new Uri("Cursor/Skill_Select.ani", UriKind.Relative));
                    var cursor = new Cursor(info.Stream);

                    principal.Cursor = cursor;                       

                }
            }
           
   
        }

        private void Unlock_Character()
        {
            var images = principal.Children.OfType<Image>();
            foreach (Image element in images)
            {
                if (element.Name != null && element.Name != "" && element.Name.Length > 15 && element.Tag.ToString() != "dead" )
                {
                    try
                    {
                        if (element.IsEnabled == false)
                        {

                            element.Opacity = 1;
                            element.IsEnabled = true;
                        }
                    }
                    catch
                    { }
                }
            }
        }

        private void Receive_SkillsDH()
        {
            var labels = SkillAdded.Children.OfType<Image>();
            foreach (Image element in labels)
            {
                string skillP1 = "0";
                try
                {
                    int skill = bat.convert_name_int(element.Name);

                    skillP1 = skill.ToString();
                    skillP1 = skillP1.Substring(0, 1);
                }
                catch
                {}

                if ((skillP1 == "1" || skillP1 == "2" || skillP1 == "3") && element.Name.Substring(0,10) == "SkillAdded" && element.Tag.ToString() != "1")
                {
                    string damage = element.Tag.ToString();
                    damage = damage.Substring(1); // pega o d25 d20 etc
                    string typeSkill = element.Tag.ToString().Substring(0, 1); 
                    //passa pra função de dmg ou heal

                    if (skillP1 == "1")
                    {
                        if (typeSkill == "d")
                        {
                            int vida = int.Parse(Character1_blue_life.Content.ToString()) - int.Parse(damage);
                            Character1_blue_life.Content = vida.ToString();
                            Dead(Character1_blue, Character1_blue_life);

                        }
                        else if (typeSkill == "h")
                        {
                            int vida = int.Parse(Character1_red_life.Content.ToString()) + int.Parse(damage);
                            if (vida > 100)
                                vida = 100;
                            Character1_red_life.Content = vida.ToString();
                        }
                    }
                    else if (skillP1 == "2")
                    {
                        if (typeSkill == "d")
                        {
                            int vida = int.Parse(Character2_blue_life.Content.ToString()) - int.Parse(damage);
                            Character2_blue_life.Content = vida.ToString();
                            Dead(Character2_blue, Character2_blue_life);

                        }
                        else if (typeSkill == "h")
                        {
                            int vida = int.Parse(Character2_red_life.Content.ToString()) + int.Parse(damage);
                            if (vida > 100)
                                vida = 100;
                            Character2_red_life.Content = vida.ToString();
                        }
                    }
                    else if (skillP1 == "3")
                    {
                        if (typeSkill == "d")
                        {
                            int vida = int.Parse(Character3_blue_life.Content.ToString()) - int.Parse(damage);
                            Character3_blue_life.Content = vida.ToString();
                            Dead(Character3_blue, Character3_blue_life);

                        }
                        else if (typeSkill == "h")
                        {

                            int vida = int.Parse(Character3_red_life.Content.ToString()) + int.Parse(damage);
                            if (vida > 100)
                                vida = 100;
                            Character3_red_life.Content = vida.ToString();
                        }
                    }

                    element.Tag = "1";
                    element.Source = new BitmapImage(new Uri("Others/invalid_default.png", UriKind.Relative));
                }

            }
        }

        private void Block_Character(int char_select)
        {
            string charn = "0";

            int skills = 1;
            charn = "Character"+ char_select + "_red_skill" + skills;
            

            var images = principal.Children.OfType<Image>();
            foreach (Image element in images)
            {
                if (element.Name != null && element.Name != "" && element.Name.Length > 14)
                {
                    try
                    {
                        if (element.Name.Substring(0, 21) == charn)
                        {
                            
                            element.Opacity = 0.5;
                            element.IsEnabled = false;
                            skills += 1;
                            charn = "Character" + char_select + "_red_skill" + skills;
                        }
                    }
                    catch
                    {}
                } 
            }
        }

        private void NewSelect_Enemy(object sender, MouseButtonEventArgs e)
        {
            if (attack_char != "" && type_skill == "attack")
            {
                string skill_second = (skillNumber.Last()).ToString();
                int char_select = int.Parse(skillNumber.Remove(skillNumber.Length - 1));

                Image image = (Image)sender;
                int characterNumber = bat.attack_choose(skill_select, Character1_blue_life.Content, Character2_blue_life.Content, Character3_blue_life.Content, image.Name);
                //List<Label> labels = new List<Label>();
                var labels = SkillAdded.Children.OfType<Image>();
                string ado = "";

                if (characterNumber == 1)
                    ado = "SkillAdded1_blue";
                if (characterNumber == 2)
                    ado = "SkillAdded2_blue";
                if (characterNumber == 3)
                    ado = "SkillAdded3_blue";

                foreach (Image element in labels)
                {
                    // element.Name.Substring(0, 11) == "SkillAdded1"


                        if (element.Tag != null && element.Name != null)
                        {
                            if (element.Tag.ToString() == "1" && element.Name.Substring(0, 16) == ado)
                            {
                                if (element.Tag.ToString().Substring(0, 1) != "d")
                                {
                                    element.Source = new BitmapImage(new Uri("Characters/" + attack_char + "/" + attack_char + "_skill" + skill_second + "_default.png", UriKind.Relative));
                                    //Tag is damage >

                                    element.Tag = "d" + bat.damage_skill(skill_second, attack_char);
                                }

                                Block_Character(char_select);
                                break;
                            }
                        }

                }

                attack_char = "";
                var info = Application.GetResourceStream(new Uri("Cursor/Normal_Select.cur", UriKind.Relative));
                var cursor = new Cursor(info.Stream);

                principal.Cursor = cursor;
            }
        }

        private void NewSelect_Friend(object sender, MouseButtonEventArgs e)
        {
            if (attack_char != "" && type_skill == "heal")
            {
                string skill_second = (skillNumber.Last()).ToString();
                int char_select = int.Parse(skillNumber.Remove(skillNumber.Length - 1));

                Image image = (Image)sender;
                int characterNumber = bat.attack_choose(skill_select, Character1_red_life.Content, Character2_red_life.Content, Character3_red_life.Content, image.Name);
                //List<Label> labels = new List<Label>();
                var labels = SkillAdded.Children.OfType<Image>();
                string ado = "";

                if (characterNumber == 1)
                    ado = "SkillAdded1_red";
                if (characterNumber == 2)
                    ado = "SkillAdded2_red";
                if (characterNumber == 3)
                    ado = "SkillAdded3_red";

                foreach (Image element in labels)
                {
                    if (element.Tag != null && element.Name != null)
                    {
                        if (element.Tag.ToString() == "1" && element.Name.Substring(0, 15) == ado)
                        {
                            if (element.Tag.ToString().Substring(0, 1) != "h")
                            {
                                element.Source = new BitmapImage(new Uri("Characters/" + attack_char + "/" + attack_char + "_skill" + skill_second + "_default.png", UriKind.Relative));
                                //Tag is damage >

                                element.Tag = "h" + bat.heal_skill(skill_second, attack_char);
                            }

                            Block_Character(char_select);
                            break;
                        }
                    }

                }

                attack_char = "";
                var info = Application.GetResourceStream(new Uri("Cursor/Normal_Select.cur", UriKind.Relative));
                var cursor = new Cursor(info.Stream);

                principal.Cursor = cursor;
            }
        }

        private void teamDead()
        {
            int dead = bat.confirmation_teamDead(Character1_blue_life.Content, Character2_blue_life.Content, Character3_blue_life.Content,
                                                 Character1_red_life.Content, Character2_red_life.Content, Character3_red_life.Content);
            string result = "";
            if (dead == 2)
            {
                result = "Winner";
                timer.Close();
                Close();
                bat.WL(account, result);
                MessageBox.Show(result);
            }
            else if (dead == 1)
            {
                result = "Loser";
                timer.Close();
                Close();
                bat.WL(account, result);
                MessageBox.Show(result);
            }

        }

        private void Dead(object nome_e_cor, object life)
        {  
            Image image = (Image)nome_e_cor;
            Label label = (Label)life;

            bool confimation = bat.dead_confirmation(label.Content);

            //12 - 14
            if (confimation == true)
            {
                if (label.Name.Substring(11) == "red_life")
                {
                    if (bat.convert_name_int(label.Name) == 1)
                    {
                        Character1_red_skill1.Source = new BitmapImage(new Uri("Others/dead_default.png", UriKind.Relative));
                        Character1_red_skill2.Source = new BitmapImage(new Uri("Others/dead_default.png", UriKind.Relative));
                        Character1_red_skill3.Source = new BitmapImage(new Uri("Others/dead_default.png", UriKind.Relative));
                        Character1_red_skill1.IsEnabled = false;
                        Character1_red_skill2.IsEnabled = false;
                        Character1_red_skill3.IsEnabled = false;
                        Character1_red_skill1.Tag = "dead";
                        Character1_red_skill2.Tag = "dead";
                        Character1_red_skill3.Tag = "dead";
                    }
                    else if (bat.convert_name_int(label.Name) == 2)
                    {
                        Character2_red_skill1.Source = new BitmapImage(new Uri("Others/dead_default.png", UriKind.Relative));
                        Character2_red_skill2.Source = new BitmapImage(new Uri("Others/dead_default.png", UriKind.Relative));
                        Character2_red_skill3.Source = new BitmapImage(new Uri("Others/dead_default.png", UriKind.Relative));
                        Character2_red_skill1.IsEnabled = false;
                        Character2_red_skill2.IsEnabled = false;
                        Character2_red_skill3.IsEnabled = false;
                        Character2_red_skill1.Tag = "dead";
                        Character2_red_skill2.Tag = "dead";
                        Character2_red_skill3.Tag = "dead";
                    }
                    else if (bat.convert_name_int(label.Name) == 3)
                    {
                        Character3_red_skill1.Source = new BitmapImage(new Uri("Others/dead_default.png", UriKind.Relative));
                        Character3_red_skill2.Source = new BitmapImage(new Uri("Others/dead_default.png", UriKind.Relative));
                        Character3_red_skill3.Source = new BitmapImage(new Uri("Others/dead_default.png", UriKind.Relative));
                        Character3_red_skill1.IsEnabled = false;
                        Character3_red_skill2.IsEnabled = false;
                        Character3_red_skill3.IsEnabled = false;
                        Character3_red_skill1.Tag = "dead";
                        Character3_red_skill2.Tag = "dead";
                        Character3_red_skill3.Tag = "dead";
                    }
                }
                
                 

                image.Source = new BitmapImage(new Uri("Others/dead_default.png", UriKind.Relative));
                image.Tag = "Others/dead_default.png";
                image.IsEnabled = false;
                label.Content = "0";
                
            }
        }

        //===================== Others ====================

        private void Change_Life_GlobalColor()
        {
            Change_Life_Color(Character1_red_life);
            Change_Life_Color(Character2_red_life);
            Change_Life_Color(Character3_red_life);
            Change_Life_Color(Character1_blue_life);
            Change_Life_Color(Character2_blue_life);
            Change_Life_Color(Character3_blue_life);
        }

        private void Change_Life_Color(Label label)
        {
            if (int.Parse(label.Content.ToString()) < 50)
                label.Background = Brushes.Orange;
            if (int.Parse(label.Content.ToString()) < 20)
                label.Background = Brushes.Red;
        }

        private void Generic_mouseEnter(object sender, MouseEventArgs e)
        {
            Image image = (Image)sender;

            string source = image.Tag.ToString();
            source = source.Remove(source.Length - 4);
            

            //auth RED
            string name = image.Name;
            name = name.Substring(name.Length - 3);

            if (skill_select != 0 && attack_char != "" && image.Tag.ToString() != "Others/dead_default.png")
            {
                if (type_skill == "heal" && name == "red")
                {
                    image.Source = new BitmapImage(new Uri(@source + "_select.png", UriKind.Relative));
                }
                else if (type_skill == "attack" && name != "red")
                {
                    image.Source = new BitmapImage(new Uri(@source + "_select.png", UriKind.Relative));
                }
            }

        }

        private void Generic_mouseLeave(object sender, MouseEventArgs e)
        {
            Image image = (Image)sender;

            string source = image.Tag.ToString();
            source = source.Remove(source.Length - 4);

            //auth RED
            string name = image.Name;
            name = name.Substring(name.Length - 3);

            if (skill_select != 0)
            {
                if (type_skill == "heal" && name == "red")
                {
                    image.Source = new BitmapImage(new Uri(@source + ".png", UriKind.Relative));
                }
                else if (type_skill == "attack" && name != "red")
                {
                    image.Source = new BitmapImage(new Uri(@source + ".png", UriKind.Relative));
                }
            }
        }


        private void ShowSkill_MouseEnter(object sender, MouseEventArgs e)
        {
            //change image
            Image image = (Image)sender;
            panelSkillImage.Source = image.Source;

            //change chakras number
            string skillNumber = bat.convert_name_int(image.Name).ToString();
            string skill_second = (skillNumber.Last()).ToString();
            int char_select = int.Parse(skillNumber.Remove(skillNumber.Length - 1));
            //skill_select = int.Parse(skillNumber.Substring(0, 1));

            List<string> skills = new List<string>();
            skills = bat.Skills(skill_second, char_select);

            TaijutsuNumber_Panel.Content = skills[0];
            BloodlineNumber_Panel.Content = skills[1];
            NinjutsuNumber_Panel.Content = skills[2];
            GenjutsuNumber_Panel.Content = skills[3];

            string charshow = "";
            if (char_select == 1)
                charshow = bat.Character1_red;
            else if (char_select == 2)
                charshow = bat.Character2_red;
            else if (char_select == 3)
                charshow = bat.Character3_red;

            DamageNumber_Panel.Content = bat.damage_skill(skill_second, charshow);
            HealNumber_Panel.Content = bat.heal_skill(skill_second, charshow);
        }

        private void Surrender(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            string result = "Loser";
            timer.Close();
            Close();
            bat.WL(account, result);
            MessageBox.Show(result);
        }

        //===================== IA ====================

        //Jogada da IA
        private void ia_play()
        {
            //MessageBox.Show("" + characterNumber);
            int playRandom = 0;

            do
            {

                Random rand = new Random();
                int characterNumber = rand.Next(1, 4);
                if (characterNumber == 1 && Character1_red_life.Content.ToString() != "0")
                {

                        Character1_red_life.Content = bat.attack_blue(Character1_red_life.Content);
                        Dead(Character1_red, Character1_red_life);
                        teamDead();
                        playRandom = 1;
 

                }
                else if (characterNumber == 2 && Character2_red_life.Content.ToString() != "0")
                {

                        Character2_red_life.Content = bat.attack_blue(Character2_red_life.Content);
                        Dead(Character2_red, Character2_red_life);
                        teamDead();
                        playRandom = 1;


                }
                else if (characterNumber == 3 && Character3_red_life.Content.ToString() != "0")
                {

                        Character3_red_life.Content = bat.attack_blue(Character3_red_life.Content);
                        Dead(Character3_red, Character3_red_life);
                        teamDead();
                        playRandom = 1;
                    

                }


            } while (playRandom == 0);
            pass_turn();
        }

        //===================== Sound ====================

        private void VolMore_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
            if (Volume.Content.ToString() != "100")
            {
                MusicPlayer.Volume = MusicPlayer.Volume + 0.1;
                Volume.Content = bat.conversion_object_toint(Volume.Content) + 10;
            }
               
        }

        private void VolLess_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
            if (Volume.Content.ToString() != "0")
            {
                MusicPlayer.Volume = MusicPlayer.Volume - 0.1;
                Volume.Content = bat.conversion_object_toint(Volume.Content) - 10;
            }
                
            
        }

    }
}
