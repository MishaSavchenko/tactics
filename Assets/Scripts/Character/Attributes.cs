using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attributes 
{
    public Dictionary<string, CombatAttribute> combat_attributes; 
    public Dictionary<string, AbilityScore> ability_scores; 

    public Attributes()
    {
        combat_attributes = new Dictionary<string, CombatAttribute>(); 
        combat_attributes.Add("speed", new CombatAttribute(30, 30)); 
        combat_attributes.Add("hit_points", new CombatAttribute(30, 30)); 
        combat_attributes.Add("armor_class", new CombatAttribute(30, 30)); 


        ability_scores = new Dictionary<string, AbilityScore>(); 
        ability_scores.Add("strength", new AbilityScore(10));
        ability_scores.Add("dexterity", new AbilityScore(10));
        ability_scores.Add("constitution", new AbilityScore(10));
        ability_scores.Add("intelligence", new AbilityScore(10));
        ability_scores.Add("wisdom", new AbilityScore(10));
        ability_scores.Add("charisma", new AbilityScore(10));
    }

}

public class AbilityScore
{
    public int value; 
    public int modifier;
    public AbilityScore(int value_)
    {
        value = value_;
        modifier = (value_ - 10) % 2; 
    }

    public override string ToString()
    {
        return "Value: " + this.value + " | Modifier: " + this.modifier;
    }
}

public class CombatAttribute 
{  
    public double current; 
    public double max;
    public double temp;

    public CombatAttribute(double current_, double max_, double temp_=0.0)
    {
        current = current_; 
        max = max_; 
        temp = temp_; 
    }

    public static CombatAttribute operator- (CombatAttribute a, double b)
    {
        double new_current = a.current - b; 
        CombatAttribute ca = new CombatAttribute(new_current, a.max, a.temp);
        return ca;
    }


    public static CombatAttribute operator+ (CombatAttribute a, double b)
    {
        double new_current = a.current + b; 
        CombatAttribute ca = new CombatAttribute(new_current, a.max, a.temp);
        return ca;
    }

    public void Reset()
    {
        this.current = max;
    }



    public override string ToString()
    {
        return "Current: " + this.current + " | Max: " + this.max + " | Temp: " + this.temp;
    }

}
