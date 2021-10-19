using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;

[CreateAssetMenu(menuName = "Items/Item")]
public class Item : ScriptableObject
{
    [SerializeField] private string id;
    public string ID { get  => id; }
    public string ItemName;
    [Range(1, 999)]
    public int MaximumStacks = 1;
    public Sprite Icon;

    protected static readonly StringBuilder sb = new StringBuilder();


    private void OnValidate()
    {
        string path = AssetDatabase.GetAssetPath(this);
        id = AssetDatabase.AssetPathToGUID(path);
    }

    public virtual Item GetCopy()
    {
        return this;
    }

    public virtual void Destroy()
    {

    }

    public virtual string GetItemType()
    {
        return "";
    }
    public virtual string GetDescription()
    {
        return "";
    }
}
