using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JetSystems
{

    public class SoundsButton : MonoBehaviour
    {
        [Header(" Settings ")]
        public Transform soundsParent;
        public static bool state;

        [Header(" UI ")]
        public Image image;
        public Sprite soundOnSprite;
        public Sprite soundOffSprite;
        public Color onColor;
        public Color offColor;

        private void Awake()
        {
            state = YanGamesSaveManager.GetSoundState() == 0 ? true : false;

            if (state)
                TurnSoundOn();
            else
                TurnSoundOff();
        }

        public void SwitchSoundState()
        {
            if (state)
                TurnSoundOff();
            else
                TurnSoundOn();

            SaveState();
        }

        void TurnSoundOn()
        {
            SetSounds(1);
            state = true;

            StopAllCoroutines();
            StartCoroutine("SwitchStateCoroutine");
            //image.sprite = soundOnSprite;

        }

        void TurnSoundOff()
        {
            SetSounds(0);
            state = false;

            //image.sprite = soundOffSprite;
            StopAllCoroutines();
            StartCoroutine("SwitchStateCoroutine");

        }



        void SaveState()
        {
            YanGamesSaveManager.SetSoundState(state ? 0 : 1);
        }

        void SetSounds(float volume)
        {
            if (soundsParent == null) return;

            for (int i = 0; i < soundsParent.childCount; i++)
            {
                soundsParent.GetChild(i).GetComponent<AudioSource>().volume = volume;
            }
        }

        IEnumerator SwitchStateCoroutine()
        {
            float t = 0;
            float duration = 0.3f;
            Sprite currentSprite;

            if (IsSoundOn())
                currentSprite = soundOnSprite;
            else
                currentSprite = soundOffSprite;

            image.color = new Color(image.color.r, image.color.g, image.color.b, 0f);
            image.sprite = currentSprite;

            while (t < duration + Time.deltaTime)
            {
                image.color = Color.Lerp(image.color, new Color(image.color.r, image.color.g, image.color.b, 1f), t / duration);
                t += Time.deltaTime;
                yield return null;
            }
        }

        public static bool IsSoundOn()
        {
            return state;
        }
    }
}