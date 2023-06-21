using System.Collections;
using System.Collections.Generic;


public class User
{
    public string username;
    public List<string> models;
    
    
    public User(string name){
        this.username = name;
        //this.models = score.ToString();
    }

    public void addModel(string model)
    {
        this.models.Add(model);
    }
//     // public void give_Score(int scr){
//     //     this.score = scr.ToString();
//     // }
}


