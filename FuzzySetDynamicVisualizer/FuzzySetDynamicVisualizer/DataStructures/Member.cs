using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuzzySetDynamicVisualizer.DataStructures
{
    public class Member
    {
        private string label;
        private Dictionary<Set, int> setMembership;
        private int totalValue = 0;

        public Member(string label, Dictionary<Set, int> membershipDictionary)
        {
            this.label = label;
            this.setMembership = membershipDictionary;

            int maxValue = membershipDictionary.Values.Max();
            Set maxSet = null;

            IEnumerator<Set> keys = membershipDictionary.Keys.GetEnumerator();

            while (keys.MoveNext())
            {
                int membership = 0;
                if (membershipDictionary.TryGetValue(keys.Current, out membership))
                {
                    totalValue += membership;  //the total value is so we can do the membership as a percent of all memberships
                    if (membership == maxValue)
                        maxSet = keys.Current;
                }
            }

            if (maxSet != null)
                maxSet.addMember(this);
        }

        /**
         * returns an int between 0 and 100
         */
        public int getMembershipAsPercent(Set set)
        {
            int returnVal = 0;
            if (!setMembership.TryGetValue(set, out returnVal))
                returnVal = 0;
            else if(totalValue > 0)
                returnVal = (int)((float)returnVal / (float)totalValue * 100.0f);
            return returnVal;
        }

        public string getLabel()
        {
            return label;
        }
    }
}
