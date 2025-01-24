using UnityEngine;
using System.Collections.Generic;
using GameModule.Core.Data;

namespace GameModule.Unity.Views
{
    public class ScoreboardView : MonoBehaviour
    {
        [SerializeField] private Transform scoreItemContainer;
        [SerializeField] private GameObject scoreItemPrefab;
        [SerializeField] private int maxDisplayCount = 10;

        private Dictionary<int, ScoreItemView> scoreItems = new();

        private void Start()
        {
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            EventManager.Instance.AddListener<ScoreUpdateEvent>(OnScoreUpdated);
            EventManager.Instance.AddListener<RankingChangeEvent>(OnRankingChanged);
        }

        private void OnScoreUpdated(ScoreUpdateEvent evt)
        {
            UpdatePlayerScore(evt.PlayerId, evt.NewScore);
        }

        private void OnRankingChanged(RankingChangeEvent evt)
        {
            UpdatePlayerRank(evt.PlayerId, evt.NewRank);
        }

        private void UpdatePlayerScore(int playerId, int score)
        {
            if (!scoreItems.TryGetValue(playerId, out var item))
            {
                item = CreateScoreItem(playerId);
            }
            item.UpdateScore(score);
        }

        private void UpdatePlayerRank(int playerId, int rank)
        {
            if (scoreItems.TryGetValue(playerId, out var item))
            {
                item.UpdateRank(rank);
            }
        }

        private ScoreItemView CreateScoreItem(int playerId)
        {
            var itemObj = Instantiate(scoreItemPrefab, scoreItemContainer);
            var item = itemObj.GetComponent<ScoreItemView>();
            scoreItems[playerId] = item;
            return item;
        }

        private void OnDestroy()
        {
            EventManager.Instance.RemoveListener<ScoreUpdateEvent>(OnScoreUpdated);
            EventManager.Instance.RemoveListener<RankingChangeEvent>(OnRankingChanged);
        }
    }
} 