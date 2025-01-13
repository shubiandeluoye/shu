using UnityEngine;

/// <summary>
/// Configures UI layer settings for different UI elements
/// </summary>
public static class UILayerConfig
{
    public const int HEALTH_DISPLAY_LAYER = 17;
    public const int BATTLE_SCORE_LAYER = 31;
    
    public static void SetUILayer(GameObject uiObject, UIElementType elementType)
    {
        switch (elementType)
        {
            case UIElementType.HealthDisplay:
                uiObject.layer = HEALTH_DISPLAY_LAYER;
                break;
            case UIElementType.BattleScore:
                uiObject.layer = BATTLE_SCORE_LAYER;
                break;
        }
    }
}

public enum UIElementType
{
    HealthDisplay,
    BattleScore,
    GameState
}
