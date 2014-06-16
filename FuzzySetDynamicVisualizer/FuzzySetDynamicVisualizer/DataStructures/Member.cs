using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuzzySetDynamicVisualizer.DataStructures
{
    public class Member
    {
        private string label;        
        private List<Set> sets;
        private int[] memberships;
        private int totalValue = 0;

        public Member(string label, List<Set> sets, int[] memberships)
        {
            this.label = label;
            this.memberships = memberships;
            this.sets = sets;

            int maxValue = 0;
            int maxIndex = 0;

            for(int i = 0; i < memberships.Length; i++){
                totalValue += memberships[i];
                if(memberships[i] >= maxValue){
                    maxValue = memberships[i];
                    maxIndex = i;
                }
            }

            Set maxSet = sets[maxIndex];
            maxSet.addMember(this);
        }

        /**
         * returns an int between 0 and 100
         */
        public float getMembershipAsPercent(Set set)
        {
            int setIndex = -1;
            for (int i = 0; i < sets.Count; i++)
            {
                if (sets[i].Equals(set))
                    setIndex = i;
            }

            float returnVal = 0;
            if(setIndex >= 0)
                returnVal = (float)memberships[setIndex] / (float)totalValue * 100.0f;
            return returnVal;
        }

        public string getLabel()
        {
            return label;
        }
    }
}
