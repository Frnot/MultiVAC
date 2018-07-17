using System;

namespace Multivac.RollPlayingCompanion.RobsRuleset
{
    class RobsRulesetCharacter //: ICharacter
    {
        public RobsRulesetCharacter(String name, ulong characterId)
        {
            this.name = name;
            this.characterId = characterId;
        }

        public string name { get; private set; }
        public ulong characterId { get; private set; }

        public int melee { get; private set; }
        public int ballistic { get; private set; }
        public int strength { get; private set; }
        public int toughness { get; private set; }
        public int agility { get; private set; }
        public int dexterity { get; private set; }
        public int perception { get; private set; }
        public int intelligence { get; private set; }
        public int fellowship { get; private set; }
        public int willpower { get; private set; }

        public int wounds { get; private set; }
        public int influence { get; private set; }
        public int focus { get; private set; }

        //these 3 start at 0
        public int insanity { get; private set; }
        public int corruption { get; private set; }
        public int infamy { get; private set; }

        public void GenerateCharacter()
        {
            melee = Rolling.R5d6DL();
            ballistic = Rolling.R5d6DL();
            strength = Rolling.R5d6DL();
            toughness = Rolling.R5d6DL();
            agility = Rolling.R5d6DL();
            dexterity = Rolling.R5d6DL();
            perception = Rolling.R5d6DL();
            intelligence = Rolling.R5d6DL();
            fellowship = Rolling.R5d6DL();
            willpower = Rolling.R5d6DL();
            wounds = Rolling.R3d6() + (toughness / 10);
            influence = Rolling.R3d6() + (fellowship / 10);
            focus = Rolling.R1d6() + (willpower / 10) + (intelligence / 10);
            insanity = 0;
            corruption = 0;
            infamy = 0;
        }

        public string PrintStats()
        {

            return "```\n" +
                $"{"melee:".PadRight(15)} {melee}\n" +
                $"{"ballistic:".PadRight(15)} {ballistic}\n" +
                $"{"strength:".PadRight(15)} {strength}\n" +
                $"{"toughness:".PadRight(15)} {toughness}\n" +
                $"{"agility:".PadRight(15)} {agility}\n" +
                $"{"dexterity:".PadRight(15)} {dexterity}\n" +
                $"{"perception:".PadRight(15)} {perception}\n" +
                $"{"intelligence:".PadRight(15)} {intelligence}\n" +
                $"{"fellowship:".PadRight(15)} {fellowship}\n" +
                $"{"willpower:".PadRight(15)} {willpower}\n" +
                $"{"wounds:".PadRight(15)} {wounds}\n" +
                $"{"influence:".PadRight(15)} {influence}\n" +
                $"{"focus:".PadRight(15)} {focus}\n" +
                $"{"insanity:".PadRight(15)} {insanity}\n" +
                $"{"corruption:".PadRight(15)} {corruption}\n" +
                $"{"infamy:".PadRight(15)} {infamy}\n" +
                "```";
        }

        public void setName(string name) { this.name = name; }
        public void setMelee(int melee) { this.melee = melee; }
        public void setBallistic(int ballistic) { this.ballistic = ballistic; }
        public void setStrength(int strength) { this.strength = strength; }
        public void setToughness(int toughness) { this.toughness = toughness; }
        public void setAgility(int agility) { this.agility = agility; }
        public void setDexterity(int dexterity) { this.dexterity = dexterity; }
        public void setPerception(int perception) { this.perception = perception; }
        public void setIntelligence(int intelligence) { this.intelligence = intelligence; }
        public void setFellowship(int fellowship) { this.fellowship = fellowship; }
        public void setWillpower(int willpower) { this.willpower = willpower; }

        public void setWounds(int wounds) { this.wounds = wounds; }
        public void setInfluence(int influence) { this.influence = influence; }
        public void setFocus(int focus) { this.focus = focus; }

        public void setInsanity(int insanity) { this.insanity = insanity; }
        public void setCorruption(int corruption) { this.corruption = corruption; }
        public void setInfamy(int infamy) { this.infamy = infamy; }

        
    } // end Character
}
