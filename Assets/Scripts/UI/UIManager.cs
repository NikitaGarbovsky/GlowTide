using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [SerializeField] public List<GameObject> m_HUDUI; // Amount of available slugs to throw
    private GameObject m_player;
    private PlayerSlugManager m_slugManager;
    private int m_playerSlugCount;
    


    // Start is called before the first frame update
    void Start()
    {
        m_player = ManageGameplay.Instance.playerCharacter;
        m_slugManager = m_player.GetComponent<PlayerSlugManager>();
    }

    // Update is called once per frame
    void Update()
    {
        int iDiff = 0;
        for (int i = 0; i < m_slugManager.m_lAssignedSlugs.Count; i++)
        {
            m_HUDUI[i].SetActive(true);
            iDiff++;
        }
        for (int i = iDiff; i < m_HUDUI.Count; i++)
        {
            m_HUDUI[i].SetActive(false);
        }
    }
    
}
