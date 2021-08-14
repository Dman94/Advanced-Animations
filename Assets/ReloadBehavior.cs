using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadBehavior : StateMachineBehaviour
{

    // reseting the reloading state on the exit of animation
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       WeaponLogic weaponLogic = animator.GetComponentInChildren<WeaponLogic>();
       weaponLogic.setReloadingState(false);
    }
}
