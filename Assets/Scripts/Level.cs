using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Enabling Level creation from Editor Menu
[CreateAssetMenu(fileName = "New Level", menuName = "Levels")] 

public class Level : ScriptableObject
{
    //Each Level Scriptableobject stores level data such as enemies, time duration, phases, etc
    //Serializing main data for editor configuration of level
    [SerializeField] public string levelType; 
    [SerializeField] public float levelTime;
    [SerializeField] public int levelPhases, maxObstacles, extraPhaseEnemies, extraPhaseTime;
    [SerializeField] public string obstacleName, bossName;
    //Dictionary for level enemies and count for transition control
    public List<Dictionary<string,int>> levelEnemies = new List<Dictionary<string, int>>();
    public List<int> enemyCount = new List<int>();

    public void SetInitialEnemies()
    {        
        //Enemy count is a list so it can store numbers of enemies on each phase
        enemyCount = new List<int>();
        //Level enemies is a List of Dicts so it can store type and quantities of enemies per phase
        levelEnemies = new List<Dictionary<string, int>>();
        
        for(int i = 0; i < levelPhases; i++) {
            enemyCount.Add(0);
            levelEnemies.Add(new Dictionary<string, int>());
            //Adding each type of enemy to enemies dictionary
            levelEnemies[i].Add("Crabcatcher", 0);
            levelEnemies[i].Add("ReptAgent", 0);
            levelEnemies[i].Add("Reptbaby", 0);
            levelEnemies[i].Add("Flamey", 0);
            levelEnemies[i].Add("Reptlizard", 0);
            //levelEnemies[i].Add("Icey", 0);
            //Establishing enemy quantities. Level type is intended as a way of adding more levels with different enemy configurations
            if(levelType == "beach") {
                levelEnemies[i]["Crabcatcher"] = 4;
                levelEnemies[i]["ReptAgent"] = 0;
                levelEnemies[i]["Reptbaby"] = 0;
                levelEnemies[i]["Flamey"] = 0;
                levelEnemies[i]["Reptlizard"] = 0;
                //levelEnemies[i]["Icey"] = 1;
            } 
            //Counting enemies
            foreach(KeyValuePair<string,int> enemy in levelEnemies[i]){
                enemyCount[i] += enemy.Value;
            }
        }
    }   
}