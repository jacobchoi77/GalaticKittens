using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

/*
    Script that controls the boss health ui
    set by boss controller when spawn
*/

public class BossUI : NetworkBehaviour{
    [SerializeField] private Slider m_healthSlider;
    [SerializeField] private Image m_healthImage;
    [SerializeField] private HealthColor m_healthColor;

    private int maxHealth;

    public void SetHealth(int health){
        if (!IsServer) return;
        maxHealth = health;
        m_healthImage.color = m_healthColor.normalColor;
        gameObject.SetActive(true);
        SetHealthClientRpc(health);
    }

    [ClientRpc]
    private void SetHealthClientRpc(int health){
        if (IsServer)
            return;

        maxHealth = health;
        m_healthImage.color = m_healthColor.normalColor;
        gameObject.SetActive(true);
    }

    public void UpdateUI(int currentHealth){
        if (!IsServer)
            return;

        var convertedHealth = (float)currentHealth / maxHealth;
        m_healthSlider.value = convertedHealth;
        m_healthImage.color = m_healthColor.GetHealthColor(convertedHealth);

        UpdateUIClientRpc(convertedHealth);
    }

    [ClientRpc]
    private void UpdateUIClientRpc(float currentHealth){
        if (IsServer)
            return;

        m_healthSlider.value = currentHealth;
        m_healthImage.color = m_healthColor.GetHealthColor(currentHealth);
    }

    override public void OnNetworkSpawn(){
        gameObject.SetActive(false);
    }
}