using UnityEngine;

namespace _00._Work.WorkSpace.Soso7194._01.Scripts.Interaction.Item
{
    public enum ItemType
    {
        Fuse,
        Silent,
        Bolt,
        Board
    }
    
    [CreateAssetMenu(fileName = "Item", menuName = "SO/Item", order = 0)]
    public class ItemSo : ScriptableObject
    {
        [SerializeField] private ItemType itemType;
        [SerializeField] private string itemName;
        [SerializeField] private Sprite itemSprite;
        [SerializeField] private int itemValue;
        [SerializeField] private string itemTooltip;
    }
}