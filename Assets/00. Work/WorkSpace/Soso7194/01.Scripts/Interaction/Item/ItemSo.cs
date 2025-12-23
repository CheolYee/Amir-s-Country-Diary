using System;
using UnityEngine;

namespace _00._Work.WorkSpace.Soso7194._01.Scripts.Interaction.Item
{
    public enum ItemType
    {
        Fuse,
        Wire,
        Bolt,
        Board
    }
    
    [CreateAssetMenu(fileName = "Item", menuName = "SO/Item", order = 0)]
    public class ItemSo : ScriptableObject
    {
        //실행 했을 때 쓴 슬롯에 있는 아이템이 그냥 삭제만되는 것이 아니라 쓴 아이템의 Prefab을 복제해서 화면에 나타나게 해야함, Inventory.cs와 연계
        public ItemType itemType;
        public string itemName;
        public Sprite itemSprite;
        public int itemValue;
        public string itemTooltip;
        public GameObject itemPrefab;
    }
}