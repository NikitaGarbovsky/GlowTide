using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class NewBehaviourScript : MonoBehaviour
{
    public GameObject m_testPlayer;
    public SlugThrowing m_slugThrower;
    public TextMeshProUGUI m_slugCountText;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        int m_slugCount = m_testPlayer.GetComponent<SlugThrowing>().m_slugCount;
       string m_slugCountString = m_slugCount.ToString();
        m_slugCountText.text = m_slugCountString;
    }
}
