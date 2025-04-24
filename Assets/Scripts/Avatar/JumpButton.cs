using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TequilaSunrise.Avatar
{
    public class JumpButton : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private AvatarController avatarController;
        
        public void OnPointerDown(PointerEventData eventData)
        {
            if (avatarController != null)
            {
                avatarController.Jump();
            }
        }
    }
} 