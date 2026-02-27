using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChuShiChangJing : MonoBehaviour
{
    // Start is called before the first frame FixedUpdate
    void Start()
    {
        //转跳到 Title 场景
        SceneManager.LoadScene("Title");
    }

    // FixedUpdate is called once per frame
    void FixedUpdate()
    {
        
    }
}
