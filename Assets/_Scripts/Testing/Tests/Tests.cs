using System;
using Eiko.YaSDK;
using Eiko.YaSDK.Data;
using JetSystems;
using ModestTree;
using UnityEngine;

namespace Testing.Tests
{
    public class Tests : MonoBehaviour
    {
        private void Start()
        {
            //arrange
            foreach (var level in new int[] {10, 20, 30, 40, 50, 60, 70, 80, 90, 100})
            {
                WhenCheckIsExpertLevel_AndLevelIsMultipleOf10_AssertThatLevelIsExpert(level);
            }
            
            foreach (var level in new int[] {1, 4, 5, 2, 22, 21})
            {
                WhenCheckIsExpertLevel_AndLevelIsNotMultipleOf10_AssertThatLevelIsNotExpert(level);
            }
        }

        private void WhenCheckIsExpertLevel_AndLevelIsMultipleOf10_AssertThatLevelIsExpert(int level)
        {
            //act
            var expert = LevelLangSwitcher.IsCurrentLevelExpert(level);
            //assert
            Assert.That(expert);
        }

        private void WhenCheckIsExpertLevel_AndLevelIsNotMultipleOf10_AssertThatLevelIsNotExpert(int level)
        {
            //act
            var isNotExpert = LevelLangSwitcher.IsCurrentLevelExpert(level) == false;
            //assert
            Assert.That(isNotExpert);
        }
    }
}