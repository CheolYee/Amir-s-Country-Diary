using System;
using _00._Work.Resources._02._Codes.Utils;
using PBG_01_PUSE;
using UnityEngine;

namespace _00._Work.WorkSpace.Soso7194._01.Scripts
{
    public class Entrance : MonoBehaviour
    {
        [SerializeField] private InputSo inputSo;
        [SerializeField] private Sprite[] sprite;
        
        private SpriteRenderer _spriteRenderer;
        private Collider2D _collider2D;
        
        private bool _isOpened;
        
        private void OnEnable()
        {
            if (inputSo != null) inputSo.OnInteractionKeyPressed += End;
        }

        private void OnDisable()
        {
            if (inputSo != null) inputSo.OnInteractionKeyPressed -= End;
        }
        
        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _collider2D = GetComponent<Collider2D>();
            GeneratorManager.Instance.OnAllPowered += OpenEntrance;
            
            _collider2D.enabled = false;
            _spriteRenderer.sprite = sprite[0];
        }

        private void OpenEntrance()
        {
            _spriteRenderer.sprite = sprite[1];
            _collider2D.enabled = true;
            _collider2D.isTrigger = true;
            GeneratorManager.Instance.OnAllPowered -= OpenEntrance;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            _isOpened = true;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            _isOpened = false;
        }
        
        private void End()
        {
            if (_isOpened)
            {
                Clear.Instance.OnClear();
            }
        }
    }
}