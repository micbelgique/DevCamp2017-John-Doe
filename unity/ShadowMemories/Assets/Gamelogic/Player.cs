using Assets.Command;
using Assets.Scripts.Network;
using Assets.Scripts.Network.Message;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public struct Shadow
{
    [Tooltip("object containing the shadow object with animation")]
    public GameObject gameObject;
    [Tooltip("animation's name to activate on the shadow object")]
    public string animation;
}

public class Player : MonoBehaviour {
    public GameObject[] soundObjects;
    public Shadow shadow;

    [Tooltip("step of focus accumulator value")]
    public float focusStep = 1.0f;
    [Tooltip("count of step of animation")]
    public float animationStepCount = 20.0f;
    [Tooltip("animation speed")]
    public float animationSpeed = 0.02f;
    [Tooltip("animation duration")]
    public float animationDuration = 1.15f;

    // current player state focus
    private float frequencyState = 0;
    private Animation shadowAnimation=null;
    private float focusCount = 0.0f;
    private float frequencyCount = 0.0f;

    private void createFocus(AbstractCommand command)
    {
        FocusCommand fcommand = (FocusCommand)command;
        if (fcommand.value > 0.0f)
            focusCount = focusCount + focusStep;
        else if (fcommand.value<0.0f)
            focusCount = focusCount - focusStep;

        this.shadowAnimation[this.shadow.animation].speed = animationSpeed;
    }

    private void createFrequency(AbstractCommand command)
    {
        FrequencyCommand fcommand = (FrequencyCommand)command;
        this.frequencyState = fcommand.value;
    }

    private float getStepValue() {
        return focusCount;
    }

    // Use this for initialization
    void Start () {
        this.shadowAnimation = this.shadow.gameObject.GetComponent<Animation>();
        if (shadowAnimation) {
            this.shadowAnimation[this.shadow.animation].enabled = true;
            this.shadowAnimation[this.shadow.animation].weight = 1f;
            this.shadowAnimation[this.shadow.animation].speed = 0.0f; //to make the animation pause
        }

        StreamManager.onReceiveCommandAsObservable(this.gameObject)
             .Subscribe(message => {
                 Debug.Log("message suscribed");
                 CommandExecuter executer = new CommandExecuter(new Dictionary<string, CommandAction>() {
                                                                            { "focus" , new CommandAction(command=> createFocus(command)) },
                                                                            { "frequency", new CommandAction(command=> createFrequency(command))}
                                                               });
                 executer.execute(message.commands);
             },
             () => Debug.Log("stream completed")
           );
    }

    // Update is called once per frame
    void Update() {
        for (int iSound = 0; iSound < soundObjects.Length; iSound++) {
            AudioSource source=soundObjects[iSound].GetComponent<AudioSource>();
            if (source.volume <= 1.0f / animationStepCount * this.getStepValue()) {
                source.volume += animationSpeed * Time.deltaTime;
            }
        }

        if (this.shadowAnimation) {
            if(this.shadowAnimation[this.shadow.animation].time>= this.animationDuration / animationStepCount * this.getStepValue())
                this.shadowAnimation[this.shadow.animation].speed = 0.0f;
        }

    }
}
