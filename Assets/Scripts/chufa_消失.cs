using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chufa_消失 : DongZuoBase
{
    public override void 动作()
    {
        this.gameObject.SetActive(false);
    }

    protected override void 重置()
    {
        this.gameObject.SetActive(true);
    }

}
