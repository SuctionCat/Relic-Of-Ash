using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITepy 
{
    private string path;
    private string name;
    public string Path { get => path;  }
    public string Name { get => name;  }
    //<summary>
    /// 获得UI的信息
    /// </summary>
    ///<param name="ui_path">UI的路径</param>
    ///<param name="ui_name">UI的名称</param>
   public UITepy(string ui_path,string ui_name)
   {
       path = ui_path;
       name = ui_name;
   }
}
