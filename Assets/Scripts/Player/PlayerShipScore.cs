using TMPro;
using UnityEngine;

public class PlayerShipScore : MonoBehaviour{
    [SerializeField] private GameObject m_vfxSmoke;

    [SerializeField] private GameObject m_vfxJet;

    [SerializeField] private TextMeshPro m_scoreText;

    [SerializeField] private TextMeshPro m_enemiesDestroyedText;

    [SerializeField] private TextMeshPro m_powerUpsUsedText;

    [SerializeField] private GameObject m_ship;

    [SerializeField] private GameObject m_crown;

    [SerializeField] private AnimationCurve m_moveCurve;

    private float m_curveDeltaTime;

    private void Update(){
        // Move the ship on an curve movement
        Vector2 currentPosition = m_ship.transform.localPosition;
        m_curveDeltaTime += Time.deltaTime;
        currentPosition.y = m_moveCurve.Evaluate(m_curveDeltaTime);
        m_ship.transform.localPosition = currentPosition;
    }

    public void SetShip(bool victory, int enemiesDestroyed, int powerUpsUsed, int score){
        // Set vfx depending on the scene we are loading 
        m_vfxSmoke.SetActive(!victory);
        m_vfxJet.SetActive(victory);

        // Set UI data base on the character data
        m_enemiesDestroyedText.text = enemiesDestroyed.ToString();
        m_powerUpsUsedText.text = powerUpsUsed.ToString();
        m_scoreText.text = score.ToString();
    }

    // Turn on the crown because I'm the best ship
    public void BestShip(){
        m_crown.SetActive(true);
    }
}