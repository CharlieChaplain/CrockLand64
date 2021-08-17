using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrockAnimListener : MonoBehaviour
{
    public List<string> soundObjectNames;
    public List<PlaySound> soundObjects;

    Dictionary<string, PlaySound> soundObjDict;

    private void Start()
    {
        //populates the dictionary for later lookup
        soundObjDict = new Dictionary<string, PlaySound>();

        if (soundObjectNames.Count != soundObjects.Count)
            Debug.Log("SOUND OBJECTS AND NAMES LISTS DON'T MATCH!!");
        else
        {
            for(int i = 0; i < soundObjects.Count; i++)
            {
                soundObjDict.Add(soundObjectNames[i], soundObjects[i]);
            }
        }
    }

    public void ChangeState(string state)
    {
        PlayerManager.Instance.currentState = (PlayerManager.PlayerState)System.Enum.Parse(typeof(PlayerManager.PlayerState), state);
    }

    public void StopIdle()
    {
        transform.root.GetComponent<Idle>().StopIdle();
    }

    public void StopAttack()
    {
        transform.root.GetComponent<Attack>().StopAttack();
    }

    public void HurtboxOn(string hurtboxName)
    {
        bool toggle = true;
        GameObject hurtbox = transform.root.Find(hurtboxName).gameObject;
        if(hurtbox != null)
            HurtBoxInfo.ToggleHurtBox(hurtbox, toggle);
    }

    public void HurtboxOff(string hurtboxName)
    {
        bool toggle = false;
        GameObject hurtbox = transform.root.Find(hurtboxName).gameObject;
        if (hurtbox != null)
            HurtBoxInfo.ToggleHurtBox(hurtbox, toggle);
    }

    public void StartCarrying()
    {
        transform.root.GetComponent<Attack>().carrying = true;
    }

    public void ToggleMove(int flag)
    {
        PlayerManager.Instance.canMove = flag == 1;
    }

    public void ToggleMatchCarryPos(int flag)
    {
        transform.root.GetComponent<Attack>().matchTargetPos = flag == 1;
    }

    public void ThrowRelease()
    {
        transform.root.GetComponent<Attack>().ThrowRelease();
    }

    public void ToggleCarry()
    {
        transform.root.GetComponent<Attack>().carrying = false;
        transform.root.GetComponent<Attack>().throwing = true;
    }

    /// <summary>
    /// Toggles the special effects for an attack.
    /// </summary>
    /// <param name="toggle">string of 2 ints separated by a '.' First number is index in attackEffects (Attack script) Second number is off (0) or on (1)</param>
    public void ToggleEffect(string toggle)
    {
        string[] toggleNumbersStr = toggle.Split('.');
        List<int> toggleNumbers = new List<int>();
        foreach(string number in toggleNumbersStr)
        {
            toggleNumbers.Add(int.Parse(number));
        }

        //failsafe
        if (toggleNumbers.Count != 2)
        {
            Debug.Log("wrong number of arguments in anim event");
            return;
        }

        transform.root.GetComponent<Attack>().attackEffects[toggleNumbers[0]].SetActive(toggleNumbers[1] == 1);
    }

    public void PlaySound(string soundName)
    {
        soundObjDict[soundName].Play(transform.position);
    }

    public void PlayLeftFootstep(AnimationEvent evt)
    {
        //this ensures only the clip in the blend tree that has the most weight plays the footstep, prevents double footsteps
        if (evt.animatorClipInfo.weight > 0.5)
            transform.root.GetComponent<Footsteps>().PlayFootstep(-1);
    }
    public void PlayRightFootstep(AnimationEvent evt)
    {
        //this ensures only the clip in the blend tree that has the most weight plays the footstep, prevents double footsteps
        if (evt.animatorClipInfo.weight > 0.5)
            transform.root.GetComponent<Footsteps>().PlayFootstep(1);
    }

    public void IncrementIdleCounter()
    {
        transform.root.GetComponent<Idle>().Increment();
    }
}
