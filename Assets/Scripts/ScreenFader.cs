using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



// this is a graphic that can be used to fade on or off to help the screen transition
[RequireComponent(typeof(MaskableGraphic))]
public class ScreenFader : MonoBehaviour 
{



    // value to represent fully opaque
	public float solidAlpha = 1f;

    // value to represent fully transparent
	public float clearAlpha = 0f;

    // time to delay the effect
	public float delay = 0f;

    // time to transition from transparent to opaque or vice versa
	public float timeToFade = 1f;


    // reference to Image (can also be used for Text)
	MaskableGraphic m_graphic;

	void Start () 
	{
        // cache the Image/Text and set the color
		m_graphic = GetComponent<MaskableGraphic> ();
		
	}

    // routine for the fade effect, pass in a target alpha value
   IEnumerator FadeRoutine(float alpha)
    {
	    // pause for delay seconds
        yield return new WaitForSeconds(delay);

		// cross fade the alpha value of the MaskableGraphic
        m_graphic.CrossFadeAlpha(alpha, timeToFade, true);

    }


    // fade the screen fader on
	public void FadeOn()
	{
		StartCoroutine (FadeRoutine (solidAlpha));
	}

    // fade the screen fader off
	public void FadeOff()
	{
		StartCoroutine (FadeRoutine (clearAlpha));
	}





}
