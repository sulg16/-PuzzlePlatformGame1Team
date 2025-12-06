using UnityEngine;

public enum ItemType { Simple, Consumable, Tool, key }

public abstract class ItemData : ScriptableObject
{
    [Header("Identity")]
    [SerializeField, HideInInspector] public string id;                 // for save
    public string displayName;
    public string description;
    public string displayNameEn;
    public string descriptionEn;
    public ItemType type = ItemType.Simple;
    public Sprite icon;
    public GameObject itemPrefab;
    public bool stackable = true;
    public int maxStack = 99;
    public bool getable = true;

    [ContextMenu("Generate New ID")]
    void GenerateId()
    {
        id = System.Guid.NewGuid().ToString("N");
    }

    protected virtual void OnValidate()
    {
        if (string.IsNullOrEmpty(id))
            GenerateId();
    }

    public string GetDisplayName()
    {
        switch (Localization.CurrentLanguage)
        {
            case Language.English:
                return string.IsNullOrEmpty(displayNameEn) ? displayName : displayNameEn;
            case Language.Korean:
            default:
                return displayName;
        }
    }

    public string GetDescription()
    {
        switch (Localization.CurrentLanguage)
        {
            case Language.English:
                return string.IsNullOrEmpty(descriptionEn) ? description : descriptionEn;
            case Language.Korean:
            default:
                return description;
        }
    }
}