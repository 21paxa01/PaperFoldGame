using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace _Packages.PaperFoldAsset.Paper_Fold.Scripts
{
    [RequireComponent(typeof(Paper))]
    public class PossibleCombinationBuilder : MonoBehaviour
    {
        [SerializeField] private CombinationSequence[] _sequence;
        
        public bool Used => _sequence.Length != 0;

        private void OnValidate()
        {
            GetComponent<Paper>().Builder = this;
        }

        public bool IsCorrect(IEnumerable<Folding> combination)
        {
            var from = 0;
            foreach (CombinationSequence sequence in _sequence)
            {
                var researched = combination.Skip(from).Take(sequence.Length);
                from += sequence.Length;
                
                switch (sequence.Type)
                {
                    case MatchType.Sequentially:
                        if (MatchSequentially(sequence, researched) == false)
                            return false;
                        break;
                    case MatchType.AnyOrder:
                        if (MatchAnyOrder(sequence, researched) == false)
                            return false;
                        break;
                }
            }

            return true;
        }

        private bool MatchAnyOrder(CombinationSequence expected, IEnumerable<Folding> actual)
        {
            Assert.AreEqual(expected.Length, actual.Count());
            var foldings = new List<Folding>(expected.Foldings);
            foreach (var folding in expected.Foldings)
            {
                if (foldings.Contains(folding) == false)
                {
                    return false;
                }
                foldings.Remove(folding);
            }

            return true;
        }

        private bool MatchSequentially(CombinationSequence expected, IEnumerable<Folding> actual)
        {
            Assert.AreEqual(expected.Length, actual.Count());
            var expectedArray = expected.Foldings.ToArray();
            var actualArray = actual.ToArray();
            for (int i = 0; i < expected.Length; i++)
            {
                if (expectedArray[i] != actualArray[i])
                {
                    return false;
                }
            }

            return true;
        }
    }

    [Serializable]
    public class CombinationSequence
    {
        public MatchType Type;
        public Folding[] Foldings;

        public int Length => Foldings.Length;
    }

    public enum MatchType
    {
        AnyOrder,
        Sequentially 
    }
}