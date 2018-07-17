namespace Multivac.Utilities.RollPlayingCompanion
{/*
    class IO
    {
        public void Save(System.IO.TextWriter textOut)
        {
            textOut.WriteLine(name);
            textOut.WriteLine(melee);
            textOut.WriteLine(ballistic);
            textOut.WriteLine(strength);
            textOut.WriteLine(toughness);
            textOut.WriteLine(agility);
            textOut.WriteLine(dexterity);
            textOut.WriteLine(perception);
            textOut.WriteLine(intelligence);
            textOut.WriteLine(fellowship);
            textOut.WriteLine(willpower);
            textOut.WriteLine(wounds);
            textOut.WriteLine(influence);
            textOut.WriteLine(focus);
            textOut.WriteLine(insanity);
            textOut.WriteLine(corruption);
            textOut.WriteLine(infamy);
        } // end save


        public static Character Load(System.IO.TextReader textIn)
        {
            Character result = new Character();

            try
            {
                string nameText = textIn.ReadLine();
                int melee = Int32.Parse(textIn.ReadLine());
                int ballistic = Int32.Parse(textIn.ReadLine());
                int strength = Int32.Parse(textIn.ReadLine());
                int toughness = Int32.Parse(textIn.ReadLine());
                int agility = Int32.Parse(textIn.ReadLine());
                int dexterity = Int32.Parse(textIn.ReadLine());
                int perception = Int32.Parse(textIn.ReadLine());
                int intelligence = Int32.Parse(textIn.ReadLine());
                int fellowship = Int32.Parse(textIn.ReadLine());
                int willpower = Int32.Parse(textIn.ReadLine());
                int wounds = Int32.Parse(textIn.ReadLine());
                int influence = Int32.Parse(textIn.ReadLine());
                int focus = Int32.Parse(textIn.ReadLine());
                int insanity = Int32.Parse(textIn.ReadLine());
                int corruption = Int32.Parse(textIn.ReadLine());
                int infamy = Int32.Parse(textIn.ReadLine());
            }
            catch
            {
                return null;
            }
            finally
            {
                if (textIn != null) textIn.Close();
            }
            return result;
        }

        public bool Save(string filename)
        {
            System.IO.TextWriter textOut = null;
            try
            {
                textOut = new System.IO.StreamWriter(filename);
                //Save(textOut);
            }
            catch
            {
                return false;
            }
            finally
            {
                if (textOut != null)
                {
                    textOut.Close();
                }
            }
            return true;
        } // end save

    }*/
}
