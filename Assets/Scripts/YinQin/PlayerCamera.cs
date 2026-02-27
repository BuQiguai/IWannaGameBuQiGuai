using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class PlayerCamera : MonoBehaviour
{
    static int fangda = 0;
    // PixelPerfectCamera camera;
    float xStart;
    float yStart;

    Player player;
    private void Start()
    {
        // camera = GetComponent<PixelPerfectCamera>();

        xStart = transform.position.x;
        yStart = transform.position.y;
    }
    private void FixedUpdate()
    {

        if (PlayInput.获取按键按下状态(KeyCode.N))
        {
            fangda++;
            fangda %= 6;
        }

        player = GameObject.FindObjectOfType<Player>();
        if (player != null)
        {
            // var xFollow = player.x - xStart + camera.refResolutionX / 2;
            // var yFollow = player.y - yStart + camera.refResolutionY / 2;

            // var width = camera.refResolutionX;
            // var height = camera.refResolutionY;

            if (fangda >= 3)
            {

                Camera.main.orthographicSize = 100 + 60f * (fangda-2);
                transform.position = Vector3.Lerp(transform.position, new Vector3(player.x, player.y, transform.position.z), 0.05f);

            }
            else
            {
                Camera.main.orthographicSize = 304f + 60f * fangda;
                var xFollow = player.x - xStart + 800 / 2;
                var yFollow = player.y - yStart + 608 / 2;

                var width = 800;
                var height = 608;

                transform.position = new Vector3(Mathf.Floor(xFollow / width) * width + xStart,
                    Mathf.Floor(yFollow / height) * height + yStart, transform.position.z);
            }

        }
    }

}
