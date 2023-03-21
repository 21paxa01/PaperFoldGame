using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace _Packages.PaperFoldAsset.Paper_Fold.Scripts.RegexCombinationBuilder
{
    [RequireComponent(typeof(Paper))]
    public class RegexCombinationAnalyzer : MonoBehaviour
    {
        [SerializeField] private string _pattern;
        private readonly char[] _alphabet = "abcdefghij".ToCharArray();
        
        private void OnValidate()
        {
            void AssignMarkerAndRandomIdentifier(Transform folding, int alphabetIndex)
            {
                if (folding.TryGetComponent<FoldingMarker>(out _) == false)
                {
                    var marker = folding.gameObject.AddComponent<FoldingMarker>();
                    marker.identifier = _alphabet[alphabetIndex];
                    folding.name = $"{folding.name} ({marker.identifier})";
                    _pattern += marker.identifier;
                }
            }

            void AttachMarkersToFoldings(Paper relatedPaper)
            {
                var alphabetIndex = 0;
                
                foreach (var folding in relatedPaper.foldingsParent.Cast<Transform>())
                {
                    AssignMarkerAndRandomIdentifier(folding, alphabetIndex);
                    alphabetIndex++;
                }
            }

            var paper = GetComponent<Paper>();
            AttachMarkersToFoldings(paper);
            paper.RegexAnalyzer = this;
        }

        public bool IsCorrect(IEnumerable<Folding> combination)
        {
            var translated = combination.Select(x => x.GetComponent<FoldingMarker>())
                .Select(x => x.identifier)
                .ToArray();
            return Regex.IsMatch(new string(translated), _pattern);
        }
    }
}